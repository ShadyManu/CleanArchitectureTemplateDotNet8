using Application.Common.Interfaces.Repositories;
using Application.Common.Result;
using Application.Features.ToDo.Commands.Create;
using Domain.Common.Constants;
using Domain.Entities;
using Moq;
using Xunit;

namespace Application.UnitTests.Tests.ToDo.Commands;

public class CreateToDoCommandHandlerTest
{
    private readonly CreateToDoCommandHandler _handler;
    private readonly Mock<IToDoRepository> _toDoItemRepositoryMock = new();

    public CreateToDoCommandHandlerTest()
    {
        _handler = new CreateToDoCommandHandler(_toDoItemRepositoryMock.Object);
    }

    #region ValidationTests
    
    [Fact]
    public void Validate_ShouldReturnError_WhenTitleIsTooShort()
    {
        // Arrange
        var shortTitle = new string('a', DbConstraints.MinToDoNameLength - 1); 
        var command = new CreateToDoCommand(shortTitle, 1);
        
        // Act
        (bool isValid, string? errorMessage) = command.Validate();
        
        // Assert
        Assert.False(isValid);
        Assert.Equal(
            ValidatorMessage.MinLength(
                nameof(CreateToDoCommand.Title), DbConstraints.MinToDoNameLength), errorMessage);
    }
    
    [Fact]
    public void Validate_ShouldReturnError_WhenTitleIsTooLong()
    {
        // Arrange
        var longTitle = new string('a', DbConstraints.MaxToDoNameLength + 1); 
        var command = new CreateToDoCommand(longTitle, 1);
        
        // Act
        (bool isValid, string? errorMessage) = command.Validate();
        
        // Assert
        Assert.False(isValid);
        Assert.Equal(
            ValidatorMessage.MaxLength(
                nameof(CreateToDoCommand.Title), DbConstraints.MaxToDoNameLength), errorMessage);
    }
    
    [Fact]
    public void Validate_ShouldReturnError_WhenPriorityIsNegative()
    {
        // Arrange
        var command = new CreateToDoCommand("Valid Title", -1);
        
        // Act
        (bool isValid, string? errorMessage) = command.Validate();
        
        // Assert
        Assert.False(isValid);
        Assert.Equal(
            ValidatorMessage.MinLength(
                nameof(CreateToDoCommand.Priority), 0), errorMessage);
    }
    
    [Fact]
    public void Validate_ShouldReturnValid_WhenTitleAndPriorityAreValid()
    {
        // Arrange
        var command = new CreateToDoCommand("Valid Title", 1);
        
        // Act
        (bool isValid, string? errorMessage) = command.Validate();
        
        // Assert
        Assert.True(isValid);
        Assert.Null(errorMessage);
    }

    #endregion

    #region HandlerTests

    [Fact]
    public async Task Handle_ShouldReturnErrorResult_WhenSaveChangesFails()
    {
        // Arrange
        var command = new CreateToDoCommand("Valid Title", 1);
        _toDoItemRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorMessage.SomethingWentWrong, result.Error.Message);
        _toDoItemRepositoryMock
            .Verify(repo => repo.AddAsync(It.Is<ToDoEntity>(e =>
                        e.Title == command.Title &&
                        e.Priority == command.Priority),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        _toDoItemRepositoryMock
            .Verify(repo => repo.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnCreatedResource_WhenToDoItemIsCreated()
    {
        // Arrange
        var command = new CreateToDoCommand("Valid Title", 1);
        _toDoItemRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result.Error);
        Assert.NotNull(result.Data);
        Assert.Equal(command.Title, result.Data.Title);
        Assert.Equal(command.Priority, result.Data.Priority);
        _toDoItemRepositoryMock
            .Verify(repo => repo.AddAsync(It.Is<ToDoEntity>(e =>
                        e.Title == command.Title &&
                        e.Priority == command.Priority),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        _toDoItemRepositoryMock
            .Verify(repo => repo.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    #endregion
}
