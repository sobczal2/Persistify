using System;
using System.Collections.Generic;
using NSubstitute;
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
        var exception = Record.Exception(
            () => new TransactionDescriptor(exclusiveGlobal, readManagers, writeManagers)
        );

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
        var exception = Record.Exception(
            () => new TransactionDescriptor(exclusiveGlobal, readManagers, writeManagers)
        );

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
        var transactionDescriptor = new TransactionDescriptor(
            exclusiveGlobal,
            readManagers,
            writeManagers
        );

        // Assert
        Assert.Equal(exclusiveGlobal, transactionDescriptor.ExclusiveGlobal);
        Assert.Equal(readManagers, transactionDescriptor.ReadManagers);
        Assert.Equal(writeManagers, transactionDescriptor.WriteManagers);
    }

    [Fact]
    public void AddReadManager_WhenManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var exclusiveGlobal = false;
        var readManagers = new List<IManager>();
        var writeManagers = new List<IManager>();
        var transactionDescriptor = new TransactionDescriptor(
            exclusiveGlobal,
            readManagers,
            writeManagers
        );
        IManager manager = null!;

        // Act
        var exception = Record.Exception(() => transactionDescriptor.AddReadManager(manager));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void AddReadManager_WhenManagerIsNotNull_AddsManager()
    {
        // Arrange
        var exclusiveGlobal = false;
        var readManagers = new List<IManager>();
        var writeManagers = new List<IManager>();
        var transactionDescriptor = new TransactionDescriptor(
            exclusiveGlobal,
            readManagers,
            writeManagers
        );
        var manager = Substitute.For<IManager>();

        // Act
        transactionDescriptor.AddReadManager(manager);

        // Assert
        Assert.Contains(manager, transactionDescriptor.ReadManagers);
    }

    [Fact]
    public void AddWriteManager_WhenManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var exclusiveGlobal = false;
        var readManagers = new List<IManager>();
        var writeManagers = new List<IManager>();
        var transactionDescriptor = new TransactionDescriptor(
            exclusiveGlobal,
            readManagers,
            writeManagers
        );
        IManager manager = null!;

        // Act
        var exception = Record.Exception(() => transactionDescriptor.AddWriteManager(manager));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void AddWriteManager_WhenManagerIsNotNull_AddsManager()
    {
        // Arrange
        var exclusiveGlobal = false;
        var readManagers = new List<IManager>();
        var writeManagers = new List<IManager>();
        var transactionDescriptor = new TransactionDescriptor(
            exclusiveGlobal,
            readManagers,
            writeManagers
        );
        var manager = Substitute.For<IManager>();

        // Act
        transactionDescriptor.AddWriteManager(manager);

        // Assert
        Assert.Contains(manager, transactionDescriptor.WriteManagers);
    }
}
