using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Management.Transactions.Exceptions;
using Xunit;

namespace Persistify.Server.Management.Tests.Unit.Transactions;

public class TransactionStateTests
{
    private readonly TransactionState _sut;

    public TransactionStateTests()
    {
        _sut = new TransactionState();
    }

    [Fact]
    public void GetCurrentTransaction_WhenCurrentTransactionIsNull_ThrowsTransactionStateCorruptedException()
    {
        // Arrange
        _sut.CurrentTransaction.Value = null;

        // Act
        Action action = () => _sut.GetCurrentTransaction();

        // Assert
        action.Should().Throw<TransactionStateCorruptedException>();
    }

    [Fact]
    public void GetCurrentTransaction_WhenCurrentTransactionIsNotNull_ReturnsCurrentTransaction()
    {
        // Arrange
        var transaction = Substitute.For<ITransaction>();
        _sut.CurrentTransaction.Value = transaction;

        // Act
        var result = _sut.GetCurrentTransaction();

        // Assert
        result.Should().Be(transaction);
    }
}
