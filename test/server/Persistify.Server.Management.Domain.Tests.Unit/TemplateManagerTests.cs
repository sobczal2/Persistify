using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Exceptions;
using Persistify.Server.Management.Abstractions.Exceptions.Template;
using Persistify.Server.Persistence.Core.Abstractions;
using Xunit;

namespace Persistify.Server.Management.Domain.Tests.Unit;

public class TemplateManagerTests
{
    private TemplateManager _sut;

    private IRepositoryManager _repositoryManager;
    private IDocumentIdManager _documentIdManager;
    private SemaphoreSlim _generalTemplateSemaphore;
    private ConcurrentDictionary<int, SemaphoreSlim> _individualSemaphores;
    private ConcurrentDictionary<string, int> _templateNameToIdMap;
    private ConcurrentDictionary<int, Template> _templates;
    private IRepository<Template> _repository;
    private ILogger<TemplateManager> _logger;

    public TemplateManagerTests()
    {
        _repositoryManager = Substitute.For<IRepositoryManager>();
        _documentIdManager = Substitute.For<IDocumentIdManager>();
        _repository = Substitute.For<IRepository<Template>>();
        _repositoryManager.Get<Template>(Arg.Any<string>()).Returns(_repository);
        _logger = Substitute.For<ILogger<TemplateManager>>();
        _sut = new TemplateManager(_repositoryManager, _documentIdManager, _logger);
        _generalTemplateSemaphore = _sut.GetGeneralTemplateSemaphore();
        _individualSemaphores = _sut.GetIndividualSemaphores();
        _templateNameToIdMap = _sut.GetTemplateNameToIdMap();
        _templates = _sut.GetTemplates();
    }

    [Fact]
    public async Task
        CreateAsync_ThrowsTemplateWithThatNameAlreadyExistsException_WhenTemplateWithThatNameAlreadyExists()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _templateNameToIdMap.TryAdd(template.Name, 1);
        _templates.TryAdd(1, template);

        // Act
        Func<Task> act = async () => await _sut.CreateAsync(template);

        // Assert
        await act.Should().ThrowAsync<TemplateWithThatNameAlreadyExistsException>();
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
        _sut.GetLastTemplateId().Should().Be(0);
    }

    [Fact]
    public async Task CreateAsync_ThrowsManagerInternalException_WhenTemplateWithThatIdAlreadyExists()
    {
        // Arrange
        var template = new Template { Name = "Template1", Id = 1 };
        _templates.TryAdd(1, template);

        // Act
        Func<Task> act = async () => await _sut.CreateAsync(template);

        // Assert
        await act.Should().ThrowAsync<ManagerInternalException>();
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
        _sut.GetLastTemplateId().Should().Be(0);
    }

    [Fact]
    public async Task CreateAsync_RestoresPreviousState_WhenRepositoryWriteAsyncThrowsException()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _repository.When(x => x.WriteAsync(Arg.Any<long>(), Arg.Any<Template>())).Do(x => throw new Exception());

        // Act
        Func<Task> act = async () => await _sut.CreateAsync(template);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _sut.GetLastTemplateId().Should().Be(0);
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
        _individualSemaphores.Should().BeEmpty();
        _templateNameToIdMap.Should().BeEmpty();
        _templates.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_RestoresPreviousState_WhenRepositoryManagerCreateThrowsException()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _repositoryManager.When(x => x.Create<Document>(Arg.Any<string>())).Do(x => throw new Exception());

        // Act
        Func<Task> act = async () => await _sut.CreateAsync(template);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _sut.GetLastTemplateId().Should().Be(0);
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
        _individualSemaphores.Should().BeEmpty();
        _templateNameToIdMap.Should().BeEmpty();
        _templates.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_RestoresPreviousState_WhenDocumentIdManagerGetNextIdThrowsException()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _documentIdManager.When(x => x.InitializeForTemplateAsync(Arg.Any<int>())).Do(x => throw new Exception());

        // Act
        Func<Task> act = async () => await _sut.CreateAsync(template);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _sut.GetLastTemplateId().Should().Be(0);
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
        _individualSemaphores.Should().BeEmpty();
        _templateNameToIdMap.Should().BeEmpty();
        _templates.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_CreatesTemplate_WhenEverythingIsOk()
    {
        // Arrange
        var template = new Template { Name = "Template1" };

        // Act
        await _sut.CreateAsync(template);

        // Assert
        _sut.GetLastTemplateId().Should().Be(1);
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
        _individualSemaphores.Should().ContainKey(1);
        _templateNameToIdMap.Should().ContainKey(template.Name);
        _templates.Should().ContainKey(1);
    }

    [Fact]
    public void Get_ReturnsNull_WhenTemplateWithThatIdDoesNotExist()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _templates.TryAdd(1, template);

        // Act
        var result = _sut.Get(2);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Get_ReturnsTemplate_WhenTemplateWithThatIdExists()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _templates.TryAdd(1, template);

        // Act
        var result = _sut.Get(1);

        // Assert
        result.Should().Be(template);
    }

    [Fact]
    public void GetAll_ReturnsEmptyList_WhenThereAreNoTemplates()
    {
        // Arrange

        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAll_ReturnsAllTemplates_WhenThereAreTemplates()
    {
        // Arrange
        var template1 = new Template { Name = "Template1" };
        var template2 = new Template { Name = "Template2" };
        _templates.TryAdd(1, template1);
        _templates.TryAdd(2, template2);

        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().BeEquivalentTo(new List<Template> { template1, template2 });
    }

    [Fact]
    public Task DeleteAsync_ThrowsTemplateNotFoundException_WhenTemplateWithThatIdDoesNotExist()
    {
        // Arrange

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(2);

        // Assert
        return act.Should().ThrowAsync<TemplateNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ThrowsManagerInternalException_WhenTemplateNameToIdMapDoesNotContainKey()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _templates.TryAdd(1, template);

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<ManagerInternalException>();
    }

    [Fact]
    public async Task DeleteAsync_RestoresPreviousState_WhenRepositoryDeleteAsyncThrows()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _templates.TryAdd(1, template);
        _templateNameToIdMap.TryAdd(template.Name, 1);
        _sut.SetLastTemplateId(1);
        _individualSemaphores.TryAdd(1, new SemaphoreSlim(1, 1));
        _repository.When(x => x.DeleteAsync(Arg.Any<long>())).Do(x => throw new Exception());
        _repository.ExistsAsync(Arg.Any<long>()).Returns(ValueTask.FromResult(true));
        _documentIdManager.ExistsForTemplateAsync(Arg.Any<int>()).Returns(ValueTask.FromResult(true));
        _repositoryManager.Exists<Document>(Arg.Any<string>()).Returns(true);

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _sut.GetLastTemplateId().Should().Be(1);
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
        _individualSemaphores.Should().ContainKey(1);
        _individualSemaphores[1].CurrentCount.Should().Be(1);
        _templateNameToIdMap.Should().ContainKey(template.Name);
        _templates.Should().ContainKey(1);
        await _repository.Received(1).DeleteAsync(1);
        await _repository.Received(1).ExistsAsync(1);
        await _repository.Received(0).WriteAsync(1, Arg.Any<Template>());
        _repositoryManager.Received(0).Delete<Document>(Arg.Any<string>());
        _repositoryManager.Received(1).Exists<Document>(Arg.Any<string>());
        _repositoryManager.Received(0).Create<Document>(Arg.Any<string>());
        await _documentIdManager.Received(0).RemoveForTemplateAsync(1);
        await _documentIdManager.Received(1).ExistsForTemplateAsync(1);
        await _documentIdManager.Received(0).InitializeForTemplateAsync(1);
    }

    [Fact]
    public async Task DeleteAsync_RestoresPreviousState_WhenDocumentIdManagerRemoveForTemplateThrows()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _templates.TryAdd(1, template);
        _templateNameToIdMap.TryAdd(template.Name, 1);
        _sut.SetLastTemplateId(1);
        _individualSemaphores.TryAdd(1, new SemaphoreSlim(1, 1));
        _repository.ExistsAsync(Arg.Any<long>()).Returns(ValueTask.FromResult(false));
        _documentIdManager.When(x => x.RemoveForTemplateAsync(Arg.Any<int>())).Do(x => throw new Exception());
        _documentIdManager.ExistsForTemplateAsync(Arg.Any<int>()).Returns(ValueTask.FromResult(true));
        _repositoryManager.Exists<Document>(Arg.Any<string>()).Returns(true);

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _sut.GetLastTemplateId().Should().Be(1);
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
        _individualSemaphores.Should().ContainKey(1);
        _individualSemaphores[1].CurrentCount.Should().Be(1);
        _templateNameToIdMap.Should().ContainKey(template.Name);
        _templates.Should().ContainKey(1);
        await _repository.Received(1).DeleteAsync(1);
        await _repository.Received(1).ExistsAsync(1);
        await _repository.Received(1).WriteAsync(1, Arg.Any<Template>());
        _repositoryManager.Received(0).Delete<Document>(Arg.Any<string>());
        _repositoryManager.Received(1).Exists<Document>(Arg.Any<string>());
        _repositoryManager.Received(0).Create<Document>(Arg.Any<string>());
        await _documentIdManager.Received(1).RemoveForTemplateAsync(1);
        await _documentIdManager.Received(1).ExistsForTemplateAsync(1);
        await _documentIdManager.Received(0).InitializeForTemplateAsync(1);
    }

    [Fact]
    public Task DeleteAsync_DeletesTemplate_WhenEverythingIsOk()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _templates.TryAdd(1, template);
        _templateNameToIdMap.TryAdd(template.Name, 1);
        _individualSemaphores.TryAdd(1, new SemaphoreSlim(1, 1));

        // Act
        Func<Task> act = async () => await _sut.DeleteAsync(1);

        // Assert
        return act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task
        PerformActionOnLockedTemplateAsyncTArgs_ThrowsTemplateNotFoundException_WhenTemplateWithThatIdDoesNotExist()
    {
        // Arrange

        // Act
        Func<Task> act = async () =>
            await _sut.PerformActionOnLockedTemplateAsync<byte?>(2, (_, _, _) => ValueTask.CompletedTask, null);

        // Assert
        await act.Should().ThrowAsync<TemplateNotFoundException>();
    }

    [Fact]
    public async Task PerformActionOnLockedTemplateAsyncTArgs_LocksTemplate_WhenEverythingIsOk()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _templates.TryAdd(1, template);
        _templateNameToIdMap.TryAdd(template.Name, 1);
        _individualSemaphores.TryAdd(1, new SemaphoreSlim(1, 1));

        // Act
        bool isLocked = false;
        Func<Task> act = async () => await _sut.PerformActionOnLockedTemplateAsync<byte?>(1, (_, _, _) =>
        {
            if (_individualSemaphores.TryGetValue(1, out var semaphore))
            {
                isLocked = semaphore.CurrentCount == 0;
            }

            return ValueTask.CompletedTask;
        }, null);

        // Assert
        await act.Should().NotThrowAsync();
        isLocked.Should().BeTrue();
        _individualSemaphores[1].CurrentCount.Should().Be(1);
    }

    [Fact]
    public async Task
        PerformActionOnLockedTemplateAsyncTTArgs_ThrowsTemplateNotFoundException_WhenTemplateWithThatIdDoesNotExist()
    {
        // Arrange

        // Act
        Func<Task> act = async () =>
            await _sut.PerformActionOnLockedTemplateAsync<byte?, byte?>(2, (_, _, _) => ValueTask.FromResult((byte?)null), null);

        // Assert
        await act.Should().ThrowAsync<TemplateNotFoundException>();
    }

    [Fact]
    public async Task PerformActionOnLockedTemplateAsyncTTArgs_LocksTemplate_WhenEverythingIsOk()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _templates.TryAdd(1, template);
        _templateNameToIdMap.TryAdd(template.Name, 1);
        _individualSemaphores.TryAdd(1, new SemaphoreSlim(1, 1));

        // Act
        bool isLocked = false;
        Func<Task> act = async () => await _sut.PerformActionOnLockedTemplateAsync<byte?, byte?>(1, (_, _, _) =>
        {
            if (_individualSemaphores.TryGetValue(1, out var semaphore))
            {
                isLocked = semaphore.CurrentCount == 0;
            }

            return ValueTask.FromResult((byte?)null);
        }, null);

        // Assert
        await act.Should().NotThrowAsync();
        isLocked.Should().BeTrue();
        _individualSemaphores[1].CurrentCount.Should().Be(1);
    }

    [Fact]
    public async Task PerformActionOnLockedTemplateAsyncTTArgs_ReturnsResult_WhenEverythingIsOk()
    {
        // Arrange
        var template = new Template { Name = "Template1" };
        _templates.TryAdd(1, template);
        _templateNameToIdMap.TryAdd(template.Name, 1);
        _individualSemaphores.TryAdd(1, new SemaphoreSlim(1, 1));

        // Act
        var result =
            await _sut.PerformActionOnLockedTemplateAsync<byte?, byte?>(1, (_, _, _) => ValueTask.FromResult((byte?)10),
                null);

        // Assert
        result.Should().Be(10);
    }

    [Fact]
    public async Task
        PerformStartupActionAsync_ThrowsManagerInternalException_WhenTemplateIdsCountIsNotEqualToTemplatesCount()
    {
        // Arrange
        _documentIdManager.GetInitializedTemplatesAsync()
            .Returns(ValueTask.FromResult<IEnumerable<int>>(new List<int> { 1, 2 }));
        _repository.ReadAllAsync()
            .Returns(ValueTask.FromResult<IEnumerable<Template>>(new List<Template> { new Template { Id = 1 } }));

        // Act
        Func<Task> act = async () => await _sut.PerformStartupActionAsync();

        // Assert
        await act.Should().ThrowAsync<ManagerInternalException>();
    }

    [Fact]
    public async Task PerformStartupActionAsync_ThrowsManagerInternalException_WhenTemplateIdsAreNotUnique()
    {
        // Arrange
        _documentIdManager.GetInitializedTemplatesAsync()
            .Returns(ValueTask.FromResult<IEnumerable<int>>(new List<int> { 1, 1 }));
        _repository.ReadAllAsync()
            .Returns(ValueTask.FromResult<IEnumerable<Template>>(new List<Template> { new Template { Id = 1 } }));

        // Act
        Func<Task> act = async () => await _sut.PerformStartupActionAsync();

        // Assert
        await act.Should().ThrowAsync<ManagerInternalException>();
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
    }

    [Fact]
    public async Task PerformStartupActionAsync_ThrowsManagerInternalException_WhenTemplateIdIsAlreadyPresent()
    {
        // Arrange
        _documentIdManager.GetInitializedTemplatesAsync()
            .Returns(ValueTask.FromResult<IEnumerable<int>>(new List<int> { 1 }));
        _repository.ReadAllAsync()
            .Returns(ValueTask.FromResult<IEnumerable<Template>>(new List<Template> { new Template { Id = 1 } }));
        _templates.TryAdd(1, new Template { Id = 1 });

        // Act
        Func<Task> act = async () => await _sut.PerformStartupActionAsync();

        // Assert
        await act.Should().ThrowAsync<ManagerInternalException>();
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
    }

    [Fact]
    public async Task PerformStartupActionAsync_ThrowsManagerInternalException_WhenTemplateNamesAreNotUnique()
    {
        // Arrange
        _documentIdManager.GetInitializedTemplatesAsync()
            .Returns(ValueTask.FromResult<IEnumerable<int>>(new List<int> { 1, 2 }));
        _repository.ReadAllAsync().Returns(ValueTask.FromResult<IEnumerable<Template>>(new List<Template>
        {
            new Template { Id = 1, Name = "Template1" }, new Template { Id = 2, Name = "Template1" }
        }));

        // Act
        Func<Task> act = async () => await _sut.PerformStartupActionAsync();

        // Assert
        await act.Should().ThrowAsync<ManagerInternalException>();
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
    }

    [Fact]
    public async Task PerformStartupActionAsync_RunsSuccessfully_WhenEverythingIsOk()
    {
        // Arrange
        _documentIdManager.GetInitializedTemplatesAsync()
            .Returns(ValueTask.FromResult<IEnumerable<int>>(new List<int> { 1, 2 }));
        _repository.ReadAllAsync().Returns(ValueTask.FromResult<IEnumerable<Template>>(new List<Template>
        {
            new Template { Id = 1, Name = "Template1" }, new Template { Id = 2, Name = "Template2" }
        }));

        // Act
        Func<Task> act = async () => await _sut.PerformStartupActionAsync();

        // Assert
        await act.Should().NotThrowAsync();
        _generalTemplateSemaphore.CurrentCount.Should().Be(1);
        _templates.Count.Should().Be(2);
        _templateNameToIdMap.Count.Should().Be(2);
        _repositoryManager.Received(2).Create<Document>(Arg.Any<string>());
    }
}
