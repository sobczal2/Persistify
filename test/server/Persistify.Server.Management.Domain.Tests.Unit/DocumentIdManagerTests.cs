using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Server.Management.Abstractions.Exceptions;
using Persistify.Server.Management.Abstractions.Exceptions.DocumentId;
using Persistify.Server.Persistence.Core.Abstractions;
using Xunit;

namespace Persistify.Server.Management.Domain.Tests.Unit;

public class DocumentIdManagerTests
{
    private readonly DocumentIdManager _sut;
    private readonly ILinearRepositoryManager _linearRepositoryManager;
    private readonly ISet<int> _initializedTemplates;

    public DocumentIdManagerTests()
    {
        _linearRepositoryManager = Substitute.For<ILinearRepositoryManager>();
        _initializedTemplates = Substitute.For<ISet<int>>();
        _sut = new DocumentIdManager(_linearRepositoryManager);
        _sut.SetInitializedTemplates(_initializedTemplates);
    }

    [Fact]
    public async Task GetNextId_ThrowsTemplateNotInitializedException_WhenTemplateIsNotInitialized()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(false);

        // Act
        var action = new Func<Task>(async () => await _sut.GetNextIdAsync(1));

        // Assert
        await action.Should().ThrowAsync<TemplateNotInitializedException>();
        _linearRepositoryManager.DidNotReceive().Get(Arg.Any<string>());
    }

    [Fact]
    public async Task GetNextId_ThrowsManagerInternalException_WhenCurrentIdIsNull()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(true);
        var repository = Substitute.For<ILinearRepository>();
        repository.ReadAsync(Arg.Any<long>()).Returns(ValueTask.FromResult<long?>(null));
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);

        // Act
        var action = new Func<Task>(async () => await _sut.GetNextIdAsync(1));

        // Assert
        await action.Should().ThrowAsync<ManagerInternalException>();
        await repository.Received(1).ReadAsync(Arg.Any<long>());
        await repository.DidNotReceive().WriteAsync(Arg.Any<long>(), Arg.Any<long>());
    }

    [Fact]
    public async Task GetNextId_ReturnsNextId_WhenCurrentIdIsNotNull()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(true);
        var repository = Substitute.For<ILinearRepository>();
        repository.ReadAsync(Arg.Any<long>()).Returns(ValueTask.FromResult<long?>(1));
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);

        // Act
        var result = await _sut.GetNextIdAsync(1);

        // Assert
        result.Should().Be(2);
        await repository.Received(1).ReadAsync(1);
        await repository.Received(1).WriteAsync(1, result);
    }

    [Fact]
    public async Task GetNextId_ReturnsNextId_WhenCurrentIdIsNotNullAndCurrentIdIsZero()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(true);
        var repository = Substitute.For<ILinearRepository>();
        repository.ReadAsync(Arg.Any<long>()).Returns(ValueTask.FromResult<long?>(0));
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);

        // Act
        var result = await _sut.GetNextIdAsync(1);

        // Assert
        result.Should().Be(1);
        await repository.Received(1).ReadAsync(1);
        await repository.Received(1).WriteAsync(1, result);
    }

    [Fact]
    public async Task GetCurrentId_ThrowsTemplateNotInitializedException_WhenTemplateIsNotInitialized()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(false);

        // Act
        var action = new Func<Task>(async () => await _sut.GetCurrentIdAsync(1));

        // Assert
        await action.Should().ThrowAsync<TemplateNotInitializedException>();
        _linearRepositoryManager.DidNotReceive().Get(Arg.Any<string>());
    }

    [Fact]
    public async Task GetCurrentId_ThrowsManagerInternalException_WhenCurrentIdIsNull()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(true);
        var repository = Substitute.For<ILinearRepository>();
        repository.ReadAsync(Arg.Any<long>()).Returns(ValueTask.FromResult<long?>(null));
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);

        // Act
        var action = new Func<Task>(async () => await _sut.GetCurrentIdAsync(1));

        // Assert
        await action.Should().ThrowAsync<ManagerInternalException>();
        await repository.Received(1).ReadAsync(Arg.Any<long>());
    }

    [Fact]
    public async Task GetCurrentId_ReturnsCurrentId_WhenCurrentIdIsNotNull()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(true);
        var repository = Substitute.For<ILinearRepository>();
        repository.ReadAsync(Arg.Any<long>()).Returns(ValueTask.FromResult<long?>(1));
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);

        // Act
        var result = await _sut.GetCurrentIdAsync(1);

        // Assert
        result.Should().Be(1);
        await repository.Received(1).ReadAsync(1);
    }

    [Fact]
    public async Task GetCurrentId_ReturnsCurrentId_WhenCurrentIdIsNotNullAndCurrentIdIsZero()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(true);
        var repository = Substitute.For<ILinearRepository>();
        repository.ReadAsync(Arg.Any<long>()).Returns(ValueTask.FromResult<long?>(0));
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);

        // Act
        var result = await _sut.GetCurrentIdAsync(1);

        // Assert
        result.Should().Be(0);
        await repository.Received(1).ReadAsync(1);
    }

    [Fact]
    public async Task
        InitializeForTemplateAsync_ThrowsTemplateAlreadyInitializedException_WhenTemplateIsAlreadyInitialized()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(true);

        // Act
        var action = new Func<Task>(async () => await _sut.InitializeForTemplateAsync(1));

        // Assert
        await action.Should().ThrowAsync<TemplateAlreadyInitializedException>();
        _linearRepositoryManager.DidNotReceive().Get(Arg.Any<string>());
    }

    [Fact]
    public async Task
        InitializedForTemplateAsync_WritesZeroToRepositoryAndAddsIdToInitializedTemplates_WhenTemplateIsNotInitialized()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(false);
        var repository = Substitute.For<ILinearRepository>();
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);

        // Act
        await _sut.InitializeForTemplateAsync(1);

        // Assert
        await repository.Received(1).WriteAsync(1, 0);
        _initializedTemplates.Received(1).Add(1);
    }

    [Fact]
    public async Task RemoveForTemplateAsync_ThrowsTemplateNotInitializedException_WhenTemplateIsNotInitialized()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(false);

        // Act
        var action = new Func<Task>(async () => await _sut.RemoveForTemplateAsync(1));

        // Assert
        await action.Should().ThrowAsync<TemplateNotInitializedException>();
        _linearRepositoryManager.DidNotReceive().Get(Arg.Any<string>());
        _linearRepositoryManager.DidNotReceive().Delete(Arg.Any<string>());
    }

    [Fact]
    public async Task
        RemoveForTemplateAsync_RemovesTemplateIdFromRepositoryAndRemovesIdFromInitializedTemplates_WhenTemplateIsInitialized()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(true);
        var repository = Substitute.For<ILinearRepository>();
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);

        // Act
        await _sut.RemoveForTemplateAsync(1);

        // Assert
        await repository.Received(1).RemoveAsync(1);
        _initializedTemplates.Received(1).Remove(1);
    }

    [Fact]
    public async Task GetInitializedTemplatesAsync_ReturnsAllInitializedTemplates_WhenInitializedTemplatesIsNotEmpty()
    {
        // Arrange
        _sut.SetInitializedTemplates(new HashSet<int> { 1, 2, 3 });

        // Act
        var result = await _sut.GetInitializedTemplatesAsync();

        // Assert
        result.Should().BeEquivalentTo(new HashSet<int> { 1, 2, 3 });
    }

    [Fact]
    public async Task GetInitializedTemplatesAsync_ReturnsEmptyHashSet_WhenInitializedTemplatesIsEmpty()
    {
        // Arrange

        // Act
        var result = await _sut.GetInitializedTemplatesAsync();

        // Assert
        result.Should().BeEquivalentTo(new HashSet<int>());
    }

    [Fact]
    public async Task ExistsForTemplateAsync_ReturnsTrue_WhenTemplateIsInitialized()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(true);

        // Act
        var result = await _sut.ExistsForTemplateAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsForTemplateAsync_ReturnsFalse_WhenTemplateIsNotInitialized()
    {
        // Arrange
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(false);

        // Act
        var result = await _sut.ExistsForTemplateAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task PerformStartupActionAsync_ThrowsManagerInternalException_WhenTemplateIdIsLessThanOne()
    {
        // Arrange
        var repository = Substitute.For<ILinearRepository>();
        repository.ReadAllAsync()
            .Returns(ValueTask.FromResult<IEnumerable<(long, long)>>(new List<(long, long)> { (0, 0) }));
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);

        // Act
        var action = new Func<Task>(async () => await _sut.PerformStartupActionAsync());

        // Assert
        await action.Should().ThrowAsync<ManagerInternalException>();
        _linearRepositoryManager.Received(1).Get(Arg.Any<string>());
    }

    [Fact]
    public async Task PerformStartupActionAsync_ThrowsManagerInternalException_WhenTemplateIdIsGreaterThanMaxInt()
    {
        // Arrange
        var repository = Substitute.For<ILinearRepository>();
        repository.ReadAllAsync()
            .Returns(ValueTask.FromResult<IEnumerable<(long, long)>>(new List<(long, long)> { ((long)int.MaxValue + 1, 0) }));
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);

        // Act
        var action = new Func<Task>(async () => await _sut.PerformStartupActionAsync());

        // Assert
        await action.Should().ThrowAsync<ManagerInternalException>();
        _linearRepositoryManager.Received(1).Get(Arg.Any<string>());
    }

    [Fact]
    public async Task PerformStartupActionAsync_ThrowsManagerInternalException_WhenTemplateAlreadyInitialized()
    {
        // Arrange
        var repository = Substitute.For<ILinearRepository>();
        repository.ReadAllAsync()
            .Returns(ValueTask.FromResult<IEnumerable<(long, long)>>(new List<(long, long)> { (1, 0) }));
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(true);

        // Act
        var action = new Func<Task>(async () => await _sut.PerformStartupActionAsync());

        // Assert
        await action.Should().ThrowAsync<ManagerInternalException>();
        _linearRepositoryManager.Received(1).Get(Arg.Any<string>());
    }

    [Fact]
    public async Task PerformStartupActionAsync_AddsTemplateIdToInitializedTemplates_WhenTemplateIsNotInitialized()
    {
        // Arrange
        var repository = Substitute.For<ILinearRepository>();
        repository.ReadAllAsync()
            .Returns(ValueTask.FromResult<IEnumerable<(long, long)>>(new List<(long, long)> { (1, 0) }));
        _linearRepositoryManager.Get(Arg.Any<string>()).Returns(repository);
        _initializedTemplates.Contains(Arg.Any<int>()).Returns(false);

        // Act
        await _sut.PerformStartupActionAsync();

        // Assert
        _initializedTemplates.Received(1).Add(1);
    }
}
