using Application.Common.Interfaces.Repositories;
using Application.Features.ToDo.Queries.GetAll;
using Application.UnitTests.Utilities;
using Domain.Entities;
using Moq;
using Xunit;

namespace Application.UnitTests.Tests.ToDo.Queries;

public class GetAllToDosQueryHandlerTest
{
    private readonly GetAllToDosQueryHandler _handler;
    private readonly Mock<IToDoRepository> _toDoItemRepositoryMock = new();
    
    public GetAllToDosQueryHandlerTest()
    {
        _handler = new GetAllToDosQueryHandler(_toDoItemRepositoryMock.Object);
    }

    #region HandlerTest
    
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsNoEntities()
    {
        // Arrange
        var query = new GetAllToDosQuery();
        _toDoItemRepositoryMock
            .Setup(repo => repo.GetAllOrderedByPriorityAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result.Error);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        _toDoItemRepositoryMock.Verify(repo => repo.GetAllOrderedByPriorityAsNoTrackingAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfToDoItemDtos_WhenRepositoryReturnsEntities()
    {
        // Arrange
        var ToDos = new List<ToDoEntity>
        {
            new() { Id = Guid.NewGuid(), Title = "Test Item 1", Priority = 1, Note = "Note 1" },
            new() { Id = Guid.NewGuid(), Title = "Test Item 2", Priority = 2, Note = "Note 2" }
        };
        var query = new GetAllToDosQuery();
        _toDoItemRepositoryMock
            .Setup(repo => repo.GetAllOrderedByPriorityAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ToDos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result.Error);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        TestUtilities.AssertCollectionsEqual(ToDos, result.Data);
        _toDoItemRepositoryMock.Verify(repo => repo.GetAllOrderedByPriorityAsNoTrackingAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
