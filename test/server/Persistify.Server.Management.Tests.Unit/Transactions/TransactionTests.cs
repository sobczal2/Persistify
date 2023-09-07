using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Management.Transactions.Exceptions;
using Xunit;

namespace Persistify.Server.Management.Tests.Unit.Transactions;

public class TransactionTests
{
    private readonly ITransactionDescriptor _transactionDescriptor;
    private readonly ITransactionState _transactionState;
    private readonly ILogger<Transaction> _logger;

    private Transaction _sut;

    private readonly TimeSpan _timeOut;

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
    public async Task BeginAsync_WhenTransactionStateIsNotCurrentTransaction_ThrowsTransactionStateCorruptedException()
    {
        // Arrange
        _transactionState.GetCurrentTransaction().Returns(Substitute.For<ITransaction>());

        // Act
        Func<Task> act = async () => await _sut.BeginAsync(_timeOut, CancellationToken.None);

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
    public async Task BeginAsync_InvokesBeginReadAsyncForEachReadManager()
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
    public async Task BeginAsync_InvokesBeginWriteAsyncForEachWriteManager()
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
}
