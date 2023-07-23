using Persistify.Persistence.DataStructures.Abstractions;
using Persistify.Persistence.DataStructures.Providers;

namespace Persistify.Persistence.DataStructures.Trees;

public class AvlAsyncTree<T> : IAsyncTree<T>
{
    private readonly IStorageProvider<Node> _storageProvider;
    private readonly IComparer<T> _comparer;
    private const long NullId = 0;
    private long _lastId = NullId;
    private long _rootId = NullId;
    private readonly IDictionary<long, Node> _changedNodes;
    private readonly SemaphoreSlim _semaphore;

    public class Node
    {
        public T Value { get; set; }
        public long Id { get; set; }
        public long LeftId { get; set; }
        public long RightId { get; set; }
        public long ParentId { get; set; }
        public int Height { get; set; }
        public int Balance { get; set; }

        public Node(T value, long id, long leftId, long rightId, long parentId, int height, int balance)
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
    }

    public AvlAsyncTree(
        IStorageProvider<Node> storageProvider,
        IComparer<T> comparer
    )
    {
        _storageProvider = storageProvider;
        _comparer = comparer;
        _changedNodes = new Dictionary<long, Node>();
        _semaphore = new SemaphoreSlim(1, 1);
    }

    public async ValueTask InsertAsync(T value)
    {
        await _semaphore.WaitAsync();
        try
        {
            await InsertInternalAsync(value);
        }
        finally
        {
            await FlushAsync();
            _semaphore.Release();
        }
    }

    public async ValueTask<T?> GetAsync(T value)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await GetInternalAsync(value);
        }
        finally
        {
            await FlushAsync();
            _semaphore.Release();
        }
    }

    public async ValueTask RemoveAsync(T value)
    {
        await _semaphore.WaitAsync();
        try
        {
            await RemoveInternalAsync(value);
        }
        finally
        {
            await FlushAsync();
            _semaphore.Release();
        }
    }

    private async ValueTask FlushAsync()
    {
        foreach (var node in _changedNodes)
        {
            if (node.Key == NullId)
            {
                continue;
            }

            await _storageProvider.WriteAsync(node.Key, node.Value);
        }

        _changedNodes.Clear();
    }

    private async ValueTask<Node?> ReadNodeAsync(long id)
    {
        if (_changedNodes.TryGetValue(id, out var node))
        {
            return node;
        }

        return await _storageProvider.ReadAsync(id);
    }

    private void WriteNode(Node node)
    {
        _changedNodes[node.Id] = node;
    }

    private async ValueTask InsertInternalAsync(T value)
    {
        if (_rootId == NullId)
        {
            _rootId = ++_lastId;
            var node = new Node(value, _rootId, NullId, NullId, NullId, 0, 0);
            WriteNode(node);
            return;
        }

        var currentNode = await ReadNodeAsync(_rootId);

        while (true)
        {
            if (currentNode is null)
            {
                throw new InvalidOperationException("Node is null");
            }

            if (_comparer.Compare(currentNode.Value, value) == 0)
            {
                throw new InvalidOperationException("Value already exists");
            }

            var parentNode = currentNode;

            var goLeft = _comparer.Compare(currentNode.Value, value) > 0;
            var childId = goLeft ? currentNode.LeftId : currentNode.RightId;

            if (childId == NullId)
            {
                if (goLeft)
                {
                    currentNode.LeftId = ++_lastId;
                    WriteNode(currentNode);
                }
                else
                {
                    currentNode.RightId = ++_lastId;
                    WriteNode(currentNode);
                }

                var node = new Node(value, _lastId, currentNode.Id);
                WriteNode(node);
                await BalanceAsync(parentNode);
                break;
            }

            currentNode = await ReadNodeAsync(childId);
        }
    }

    private async ValueTask<T?> GetInternalAsync(T value)
    {
        if (_rootId == NullId)
        {
            return default;
        }

        var currentNode = await ReadNodeAsync(_rootId);

        while (true)
        {
            if (currentNode is null)
            {
                return default;
            }

            if (_comparer.Compare(currentNode.Value, value) == 0)
            {
                return currentNode.Value;
            }

            var goLeft = _comparer.Compare(currentNode.Value, value) > 0;
            var childId = goLeft ? currentNode.LeftId : currentNode.RightId;

            if (childId == NullId)
            {
                return default;
            }

            currentNode = await ReadNodeAsync(childId);
        }
    }

    private async ValueTask RemoveInternalAsync(T value)
    {
        if (_rootId == NullId)
        {
            return;
        }

        var currentNode = await ReadNodeAsync(_rootId);

        while (true)
        {
            if (currentNode is null)
            {
                return;
            }

            if (_comparer.Compare(currentNode.Value, value) == 0)
            {
                await RemoveNodeAsync(currentNode);
                return;
            }

            var goLeft = _comparer.Compare(currentNode.Value, value) > 0;
            var childId = goLeft ? currentNode.LeftId : currentNode.RightId;

            if (childId == NullId)
            {
                return;
            }

            currentNode = await ReadNodeAsync(childId);
        }
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

            _rootId = node.Id;

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
                    _rootId = NullId;
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

        node.Height = Math.Max(leftHeight, rightHeight) + 1;
        node.Balance = await GetHeight(rightNode?.Id ?? NullId) - await GetHeight(leftNode?.Id ?? NullId);
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

        if(node.LeftId != NullId)
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

            if(parentNode.RightId == node.Id)
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

        if(node.RightId != NullId)
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

            if(parentNode.RightId == node.Id)
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

    private async ValueTask<int> GetHeight(long id)
    {
        var node = await ReadNodeAsync(id);

        return node?.Height ?? -1;
    }
}
