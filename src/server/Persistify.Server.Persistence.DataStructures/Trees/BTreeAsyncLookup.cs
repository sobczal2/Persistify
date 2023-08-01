using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.Persistence.Core.Abstractions;
using Persistify.Server.Persistence.DataStructures.Abstractions;
using Persistify.Server.Persistence.DataStructures.Exceptions;

namespace Persistify.Server.Persistence.DataStructures.Trees;

public class BTreeAsyncLookup<TKey, TItem> : IAsyncLookup<TKey, TItem>
    where TKey : notnull
{
    private readonly IRepository<BTreeInternalNode<TKey>> _internalNodeStorageProvider;
    private readonly IIntLinearRepository _intLinearRepository;
    private readonly int _degree;
    private readonly IRepository<BTreeLeafNode<TKey, TItem>> _leafNodeStorageProvider;
    private const long NullId = 0;
    private long _rootId;
    private bool _isRootLeaf;
    private long _maxInternalNodeId;
    private long _maxLeafNodeId;
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly IComparer<TKey> _comparer;
    private readonly Dictionary<long, BTreeInternalNode<TKey>> _internalNodeBuffer;
    private readonly Dictionary<long, BTreeLeafNode<TKey, TItem>> _leafNodeBuffer;

    public BTreeAsyncLookup(
        IRepository<BTreeInternalNode<TKey>> internalNodeStorageProvider,
        IRepository<BTreeLeafNode<TKey, TItem>> leafNodeStorageProvider,
        IIntLinearRepository intLinearRepository,
        int degree,
        IComparer<TKey> comparer
    )
    {
        _leafNodeStorageProvider = leafNodeStorageProvider;
        _internalNodeStorageProvider = internalNodeStorageProvider;
        _intLinearRepository = intLinearRepository;
        _degree = degree;
        _rootId = NullId;
        _isRootLeaf = true;
        _maxInternalNodeId = NullId;
        _maxLeafNodeId = NullId;
        _semaphoreSlim = new SemaphoreSlim(1, 1);
        _comparer = comparer;
        _internalNodeBuffer = new Dictionary<long, BTreeInternalNode<TKey>>();
        _leafNodeBuffer = new Dictionary<long, BTreeLeafNode<TKey, TItem>>();
    }

    public async ValueTask InitializeAsync()
    {
        _rootId = await _intLinearRepository.ReadAsync(1) ?? NullId;
        _isRootLeaf = (await _intLinearRepository.ReadAsync(2) ?? 1) == 1;
        _maxInternalNodeId = await _intLinearRepository.ReadAsync(3) ?? NullId;
        _maxLeafNodeId = await _intLinearRepository.ReadAsync(4) ?? NullId;
    }

    private async ValueTask IncrementMaxInternalNodeIdAsync()
    {
        _maxInternalNodeId++;
        await _intLinearRepository.WriteAsync(3, _maxInternalNodeId);
    }

    private async ValueTask IncrementMaxLeafNodeIdAsync()
    {
        _maxLeafNodeId++;
        await _intLinearRepository.WriteAsync(4, _maxLeafNodeId);
    }

    private async ValueTask WriteRootIdAsync()
    {
        await _intLinearRepository.WriteAsync(1, _rootId);
    }

    private async ValueTask WriteIsRootLeafAsync()
    {
        await _intLinearRepository.WriteAsync(2, _isRootLeaf ? 1 : 0);
    }

    private async ValueTask<IBTreeNode?> ReadNodeAsync(long rootId, bool isRootLeaf)
    {
        if (rootId == NullId)
        {
            return null;
        }

        if (isRootLeaf)
        {
            if (_leafNodeBuffer.TryGetValue(rootId, out var leafNode))
            {
                return leafNode;
            }

            return await _leafNodeStorageProvider.ReadAsync(rootId);
        }

        if (_internalNodeBuffer.TryGetValue(rootId, out var internalNode))
        {
            return internalNode;
        }

        return await _internalNodeStorageProvider.ReadAsync(rootId);
    }

    private void WriteNodeAsync(IBTreeNode node)
    {
        switch (node)
        {
            case BTreeInternalNode<TKey> internalNode:
                _internalNodeBuffer[internalNode.Id] = internalNode;
                break;
            case BTreeLeafNode<TKey, TItem> leafNode:
                _leafNodeBuffer[leafNode.Id] = leafNode;
                break;
        }
    }

    private async ValueTask RemoveNodeAsync(IBTreeNode node)
    {
        switch (node)
        {
            case BTreeInternalNode<TKey> internalNode:
                _internalNodeBuffer.Remove(internalNode.Id);
                await _internalNodeStorageProvider.DeleteAsync(internalNode.Id);
                break;
            case BTreeLeafNode<TKey, TItem> leafNode:
                _leafNodeBuffer.Remove(leafNode.Id);
                await _leafNodeStorageProvider.DeleteAsync(leafNode.Id);
                break;
        }
    }

    private async ValueTask FlushBuffersAsync()
    {
        foreach (var (id, internalNode) in _internalNodeBuffer)
        {
            await _internalNodeStorageProvider.WriteAsync(id, internalNode);
        }

        foreach (var (id, leafNode) in _leafNodeBuffer)
        {
            await _leafNodeStorageProvider.WriteAsync(id, leafNode);
        }
    }

    public async ValueTask AddAsync(TKey key, TItem item)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            await AddInternalAsync(key, item);
            await FlushBuffersAsync();
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask<List<TItem>> GetAsync(TKey key)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            return await GetInternalAsync(key);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask<List<TItem>> GetRangeAsync(TKey from, TKey to)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            return await GetRangeInternalAsync(from, to);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async ValueTask RemoveAsync(TKey key, TItem item)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            await RemoveInternalAsync(key, item);
            await FlushBuffersAsync();
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private async ValueTask AddInternalAsync(TKey key, TItem item)
    {
        var node = await FindLeafNodeAsync(key);

        if (node.Items.TryGetValue(key, out var existingItem))
        {
            existingItem.Add(item);
        }
        else
        {
            node.Items.Add(key, new List<TItem> { item });
        }

        WriteNodeAsync(node);

        if (node.Items.Count > (2 * _degree - 1))
        {
            await SplitLeafNodeAsync(node);
        }
    }

    private async ValueTask<List<TItem>> GetInternalAsync(TKey key)
    {
        var node = await FindLeafNodeAsync(key);

        if (node.Items.TryGetValue(key, out var items))
        {
            // TODO: Sort items more efficiently
            items.Sort();
            return items;
        }

        return Enumerable.Empty<TItem>().ToList();
    }

    private async ValueTask<List<TItem>> GetRangeInternalAsync(TKey from, TKey to)
    {
        BTreeLeafNode<TKey, TItem>? node = await FindLeafNodeAsync(from);

        var result = new List<TItem>();

        while (node != null)
        {
            foreach (var (key, items) in node.Items)
            {
                if (_comparer.Compare(key, from) >= 0 && _comparer.Compare(key, to) <= 0)
                {
                    result.AddRange(items);
                }
            }

            node = await ReadNodeAsync(node.RightSiblingId, true) as BTreeLeafNode<TKey, TItem>;
        }

        // TODO: Sort items more efficiently
        result.Sort();
        return result;
    }

    private async ValueTask RemoveInternalAsync(TKey key, TItem item)
    {
        var node = await FindLeafNodeAsync(key);

        if (node.Items.TryGetValue(key, out var items))
        {
            items.Remove(item);

            if (items.Count == 0)
            {
                node.Items.Remove(key);
            }
        }

        WriteNodeAsync(node);

        if (node.Items.Count < _degree - 1)
        {
            if (await TryBorrowFromSiblingAsync(node))
            {
                return;
            }

            await MergeLeafNodeAsync(node);
        }
    }

    private async ValueTask<BTreeLeafNode<TKey, TItem>> FindLeafNodeAsync(TKey key)
    {
        if (_rootId == NullId)
        {
            await IncrementMaxLeafNodeIdAsync();
            var createdLeafNode = new BTreeLeafNode<TKey, TItem>
            {
                Id = _maxLeafNodeId,
                ParentId = NullId,
                LeftSiblingId = NullId,
                RightSiblingId = NullId,
                Items = new SortedList<TKey, List<TItem>>(_comparer)
            };
            _rootId = _maxLeafNodeId;
            await WriteRootIdAsync();
            _isRootLeaf = true;
            await WriteIsRootLeafAsync();
            WriteNodeAsync(createdLeafNode);
            return createdLeafNode;
        }

        if (_isRootLeaf)
        {
            return await _leafNodeStorageProvider.ReadAsync(_rootId) ?? throw new ProviderDataCorruptedException();
        }

        BTreeLeafNode<TKey, TItem> leafNode;
        var internalNode = await _internalNodeStorageProvider.ReadAsync(_rootId) ??
                           throw new ProviderDataCorruptedException();
        while (true)
        {
            var tempNodeId = internalNode.GetChildId(key, _comparer);
            if (tempNodeId.leaf)
            {
                leafNode = await _leafNodeStorageProvider.ReadAsync(tempNodeId.id) ??
                           throw new ProviderDataCorruptedException();
                break;
            }

            internalNode = await _internalNodeStorageProvider.ReadAsync(tempNodeId.id) ??
                           throw new ProviderDataCorruptedException();
        }

        return leafNode;
    }

    private async ValueTask SplitLeafNodeAsync(BTreeLeafNode<TKey, TItem> node)
    {
        await IncrementMaxLeafNodeIdAsync();
        var newNode = new BTreeLeafNode<TKey, TItem>
        {
            Id = _maxLeafNodeId,
            ParentId = node.ParentId,
            LeftSiblingId = node.Id,
            RightSiblingId = node.RightSiblingId,
            Items = new SortedList<TKey, List<TItem>>(_comparer)
        };

        if (node.RightSiblingId != NullId)
        {
            var rightSibling = await _leafNodeStorageProvider.ReadAsync(node.RightSiblingId) ??
                               throw new ProviderDataCorruptedException();
            rightSibling.LeftSiblingId = newNode.Id;
            WriteNodeAsync(rightSibling);
        }

        node.RightSiblingId = newNode.Id;

        var middleIndex = node.Items.Count / 2;
        for (var i = middleIndex; i < node.Items.Count; i++)
        {
            newNode.Items.Add(node.Items.Keys[i], node.Items.GetValueAtIndex(i));
        }

        for (var i = node.Items.Count - 1; i >= middleIndex; i--)
        {
            node.Items.RemoveAt(i);
        }

        if (node.ParentId == NullId)
        {
            await IncrementMaxInternalNodeIdAsync();
            var newRoot = new BTreeInternalNode<TKey>
            {
                Id = _maxInternalNodeId,
                ParentId = NullId,
                ChildrenIds = new List<(long id, bool leaf)>(2) { (node.Id, true), (newNode.Id, true) },
                LeftSiblingId = NullId,
                RightSiblingId = NullId,
            };
            newRoot.Keys.Add(newNode.Items.Keys[0]);
            node.ParentId = newRoot.Id;
            newNode.ParentId = newRoot.Id;
            if (_rootId != newRoot.Id)
            {
                _rootId = newRoot.Id;
                await WriteRootIdAsync();
            }

            _isRootLeaf = false;
            await WriteIsRootLeafAsync();
            WriteNodeAsync(newRoot);
            WriteNodeAsync(newNode);
            WriteNodeAsync(node);
        }
        else
        {
            var parent = await _internalNodeStorageProvider.ReadAsync(node.ParentId) ??
                         throw new ProviderDataCorruptedException();

            var newKey = newNode.Items.Keys[0];

            var nodeId = node.Id;
            int index = parent.ChildrenIds.FindIndex(x => x.id == nodeId);

            parent.ChildrenIds.Insert(index + 1, (newNode.Id, true));
            parent.Keys.Insert(index, newKey);

            WriteNodeAsync(parent);
            WriteNodeAsync(newNode);
            WriteNodeAsync(node);

            if (parent.Keys.Count > (2 * _degree - 1))
            {
                await SplitInternalNodeAsync(parent);
            }
        }
    }

    private async ValueTask SplitInternalNodeAsync(BTreeInternalNode<TKey> node)
    {
        var middleIndex = node.ChildrenIds.Count / 2;

        await IncrementMaxInternalNodeIdAsync();
        var newNode = new BTreeInternalNode<TKey>
        {
            Id = _maxInternalNodeId,
            ParentId = node.ParentId,
            ChildrenIds = node.ChildrenIds.GetRange(middleIndex, node.ChildrenIds.Count - middleIndex),
            LeftSiblingId = node.Id,
            RightSiblingId = node.RightSiblingId,
        };

        if (node.RightSiblingId != NullId)
        {
            var rightSibling = await _internalNodeStorageProvider.ReadAsync(node.RightSiblingId) ??
                               throw new ProviderDataCorruptedException();
            rightSibling.LeftSiblingId = newNode.Id;
            WriteNodeAsync(rightSibling);
        }

        node.RightSiblingId = newNode.Id;

        for (var i = middleIndex; i < node.Keys.Count; i++)
        {
            newNode.Keys.Add(node.Keys[i]);
        }

        for (var i = middleIndex; i < node.ChildrenIds.Count; i++)
        {
            var child = await ReadNodeAsync(node.ChildrenIds[i].id, node.ChildrenIds[i].leaf) ??
                        throw new ProviderDataCorruptedException();
            child.ParentId = newNode.Id;
            WriteNodeAsync(child);
        }

        var middleKey = node.Keys[middleIndex];

        node.Keys.RemoveRange(middleIndex - 1, node.Keys.Count - middleIndex + 1);
        node.ChildrenIds.RemoveRange(middleIndex, node.ChildrenIds.Count - middleIndex);

        if (node.ParentId == NullId)
        {
            await IncrementMaxInternalNodeIdAsync();
            var newRoot = new BTreeInternalNode<TKey>
            {
                Id = _maxInternalNodeId,
                ParentId = NullId,
                ChildrenIds = new List<(long id, bool leaf)> { (node.Id, false), (newNode.Id, false) },
                Keys = new List<TKey> { middleKey }
            };
            node.ParentId = newRoot.Id;
            newNode.ParentId = newRoot.Id;
            _rootId = newRoot.Id;
            await WriteRootIdAsync();
            WriteNodeAsync(newRoot);
            WriteNodeAsync(newNode);
            WriteNodeAsync(node);
        }
        else
        {
            var parent = await _internalNodeStorageProvider.ReadAsync(node.ParentId) ??
                         throw new ProviderDataCorruptedException();

            var nodeId = node.Id;
            int index = parent.ChildrenIds.FindIndex(x => x.id == nodeId);
            parent.ChildrenIds.Insert(index + 1, (newNode.Id, false));
            parent.Keys.Insert(index, middleKey);


            WriteNodeAsync(parent);
            WriteNodeAsync(newNode);
            WriteNodeAsync(node);

            if (parent.Keys.Count == (2 * _degree - 1))
            {
                await SplitInternalNodeAsync(parent);
            }
        }
    }

    private ValueTask<bool> TryBorrowFromSiblingAsync(BTreeLeafNode<TKey, TItem> node)
    {
        throw new NotImplementedException();
    }

    private ValueTask MergeLeafNodeAsync(BTreeLeafNode<TKey, TItem> node)
    {
        throw new NotImplementedException();
    }

    private ValueTask MergeInternalNodeAsync(BTreeInternalNode<TKey> node)
    {
        throw new NotImplementedException();
    }
}
