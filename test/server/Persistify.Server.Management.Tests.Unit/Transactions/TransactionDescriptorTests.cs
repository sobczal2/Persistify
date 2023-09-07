using System;
using System.Collections.Generic;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Transactions;
using Xunit;

namespace Persistify.Server.Management.Tests.Unit.Transactions;

public class TransactionDescriptorTests
{
    [Fact]
    public void Ctor_WhenReadManagersIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var exclusiveGlobal = false;
        List<IManager> readManagers = null!;
        var writeManagers = new List<IManager>();

        // Act
        var exception = Record.Exception(() => new TransactionDescriptor(exclusiveGlobal, readManagers, writeManagers));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Ctor_WhenWriteManagersIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var exclusiveGlobal = false;
        var readManagers = new List<IManager>();
        List<IManager> writeManagers = null!;

        // Act
        var exception = Record.Exception(() => new TransactionDescriptor(exclusiveGlobal, readManagers, writeManagers));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Ctor_WhenReadManagersAndWriteManagersAreNotNull_SetsProperties()
    {
        // Arrange
        var exclusiveGlobal = false;
        var readManagers = new List<IManager>();
        var writeManagers = new List<IManager>();

        // Act
        var transactionDescriptor = new TransactionDescriptor(exclusiveGlobal, readManagers, writeManagers);

        // Assert
        Assert.Equal(exclusiveGlobal, transactionDescriptor.ExclusiveGlobal);
        Assert.Equal(readManagers, transactionDescriptor.ReadManagers);
        Assert.Equal(writeManagers, transactionDescriptor.WriteManagers);
    }
}
