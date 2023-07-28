using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Server.Persistence.DataStructures.Abstractions;
using Persistify.Server.Persistence.DataStructures.Providers;
using ProtoBuf;

namespace Persistify.Server.Persistence.DataStructures.Trees;

public class AvlAsyncTree<T> : IAsyncTree<T>
{
    protected const long NullId = 0;
    protected readonly IDictionary<long, Node> ChangedNodes;
    protected readonly IComparer<T> Comparer;
    protected readonly SemaphoreSlim Semaphore;
    protected readonly IStorageProvider<Node> StorageProvider;
    protected long LastId = NullId;
    protected long RootId = NullId;

    public AvlAsyncTree(
        IStorageProvider<Node> storageProvider,
        IComparer<T> comparer
    )
    {
        StorageProvider = storageProvider;
        Comparer = comparer;
        ChangedNodes = new Dictionary<long, Node>();
        Semaphore = new SemaphoreSlim(1, 1);
    }

    public async ValueTask InsertAsync(T value)
    {
        await Semaphore.WaitAsync();
        try
        {
            await InsertInternalAsync(value);
        }
        finally
        {
            await FlushAsync();
            Semaphore.Release();
        }
    }

    public async ValueTask InsertOrPerformActionOnValueAsync<TArgs>(T value, Action<T, TArgs> action, TArgs args)
    {
        await Semaphore.WaitAsync();
        try
        {
            await InsertOrPerformActionOnValueInternalAsync(value, action, args);
        }
        finally
        {
            await FlushAsync();
            Semaphore.Release();
        }
    }

    public async ValueTask<T?> GetAsync(T value)
    {
        await Semaphore.WaitAsync();
        try
        {
            return await GetInternalAsync(value);
        }
        finally
        {
            await FlushAsync();
            Semaphore.Release();
        }
    }

    public async ValueTask RemoveAsync(T value)
    {
        await Semaphore.WaitAsync();
        try
        {
            await RemoveInternalAsync(value);
        }
        finally
        {
            await FlushAsync();
            Semaphore.Release();
        }
    }

    public async ValueTask PerformActionByPredicateAndMaybeRemoveAsync<TArgs>(Func<T, TArgs, bool> predicate, Func<T, TArgs, bool> maybeRemoveAction, TArgs args)
    {
        await Semaphore.WaitAsync();
        try
        {
            await PerformActionByPredicateAndMaybeRemoveInternalAsync(RootId, predicate, maybeRemoveAction, args);
        }
        finally
        {
            await FlushAsync();
            Semaphore.Release();
        }
    }

    private async ValueTask FlushAsync()
    {
        foreach (var node in ChangedNodes)
        {
            if (node.Key == NullId)
            {
                continue;
            }

            await StorageProvider.WriteAsync(node.Key, node.Value);
        }

        ChangedNodes.Clear();
    }

    protected async ValueTask<Node?> ReadNodeAsync(long id)
    {
        if(id == NullId)
        {
            return null;
        }

        if (ChangedNodes.TryGetValue(id, out var node))
        {
            return node;
        }

        return await StorageProvider.ReadAsync(id);
    }

    protected void WriteNode(Node node)
    {
        ChangedNodes[node.Id] = node;
    }

    private async ValueTask InsertInternalAsync(T value)
    {
        if (RootId == NullId)
        {
            RootId = ++LastId;
            var node = new Node(value, RootId, NullId, NullId, NullId, 0, 0);
            WriteNode(node);
            return;
        }

        var currentNode = await ReadNodeAsync(RootId);

        while (true)
        {
            if (currentNode is null)
            {
                throw new InvalidOperationException("Node is null");
            }

            if (Comparer.Compare(currentNode.Value, value) == 0)
            {
                throw new InvalidOperationException("Value already exists");
            }

            var parentNode = currentNode;

            var goLeft = Comparer.Compare(currentNode.Value, value) > 0;
            var childId = goLeft ? currentNode.LeftId : currentNode.RightId;

            if (childId == NullId)
            {
                if (goLeft)
                {
                    currentNode.LeftId = ++LastId;
                    WriteNode(currentNode);
                }
                else
                {
                    currentNode.RightId = ++LastId;
                    WriteNode(currentNode);
                }

                var node = new Node(value, LastId, currentNode.Id);
                WriteNode(node);
                await BalanceAsync(parentNode);
                break;
            }

            currentNode = await ReadNodeAsync(childId);
        }
    }

    private async ValueTask InsertOrPerformActionOnValueInternalAsync<TArgs>(T value, Action<T, TArgs> action, TArgs args)
    {
        if (RootId == NullId)
        {
            RootId = ++LastId;
            var node = new Node(value, RootId, NullId, NullId, NullId, 0, 0);
            WriteNode(node);
            return;
        }

        var currentNode = await ReadNodeAsync(RootId);

        while (true)
        {
            if (currentNode is null)
            {
                throw new InvalidOperationException("Node is null");
            }

            if (Comparer.Compare(currentNode.Value, value) == 0)
            {
                action(currentNode.Value, args);
                WriteNode(currentNode);
                return;
            }

            var parentNode = currentNode;

            var goLeft = Comparer.Compare(currentNode.Value, value) > 0;
            var childId = goLeft ? currentNode.LeftId : currentNode.RightId;

            if (childId == NullId)
            {
                if (goLeft)
                {
                    currentNode.LeftId = ++LastId;
                    WriteNode(currentNode);
                }
                else
                {
                    currentNode.RightId = ++LastId;
                    WriteNode(currentNode);
                }

                var node = new Node(value, LastId, currentNode.Id);
                WriteNode(node);
                await BalanceAsync(parentNode);
                break;
            }

            currentNode = await ReadNodeAsync(childId);
        }
    }

    private async ValueTask<T?> GetInternalAsync(T value)
    {
        if (RootId == NullId)
        {
            return default;
        }

        var currentNode = await StorageProvider.ReadAsync(RootId);

        while (true)
        {
            if (currentNode is null)
            {
                return default;
            }

            if (Comparer.Compare(currentNode.Value, value) == 0)
            {
                return currentNode.Value;
            }

            var goLeft = Comparer.Compare(currentNode.Value, value) > 0;
            var childId = goLeft ? currentNode.LeftId : currentNode.RightId;

            if (childId == NullId)
            {
                return default;
            }

            currentNode = await StorageProvider.ReadAsync(childId);
        }
    }

    private async ValueTask RemoveInternalAsync(T value)
    {
        if (RootId == NullId)
        {
            return;
        }

        var currentNode = await ReadNodeAsync(RootId);

        while (true)
        {
            if (currentNode is null)
            {
                return;
            }

            if (Comparer.Compare(currentNode.Value, value) == 0)
            {
                await RemoveNodeAsync(currentNode);
                return;
            }

            var goLeft = Comparer.Compare(currentNode.Value, value) > 0;
            var childId = goLeft ? currentNode.LeftId : currentNode.RightId;

            if (childId == NullId)
            {
                return;
            }

            currentNode = await ReadNodeAsync(childId);
        }
    }

    private async ValueTask PerformActionByPredicateAndMaybeRemoveInternalAsync<TArgs>(long currentId, Func<T, TArgs, bool> predicate, Func<T, TArgs, bool> maybeRemoveAction, TArgs args)
    {
        if(currentId == NullId)
        {
            return;
        }

        var currentNode = await ReadNodeAsync(currentId);

        if(currentNode is null)
        {
            throw new InvalidOperationException("Node is null");
        }

        if(predicate(currentNode.Value, args))
        {
            if(maybeRemoveAction(currentNode.Value, args))
            {
                await RemoveNodeAsync(currentNode);
            }
            else
            {
                WriteNode(currentNode);
            }
        }

        await PerformActionByPredicateAndMaybeRemoveInternalAsync(currentNode.LeftId, predicate, maybeRemoveAction, args);
        await PerformActionByPredicateAndMaybeRemoveInternalAsync(currentNode.RightId, predicate, maybeRemoveAction, args);
    }

    private async ValueTask BalanceAsync(Node node)
    {
        while (true)
        {
            await UpdateHeightBalanceAsync(node);

            if (node.Balance == -2)
            {
                var leftNode = await ReadNodeAsync(node.LeftId) ??
                               throw new InvalidOperationException("Left node is null");

                if (await GetHeight(leftNode.LeftId) >= await GetHeight(leftNode.RightId))
                {
                    var newId = await RotateRightAsync(node);
                    node = await ReadNodeAsync(newId) ??
                           throw new InvalidOperationException("New node is null");
                }
                else
                {
                    var newId = await RotateLeftRightAsync(node);
                    node = await ReadNodeAsync(newId) ??
                           throw new InvalidOperationException("New node is null");
                }
            }
            else if (node.Balance == 2)
            {
                var rightNode = await ReadNodeAsync(node.RightId) ??
                                throw new InvalidOperationException("Right node is null");

                if (await GetHeight(rightNode.RightId) >= await GetHeight(rightNode.LeftId))
                {
                    var newId = await RotateLeftAsync(node);
                    node = await ReadNodeAsync(newId) ??
                           throw new InvalidOperationException("New node is null");
                }
                else
                {
                    var newId = await RotateRightLeftAsync(node);
                    node = await ReadNodeAsync(newId) ??
                           throw new InvalidOperationException("New node is null");
                }
            }

            if (node.ParentId != NullId)
            {
                var parentNode = await ReadNodeAsync(node.ParentId) ??
                                 throw new InvalidOperationException("Parent node is null");

                node = parentNode;
                continue;
            }

            RootId = node.Id;

            break;
        }
    }

    private async ValueTask RemoveNodeAsync(Node node)
    {
        while (true)
        {
            if (node is { LeftId: NullId, RightId: NullId })
            {
                if (node.ParentId == NullId)
                {
                    RootId = NullId;
                }
                else
                {
                    var parentNode = await ReadNodeAsync(node.ParentId) ??
                                     throw new InvalidOperationException("Parent node is null");

                    if (parentNode.LeftId == node.Id)
                    {
                        parentNode.LeftId = NullId;
                        WriteNode(parentNode);
                    }
                    else
                    {
                        parentNode.RightId = NullId;
                        WriteNode(parentNode);
                    }

                    await BalanceAsync(parentNode);
                }

                return;
            }

            Node? childNode;
            if (node.LeftId != NullId)
            {
                childNode = await ReadNodeAsync(node.LeftId) ??
                            throw new InvalidOperationException("Left node is null");

                while (childNode.RightId != NullId)
                {
                    childNode = await ReadNodeAsync(childNode.RightId) ??
                                throw new InvalidOperationException("Right node is null");
                }
            }
            else
            {
                childNode = await ReadNodeAsync(node.RightId) ??
                            throw new InvalidOperationException("Right node is null");
                while (childNode.LeftId != NullId)
                {
                    childNode = await ReadNodeAsync(childNode.LeftId) ??
                                throw new InvalidOperationException("Left node is null");
                }
            }

            node.Value = childNode.Value;
            node.Id = childNode.Id;
            WriteNode(node);

            node = childNode;
        }
    }

    private async ValueTask UpdateHeightBalanceAsync(Node node)
    {
        var leftNode = await ReadNodeAsync(node.LeftId);
        var rightNode = await ReadNodeAsync(node.RightId);

        var leftHeight = leftNode?.Height ?? 0;
        var rightHeight = rightNode?.Height ?? 0;

        node.Height = (sbyte)(Math.Max(leftHeight, rightHeight) + 1);
        node.Balance = (sbyte)(await GetHeight(rightNode?.Id ?? NullId) - await GetHeight(leftNode?.Id ?? NullId));
        WriteNode(node);
    }

    private async ValueTask<long> RotateRightAsync(Node node)
    {
        var leftNode = await ReadNodeAsync(node.LeftId) ??
                       throw new InvalidOperationException("Left node is null");

        leftNode.ParentId = node.ParentId;
        WriteNode(leftNode);

        node.LeftId = leftNode.RightId;
        WriteNode(node);

        if (node.LeftId != NullId)
        {
            var nodeLeftNode = await ReadNodeAsync(node.LeftId) ??
                               throw new InvalidOperationException("Node left node is null");

            nodeLeftNode.ParentId = node.Id;
        }

        leftNode.RightId = node.Id;
        WriteNode(leftNode);

        node.ParentId = leftNode.Id;
        WriteNode(node);

        if (leftNode.ParentId != NullId)
        {
            var parentNode = await ReadNodeAsync(leftNode.ParentId) ??
                             throw new InvalidOperationException("Parent node is null");

            if (parentNode.RightId == node.Id)
            {
                parentNode.RightId = leftNode.Id;
                WriteNode(parentNode);
            }
            else
            {
                parentNode.LeftId = leftNode.Id;
                WriteNode(parentNode);
            }
        }

        await UpdateHeightBalanceAsync(node);
        await UpdateHeightBalanceAsync(leftNode);

        return leftNode.Id;
    }

    private async ValueTask<long> RotateLeftRightAsync(Node node)
    {
        var leftNode = await ReadNodeAsync(node.LeftId) ??
                       throw new InvalidOperationException("Left node is null");

        await RotateLeftAsync(leftNode);
        return await RotateRightAsync(node);
    }

    private async ValueTask<long> RotateLeftAsync(Node node)
    {
        var rightNode = await ReadNodeAsync(node.RightId) ??
                        throw new InvalidOperationException("Right node is null");

        rightNode.ParentId = node.ParentId;
        WriteNode(rightNode);

        node.RightId = rightNode.LeftId;
        WriteNode(node);

        if (node.RightId != NullId)
        {
            var nodeRightNode = await ReadNodeAsync(node.RightId) ??
                                throw new InvalidOperationException("Node right node is null");

            nodeRightNode.ParentId = node.Id;
        }

        rightNode.LeftId = node.Id;
        WriteNode(rightNode);

        node.ParentId = rightNode.Id;
        WriteNode(node);

        if (rightNode.ParentId != NullId)
        {
            var parentNode = await ReadNodeAsync(rightNode.ParentId) ??
                             throw new InvalidOperationException("Parent node is null");

            if (parentNode.RightId == node.Id)
            {
                parentNode.RightId = rightNode.Id;
                WriteNode(parentNode);
            }
            else
            {
                parentNode.LeftId = rightNode.Id;
                WriteNode(parentNode);
            }
        }

        await UpdateHeightBalanceAsync(node);
        await UpdateHeightBalanceAsync(rightNode);

        return rightNode.Id;
    }

    private async ValueTask<long> RotateRightLeftAsync(Node node)
    {
        var rightNode = await ReadNodeAsync(node.RightId) ??
                        throw new InvalidOperationException("Right node is null");

        await RotateRightAsync(rightNode);
        return await RotateLeftAsync(node);
    }

    private async ValueTask<sbyte> GetHeight(long id)
    {
        var node = await ReadNodeAsync(id);

        return node?.Height ?? (sbyte)-1;
    }

    [ProtoContract]
    public class Node
    {
        public Node(T value, long id, long leftId, long rightId, long parentId, sbyte height, sbyte balance)
        {
            Value = value;
            Id = id;
            LeftId = leftId;
            RightId = rightId;
            ParentId = parentId;
            Height = height;
            Balance = balance;
        }

        public Node(T value, long id, long parentId)
        {
            Value = value;
            Id = id;
            ParentId = parentId;
        }

        public Node()
        {
            Value = default!;
            Id = NullId;
            LeftId = NullId;
            RightId = NullId;
            ParentId = NullId;
            Height = 0;
            Balance = 0;
        }

        [ProtoMember(1)]
        public T Value { get; set; }
        [ProtoMember(2)]
        public long Id { get; set; }
        [ProtoMember(3)]
        public long LeftId { get; set; }
        [ProtoMember(4)]
        public long RightId { get; set; }
        [ProtoMember(5)]
        public long ParentId { get; set; }
        [ProtoMember(6)]
        public sbyte Height { get; set; }
        [ProtoMember(7)]
        public sbyte Balance { get; set; }
    }
}
