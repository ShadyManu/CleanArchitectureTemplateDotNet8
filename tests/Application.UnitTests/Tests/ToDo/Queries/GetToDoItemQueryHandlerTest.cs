using Application.Common.Interfaces.Repositories;
using Application.Common.Result;
using Application.Features.ToDo.Queries.Get;
using Domain.Entities;
using Moq;
using Xunit;

namespace Application.UnitTests.Tests.ToDo.Queries;

public class GetToDoQueryHandlerTest
{
    private readonly GetToDoQueryHandler _handler;
    private readonly Mock<IToDoRepository> _toDoItemRepositoryMock = new();

    public GetToDoQueryHandlerTest()
    {
        _handler = new GetToDoQueryHandler(_toDoItemRepositoryMock.Object);
    }

    #region ValidationTests

    [Fact]
    public void Validate_ShouldReturnError_WhenIdIsEmptyGuid()
    {
        // Arrange
        var query = new GetToDoQuery(Guid.Empty);
        
        // Act
        (bool isValid, string? errorMessage) = query.Validate();
        
        // Assert
        Assert.False(isValid);
        Assert.Equal(ValidatorMessage.InvalidGuid, errorMessage);
    }
    
    [Fact]
    public void Validate_ShouldReturnValid_WhenIdIsValidGuid()
    {
        // Arrange
        var query = new GetToDoQuery(Guid.NewGuid());
        
        // Act
        (bool isValid, string? errorMessage) = query.Validate();
        
        // Assert
        Assert.True(isValid);
        Assert.Null(errorMessage);
    }

    #endregion

    #region HandlerTests

    [Fact]
    public async Task Handle_ShouldReturnErrorResult_WhenToDoItemDoesNotExist()
    {
        // Arrange
        var toDoItemId = Guid.NewGuid();
        var query = new GetToDoQuery(toDoItemId);
        _toDoItemRepositoryMock
            .Setup(r => r.GetByIdAsNoTrackingAsync(
                toDoItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ToDoEntity?)null);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorMessage.NotFound, result.Error.Message);
        _toDoItemRepositoryMock
            .Verify(database => database.GetByIdAsNoTrackingAsync(
                toDoItemId, CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenToDoItemExists()
    {
        // Arrange
        var toDoItemId = Guid.NewGuid();
        var toDoItemEntity = new ToDoEntity
        {
            Id = toDoItemId,
            Title = "Test ToDo Item",
            Priority = 1
        };
        var query = new GetToDoQuery(toDoItemId);
        _toDoItemRepositoryMock
            .Setup(r => r.GetByIdAsNoTrackingAsync(
                toDoItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(toDoItemEntity);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Null(result.Error);
        Assert.NotNull(result.Data);
        Assert.Equal(toDoItemEntity.Id, result.Data!.Id);
        Assert.Equal(toDoItemEntity.Title, result.Data.Title);
        Assert.Equal(toDoItemEntity.Priority, result.Data.Priority);
        _toDoItemRepositoryMock
            .Verify(database => database.GetByIdAsNoTrackingAsync(
                toDoItemId, CancellationToken.None), Times.Once);
    }

    #endregion
}
