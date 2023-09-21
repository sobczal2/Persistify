using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Management.Transactions.Exceptions;
using Xunit;

namespace Persistify.Server.Management.Tests.Unit.Transactions;

public class TransactionTests
{
    private readonly ILogger<Transaction> _logger;

    private readonly TimeSpan _timeOut;
    private readonly ITransactionDescriptor _transactionDescriptor;
    private readonly ITransactionState _transactionState;

    private Transaction _sut;

    public TransactionTests()
    {
        _transactionDescriptor = Substitute.For<ITransactionDescriptor>();
        _transactionState = Substitute.For<ITransactionState>();
        _logger = Substitute.For<ILogger<Transaction>>();

        _sut = new Transaction(_transactionDescriptor, _transactionState, _logger);

        _timeOut = TimeSpan.FromSeconds(1);
    }

    [Fact]
    public void Ctor_WhenTransactionDescriptorIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        TransactionDescriptor transactionDescriptor = null!;

        // Act
        Action act = () => _sut = new Transaction(transactionDescriptor, _transactionState, _logger);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenTransactionStateIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        ITransactionState transactionState = null!;

        // Act
        Action act = () => _sut = new Transaction(_transactionDescriptor, transactionState, _logger);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        ILogger<Transaction> logger = null!;

        // Act
        Action act = () => _sut = new Transaction(_transactionDescriptor, _transactionState, logger);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenCalled_SetsId()
    {
        // Arrange

        // Act
        _sut = new Transaction(_transactionDescriptor, _transactionState, _logger);

        // Assert
        _sut.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Ctor_WhenCalled_SetsPhaseToReady()
    {
        // Arrange

        // Act
        _sut = new Transaction(_transactionDescriptor, _transactionState, _logger);

        // Assert
        _sut.Phase.Should().Be(TransactionPhase.Ready);
    }

    [Fact]
    public async Task BeginAsync_WhenTransactionStateIsNotCurrentTransaction_ThrowsTransactionStateCorruptedException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(Substitute.For<ITransaction>());

        // Act
        var act = async () => await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<TransactionStateCorruptedException>();
    }

    [Fact]
    public async Task BeginAsync_WhenExclusiveGlobal_InvokesEnterWriteGlobalLockAsync()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionDescriptor.ExclusiveGlobal.Returns(true);

        // Act
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Assert
        await _transactionState.Received(1).EnterWriteGlobalLockAsync(_sut.Id, _timeOut, CancellationToken.None);
    }

    [Fact]
    public async Task BeginAsync_WhenNotExclusiveGlobal_InvokesEnterReadGlobalLockAsync()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionDescriptor.ExclusiveGlobal.Returns(false);

        // Act
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Assert
        await _transactionState.Received(1).EnterReadGlobalLockAsync(_sut.Id, _timeOut, CancellationToken.None);
    }

    [Fact]
    public async Task BeginAsync_WhenThereAreReadManagers_InvokesBeginReadAsyncForEachReadManager()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        var readManager1 = Substitute.For<IManager>();
        var readManager2 = Substitute.For<IManager>();
        _transactionDescriptor.ReadManagers.Returns(new[] { readManager1, readManager2 }.ToImmutableList());

        // Act
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Assert
        foreach (var readManager in _transactionDescriptor.ReadManagers)
        {
            await readManager.Received(1).BeginReadAsync(_timeOut, CancellationToken.None);
        }
    }

    [Fact]
    public async Task BeginAsync_WhenThereAreWriteManagers_InvokesBeginWriteAsyncForEachWriteManager()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        var writeManager1 = Substitute.For<IManager>();
        var writeManager2 = Substitute.For<IManager>();
        _transactionDescriptor.WriteManagers.Returns(new[] { writeManager1, writeManager2 }.ToImmutableList());

        // Act
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Assert
        foreach (var writeManager in _transactionDescriptor.WriteManagers)
        {
            await writeManager.Received(1).BeginWriteAsync(_timeOut, CancellationToken.None);
        }
    }

    [Fact]
    public async Task BeginAsync_WhenSuccessful_SetsPhaseToStarted()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);

        // Act
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Assert
        _sut.Phase.Should().Be(TransactionPhase.Started);
    }

    [Fact]
    public async Task BeginAsync_WhenCalledTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        var act = async () => await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Assert
        await act.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task PromoteManagerAsync_WhenCurrentTransactionIsNotThisTransaction_ThrowsTransactionStateCorruptedException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        _transactionState.GetCurrentTransaction().Returns(Substitute.For<ITransaction>());

        // Act
        var act = async () => await _sut.PromoteManagerAsync(Substitute.For<IManager>(), true, _timeOut);

        // Assert
        await act.Should().ThrowExactlyAsync<TransactionStateCorruptedException>();
    }

    [Fact]
    public async Task PromoteManagerAsync_WhenPhaseIsNotStarted_ThrowsInvalidOperationException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);

        // Act
        var act = async () => await _sut.PromoteManagerAsync(Substitute.For<IManager>(), true, _timeOut);

        // Assert
        await act.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task PromoteManagerAsync_WhenManagerIsAlreadyAReadManagerInThisTransaction_ThrowsInvalidOperationException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        var manager = Substitute.For<IManager>();
        _transactionDescriptor.ReadManagers.Returns(new[] { manager }.ToImmutableList());

        // Act
        var act = async () => await _sut.PromoteManagerAsync(manager, true, _timeOut);

        // Assert
        await act.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task PromoteManagerAsync_WhenManagerIsAlreadyAWriteManagerInThisTransaction_ThrowsInvalidOperationException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        var manager = Substitute.For<IManager>();
        _transactionDescriptor.WriteManagers.Returns(new[] { manager }.ToImmutableList());

        // Act
        var act = async () => await _sut.PromoteManagerAsync(manager, false, _timeOut);

        // Assert
        await act.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task PromoteManagerAsync_WhenWrite_InvokesBeginWriteAsync()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        var manager = Substitute.For<IManager>();

        // Act
        await _sut.PromoteManagerAsync(manager, true, _timeOut);

        // Assert
        await manager.Received(1).BeginWriteAsync(_timeOut, CancellationToken.None);
    }

    [Fact]
    public async Task PromoteManagerAsync_WhenRead_InvokesBeginReadAsync()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        var manager = Substitute.For<IManager>();

        // Act
        await _sut.PromoteManagerAsync(manager, false, _timeOut);

        // Assert
        await manager.Received(1).BeginReadAsync(_timeOut, CancellationToken.None);
    }

    [Fact]
    public async Task PromoteManagerAsync_WhenWrite_AddsManagerToWriteManagers()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        var manager = Substitute.For<IManager>();

        // Act
        await _sut.PromoteManagerAsync(manager, true, _timeOut);

        // Assert
        _transactionDescriptor.Received(1).AddWriteManager(manager);
    }

    [Fact]
    public async Task PromoteManagerAsync_WhenRead_AddsManagerToReadManagers()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        var manager = Substitute.For<IManager>();

        // Act
        await _sut.PromoteManagerAsync(manager, false, _timeOut);

        // Assert
        _transactionDescriptor.Received(1).AddReadManager(manager);
    }

    [Fact]
    public async Task PromoteManagerAsync_DoesNotChangePhase()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        var manager = Substitute.For<IManager>();

        // Act
        await _sut.PromoteManagerAsync(manager, true, _timeOut);

        // Assert
        _sut.Phase.Should().Be(TransactionPhase.Started);
    }

    [Fact]
    public async Task CommitAsync_WhenCurrentTransactionIsNotThisTransaction_ThrowsTransactionStateCorruptedException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        _transactionState.GetCurrentTransaction().Returns(Substitute.For<ITransaction>());

        // Act
        var act = async () => await _sut.CommitAsync();

        // Assert
        await act.Should().ThrowExactlyAsync<TransactionStateCorruptedException>();
    }

    [Fact]
    public async Task CommitAsync_WhenThereAreWriteManagers_InvokesExecutePendingActionsAsyncForEachWriteManager()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionState.CurrentTransaction.Returns(new AsyncLocal<ITransaction?>());
        var writeManager1 = Substitute.For<IManager>();
        var writeManager2 = Substitute.For<IManager>();
        _transactionDescriptor.WriteManagers.Returns(new[] { writeManager1, writeManager2 }.ToImmutableList());
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        await _sut.CommitAsync();

        // Assert
        foreach (var writeManager in _transactionDescriptor.WriteManagers)
        {
            await writeManager.Received(1).ExecutePendingActionsAsync();
        }
    }

    [Fact]
    public async Task CommitAsync_WhenOneWriteManagerThrowsOnExecutePendingActionsAsync_ConsumesExceptionAndContinues()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionState.CurrentTransaction.Returns(new AsyncLocal<ITransaction?>());
        var writeManager1 = Substitute.For<IManager>();
        var writeManager2 = Substitute.For<IManager>();
        _transactionDescriptor.WriteManagers.Returns(new[] { writeManager1, writeManager2 }.ToImmutableList());
        writeManager1.ExecutePendingActionsAsync().Returns(ValueTask.FromException(new Exception()));
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        await _sut.CommitAsync();

        // Assert
        foreach (var writeManager in _transactionDescriptor.WriteManagers)
        {
            await writeManager.Received(1).ExecutePendingActionsAsync();
        }
    }

    [Fact]
    public async Task CommitAsync_WhenThereAreReadManagers_InvokesEndReadAsyncForEachReadManager()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionState.CurrentTransaction.Returns(new AsyncLocal<ITransaction?>());
        var readManager1 = Substitute.For<IManager>();
        var readManager2 = Substitute.For<IManager>();
        _transactionDescriptor.ReadManagers.Returns(new[] { readManager1, readManager2 }.ToImmutableList());
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        await _sut.CommitAsync();

        // Assert
        foreach (var readManager in _transactionDescriptor.ReadManagers)
        {
            await readManager.Received(1).EndReadAsync();
        }
    }

    [Fact]
    public async Task CommitAsync_WhenOneReadManagerThrowsOnEndReadAsync_PropagatesException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionState.CurrentTransaction.Returns(new AsyncLocal<ITransaction?>());
        var readManager1 = Substitute.For<IManager>();
        var readManager2 = Substitute.For<IManager>();
        _transactionDescriptor.ReadManagers.Returns(new[] { readManager1, readManager2 }.ToImmutableList());
        readManager1.EndReadAsync().Returns(ValueTask.FromException(new Exception()));
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        var act = async () => await _sut.CommitAsync();

        // Assert
        await act.Should().ThrowExactlyAsync<Exception>();
    }

    [Fact]
    public async Task CommitAsync_WhenCalledTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        await _sut.CommitAsync();

        // Act
        var act = async () => await _sut.CommitAsync();

        // Assert
        await act.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CommitAsync_WhenSuccessful_SetsPhaseToCommitted()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        await _sut.CommitAsync();

        // Assert
        _sut.Phase.Should().Be(TransactionPhase.Committed);
    }

    [Fact]
    public async Task CommitAsync_WhenSuccessful_InvokesExitWriteGlobalLockAsync()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        _transactionDescriptor.ExclusiveGlobal.Returns(true);

        // Act
        await _sut.CommitAsync();

        // Assert
        await _transactionState.Received(1).ExitWriteGlobalLockAsync(_sut.Id);
    }

    [Fact]
    public async Task CommitAsync_WhenSuccessful_InvokesExitReadGlobalLockAsync()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        _transactionDescriptor.ExclusiveGlobal.Returns(false);

        // Act
        await _sut.CommitAsync();

        // Assert
        await _transactionState.Received(1).ExitReadGlobalLockAsync(_sut.Id);
    }

    [Fact]
    public async Task RollbackAsync_WhenCurrentTransactionIsNotThisTransaction_ThrowsTransactionStateCorruptedException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        _transactionState.GetCurrentTransaction().Returns(Substitute.For<ITransaction>());

        // Act
        var act = async () => await _sut.RollbackAsync();

        // Assert
        await act.Should().ThrowExactlyAsync<TransactionStateCorruptedException>();
    }

    [Fact]
    public async Task RollbackAsync_WhenPhaseIsNotStarted_ThrowsInvalidOperationException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);

        // Act
        var act = async () => await _sut.RollbackAsync();

        // Assert
        await act.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task RollbackAsync_WhenThereAreWriteManagers_InvokesClearPendingActionsForEachWriteManager()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionState.CurrentTransaction.Returns(new AsyncLocal<ITransaction?>());
        var writeManager1 = Substitute.For<IManager>();
        var writeManager2 = Substitute.For<IManager>();
        _transactionDescriptor.WriteManagers.Returns(new[] { writeManager1, writeManager2 }.ToImmutableList());
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        await _sut.RollbackAsync();

        // Assert
        foreach (var writeManager in _transactionDescriptor.WriteManagers)
        {
            writeManager.Received(1).ClearPendingActions();
        }
    }

    [Fact]
    public async Task RollbackAsync_WhenThereAreWriteManagers_InvokesEndWriteAsyncForEachWriteManager()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionState.CurrentTransaction.Returns(new AsyncLocal<ITransaction?>());
        var writeManager1 = Substitute.For<IManager>();
        var writeManager2 = Substitute.For<IManager>();
        _transactionDescriptor.WriteManagers.Returns(new[] { writeManager1, writeManager2 }.ToImmutableList());
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        await _sut.RollbackAsync();

        // Assert
        foreach (var writeManager in _transactionDescriptor.WriteManagers)
        {
            await writeManager.Received(1).EndWriteAsync();
        }
    }

    [Fact]
    public async Task RollbackAsync_WhenOneWriteManagerThrowsOnEndWriteAsync_PropagatesException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionState.CurrentTransaction.Returns(new AsyncLocal<ITransaction?>());
        var writeManager1 = Substitute.For<IManager>();
        var writeManager2 = Substitute.For<IManager>();
        _transactionDescriptor.WriteManagers.Returns(new[] { writeManager1, writeManager2 }.ToImmutableList());
        writeManager1.EndWriteAsync().Returns(ValueTask.FromException(new Exception()));
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        var act = async () => await _sut.RollbackAsync();

        // Assert
        await act.Should().ThrowExactlyAsync<Exception>();
    }

    [Fact]
    public async Task RollbackAsync_WhenThereAreReadManagers_InvokesEndReadAsyncForEachReadManager()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionState.CurrentTransaction.Returns(new AsyncLocal<ITransaction?>());
        var readManager1 = Substitute.For<IManager>();
        var readManager2 = Substitute.For<IManager>();
        _transactionDescriptor.ReadManagers.Returns(new[] { readManager1, readManager2 }.ToImmutableList());
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        await _sut.RollbackAsync();

        // Assert
        foreach (var readManager in _transactionDescriptor.ReadManagers)
        {
            await readManager.Received(1).EndReadAsync();
        }
    }

    [Fact]
    public async Task RollbackAsync_WhenOneReadManagerThrowsOnEndReadAsync_PropagatesException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        _transactionState.CurrentTransaction.Returns(new AsyncLocal<ITransaction?>());
        var readManager1 = Substitute.For<IManager>();
        var readManager2 = Substitute.For<IManager>();
        _transactionDescriptor.ReadManagers.Returns(new[] { readManager1, readManager2 }.ToImmutableList());
        readManager1.EndReadAsync().Returns(ValueTask.FromException(new Exception()));
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        var act = async () => await _sut.RollbackAsync();

        // Assert
        await act.Should().ThrowExactlyAsync<Exception>();
    }

    [Fact]
    public async Task RollbackAsync_WhenCalledTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        await _sut.RollbackAsync();

        // Act
        var act = async () => await _sut.RollbackAsync();

        // Assert
        await act.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task RollbackAsync_WhenSuccessful_SetsPhaseToRolledBack()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);

        // Act
        await _sut.RollbackAsync();

        // Assert
        _sut.Phase.Should().Be(TransactionPhase.RolledBack);
    }

    [Fact]
    public async Task RollbackAsync_WhenSuccessful_InvokesExitWriteGlobalLockAsync()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        _transactionDescriptor.ExclusiveGlobal.Returns(true);

        // Act
        await _sut.RollbackAsync();

        // Assert
        await _transactionState.Received(1).ExitWriteGlobalLockAsync(_sut.Id);
    }

    [Fact]
    public async Task RollbackAsync_WhenSuccessful_InvokesExitReadGlobalLockAsync()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(_sut);
        await _sut.BeginAsync(_timeOut, CancellationToken.None);
        _transactionDescriptor.ExclusiveGlobal.Returns(false);

        // Act
        await _sut.RollbackAsync();

        // Assert
        await _transactionState.Received(1).ExitReadGlobalLockAsync(_sut.Id);
    }
}
