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
    private readonly ISet<Node> _changedNodes;
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
        _changedNodes = new HashSet<Node>();
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
            await _storageProvider.WriteAsync(node.Id, node);
        }
    }

    private async ValueTask InsertInternalAsync(T value)
    {
        if (_rootId == NullId)
        {
            _rootId = ++_lastId;
            var node = new Node(value, _rootId, NullId, NullId, NullId, 1, 0);
            _changedNodes.Add(node);
            return;
        }

        var currentNode = await _storageProvider.ReadAsync(_rootId);
        Node? parentNode;

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

            parentNode = currentNode;

            var goLeft = _comparer.Compare(currentNode.Value, value) > 0;
            var childId = goLeft ? currentNode.LeftId : currentNode.RightId;

            if (childId == NullId)
            {
                if (goLeft)
                {
                    currentNode.LeftId = ++_lastId;
                }
                else
                {
                    currentNode.RightId = ++_lastId;
                }

                var node = new Node(value, _lastId, currentNode.Id);
                _changedNodes.Add(node);
                break;
            }

            currentNode = await _storageProvider.ReadAsync(childId);
        }

        await BalanceAsync(parentNode);
    }

    private async ValueTask<T?> GetInternalAsync(T value)
    {
        if (_rootId == NullId)
        {
            return default;
        }

        var currentNode = await _storageProvider.ReadAsync(_rootId);

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

            currentNode = await _storageProvider.ReadAsync(childId);
        }
    }

    private async ValueTask RemoveInternalAsync(T value)
    {
        if (_rootId == NullId)
        {
            return;
        }

        var currentNode = await _storageProvider.ReadAsync(_rootId);

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

            currentNode = await _storageProvider.ReadAsync(childId);
        }
    }

    private async ValueTask BalanceAsync(Node node)
    {
        await UpdateHeightBalanceAsync(node);

        if (node.Balance == -2)
        {
            var leftNode = await _storageProvider.ReadAsync(node.LeftId) ??
                           throw new InvalidOperationException("Left node is null");

            if (leftNode.Balance <= 0)
            {
                await RotateRightAsync(node);
            }
            else
            {
                await RotateLeftRightAsync(node);
            }
        }
        else if (node.Balance == 2)
        {
            var rightNode = await _storageProvider.ReadAsync(node.RightId) ??
                            throw new InvalidOperationException("Right node is null");

            if (rightNode.Balance >= 0)
            {
                await RotateLeftAsync(node);
            }
            else
            {
                await RotateRightLeftAsync(node);
            }
        }

        if (node.ParentId != NullId)
        {
            var parentNode = await _storageProvider.ReadAsync(node.ParentId) ??
                             throw new InvalidOperationException("Parent node is null");

            await BalanceAsync(parentNode);
        }
        else
        {
            _rootId = node.Id;
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
                    var parentNode = await _storageProvider.ReadAsync(node.ParentId) ??
                                     throw new InvalidOperationException("Parent node is null");

                    if (parentNode.LeftId == node.Id)
                    {
                        parentNode.LeftId = NullId;
                    }
                    else
                    {
                        parentNode.RightId = NullId;
                    }

                    _changedNodes.Add(parentNode);
                    await BalanceAsync(parentNode);
                }

                return;
            }

            Node? childNode;
            if (node.LeftId != NullId)
            {
                childNode = await _storageProvider.ReadAsync(node.LeftId) ??
                            throw new InvalidOperationException("Left node is null");

                while (childNode.RightId != NullId)
                {
                    childNode = await _storageProvider.ReadAsync(childNode.RightId) ??
                                throw new InvalidOperationException("Right node is null");
                }
            }
            else
            {
                childNode = await _storageProvider.ReadAsync(node.RightId) ??
                            throw new InvalidOperationException("Right node is null");
                while (childNode.LeftId != NullId)
                {
                    childNode = await _storageProvider.ReadAsync(childNode.LeftId) ??
                                throw new InvalidOperationException("Left node is null");
                }
            }

            node.Value = childNode.Value;
            node.Id = childNode.Id;

            node = childNode;
        }
    }

    private async ValueTask UpdateHeightBalanceAsync(Node node)
    {
        var leftNode = await _storageProvider.ReadAsync(node.LeftId);
        var rightNode = await _storageProvider.ReadAsync(node.RightId);

        var leftHeight = leftNode?.Height ?? 0;
        var rightHeight = rightNode?.Height ?? 0;

        node.Height = Math.Max(leftHeight, rightHeight) + 1;
        node.Balance = rightHeight - leftHeight;
    }

    private async ValueTask RotateRightAsync(Node node)
    {
        var leftNode = await _storageProvider.ReadAsync(node.LeftId) ??
                       throw new InvalidOperationException("Left node is null");

        var leftRightNode = await _storageProvider.ReadAsync(leftNode.RightId);

        leftNode.ParentId = node.ParentId;
        leftNode.RightId = node.Id;
        node.ParentId = leftNode.Id;
        node.LeftId = leftRightNode?.Id ?? NullId;

        if (leftRightNode != null)
        {
            leftRightNode.ParentId = node.Id;
        }

        if (node.ParentId != NullId)
        {
            var parentNode = await _storageProvider.ReadAsync(node.ParentId) ??
                             throw new InvalidOperationException("Parent node is null");

            if (parentNode.LeftId == node.Id)
            {
                parentNode.LeftId = leftNode.Id;
            }
            else
            {
                parentNode.RightId = leftNode.Id;
            }
        }
        else
        {
            _rootId = leftNode.Id;
        }

        await UpdateHeightBalanceAsync(node);
        await UpdateHeightBalanceAsync(leftNode);

        _changedNodes.Add(node);
        _changedNodes.Add(leftNode);
    }

    private async ValueTask RotateLeftRightAsync(Node node)
    {
        var leftNode = await _storageProvider.ReadAsync(node.LeftId) ??
                       throw new InvalidOperationException("Left node is null");

        await RotateLeftAsync(leftNode);
        await RotateRightAsync(node);
    }

    private async ValueTask RotateLeftAsync(Node node)
    {
        var rightNode = await _storageProvider.ReadAsync(node.RightId) ??
                        throw new InvalidOperationException("Right node is null");

        var rightLeftNode = await _storageProvider.ReadAsync(rightNode.LeftId);

        rightNode.ParentId = node.ParentId;
        rightNode.LeftId = node.Id;
        node.ParentId = rightNode.Id;
        node.RightId = rightLeftNode?.Id ?? NullId;

        if (rightLeftNode != null)
        {
            rightLeftNode.ParentId = node.Id;
        }

        if (node.ParentId != NullId)
        {
            var parentNode = await _storageProvider.ReadAsync(node.ParentId) ??
                             throw new InvalidOperationException("Parent node is null");

            if (parentNode.LeftId == node.Id)
            {
                parentNode.LeftId = rightNode.Id;
            }
            else
            {
                parentNode.RightId = rightNode.Id;
            }
        }
        else
        {
            _rootId = rightNode.Id;
        }

        await UpdateHeightBalanceAsync(node);
        await UpdateHeightBalanceAsync(rightNode);
    }

    private async Task RotateRightLeftAsync(Node node)
    {
        var rightNode = await _storageProvider.ReadAsync(node.RightId) ??
                        throw new InvalidOperationException("Right node is null");

        await RotateRightAsync(rightNode);
        await RotateLeftAsync(node);
    }
}
