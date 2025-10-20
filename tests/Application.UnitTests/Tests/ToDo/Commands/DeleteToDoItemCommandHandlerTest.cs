using Application.Common.Interfaces.Repositories;
using Application.Common.Result;
using Application.Features.ToDo.Commands.Delete;
using Moq;
using Xunit;

namespace Application.UnitTests.Tests.ToDo.Commands;

public class DeleteToDoCommandHandlerTest
{
    private readonly DeleteToDoCommandHandler _handler;
    private readonly Mock<IToDoRepository> _toDoItemRepositoryMock = new();

    public DeleteToDoCommandHandlerTest()
    {
        _handler = new DeleteToDoCommandHandler(_toDoItemRepositoryMock.Object);
    }

    #region ValidationTests

    [Fact]
    public void Validate_ShouldReturnError_WhenIdIsEmptyGuid()
    {
        // Arrange
        var query = new DeleteToDoCommand(Guid.Empty);
        
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
        var query = new DeleteToDoCommand(Guid.NewGuid());
        
        // Act
        (bool isValid, string? errorMessage) = query.Validate();
        
        // Assert
        Assert.True(isValid);
        Assert.Null(errorMessage);
    }

    #endregion

    #region HandlerTests
    [Fact]
    public async Task Handle_ShouldReturnErrorResult_WhenDeletionFails()
    {
        // Arrange
        var toDoItemId = Guid.NewGuid();
        var command = new DeleteToDoCommand(toDoItemId);
        _toDoItemRepositoryMock
            .Setup(repo => repo.DeleteAsync(toDoItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        
        // Act
        Result<bool> result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.False(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorMessage.NotFound, result.Error.Message);
        _toDoItemRepositoryMock
            .Verify(database => database.DeleteAsync(toDoItemId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenDeletionIsSuccessful()
    {
        // Arrange
        var toDoItemId = Guid.NewGuid();
        var command = new DeleteToDoCommand(toDoItemId);
        _toDoItemRepositoryMock
            .Setup(repo => repo.DeleteAsync(toDoItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // Act
        Result<bool> result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Null(result.Error);
        Assert.True(result.Data);
        _toDoItemRepositoryMock
            .Verify(database => database.DeleteAsync(toDoItemId, CancellationToken.None), Times.Once);
    }

    #endregion
}
