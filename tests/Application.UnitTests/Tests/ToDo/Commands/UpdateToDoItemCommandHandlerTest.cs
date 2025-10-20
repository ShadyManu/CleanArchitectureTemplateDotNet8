using Application.Common.Interfaces.Repositories;
using Application.Common.Result;
using Application.Features.ToDo.Commands.Update;
using Application.UnitTests.Utilities;
using Domain.Common.Constants;
using Domain.Entities;
using Moq;
using Xunit;

namespace Application.UnitTests.Tests.ToDo.Commands;

public class UpdateToDoCommandHandlerTest
{
    private readonly UpdateToDoCommandHandler _handler;
    private readonly Mock<IToDoRepository> _toDoItemRepositoryMock = new();

    public UpdateToDoCommandHandlerTest()
    {
        _handler = new UpdateToDoCommandHandler(_toDoItemRepositoryMock.Object);
    }

    #region ValidationTests
    
    // invalid guid
    [Fact]
    public void Validate_ShouldReturnError_WhenIdIsEmptyGuid()
    {
        // Arrange
        var command = new UpdateToDoCommand(Guid.Empty, "Valid Title", 1, null);

        // Act
        (bool isValid, string? errorMessage) = command.Validate();

        // Assert
        Assert.False(isValid);
        Assert.Equal(ValidatorMessage.InvalidGuid, errorMessage);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenTitleIsTooShort()
    {
        // Arrange
        var shortTitle = new string('a', DbConstraints.MinToDoNameLength - 1);
        var command = new UpdateToDoCommand(Guid.NewGuid(), shortTitle, 1, null);

        // Act
        (bool isValid, string? errorMessage) = command.Validate();

        // Assert
        Assert.False(isValid);
        Assert.Equal(
            ValidatorMessage.MinLength(
                nameof(UpdateToDoCommand.Title), DbConstraints.MinToDoNameLength), errorMessage);
    }
    
    [Fact]
    public void Validate_ShouldReturnError_WhenTitleIsTooLong()
    {
        // Arrange
        var longTitle = new string('a', DbConstraints.MaxToDoNameLength + 1);
        var command = new UpdateToDoCommand(Guid.NewGuid(), longTitle, 1, null);
        
        // Act
        (bool isValid, string? errorMessage) = command.Validate();
        
        // Assert
        Assert.False(isValid);
        Assert.Equal(
            ValidatorMessage.MaxLength(
                nameof(UpdateToDoCommand.Title), DbConstraints.MaxToDoNameLength), errorMessage);
    }
    
    [Fact]
    public void Validate_ShouldReturnError_WhenPriorityIsNegative()
    {
        // Arrange
        var command = new UpdateToDoCommand(Guid.NewGuid(), "Valid Title", -1, null);
        
        // Act
        (bool isValid, string? errorMessage) = command.Validate();
        
        // Assert
        Assert.False(isValid);
        Assert.Equal(
            ValidatorMessage.MinLength(
                nameof(UpdateToDoCommand.Priority), 0), errorMessage);
    }
    
    [Fact]
    public void Validate_ShouldReturnError_WhenNoteIsTooLong()
    {
        // Arrange
        var longNote = new string('a', DbConstraints.MaxToDoNoteLength + 1);
        var command = new UpdateToDoCommand(Guid.NewGuid(), "Valid Title", 1, longNote);
        
        // Act
        (bool isValid, string? errorMessage) = command.Validate();
        
        // Assert
        Assert.False(isValid);
        Assert.Equal(
            ValidatorMessage.MaxLength(
                nameof(UpdateToDoCommand.Note), DbConstraints.MaxToDoNoteLength), errorMessage);
    }
    
    [Fact]
    public void Validate_ShouldReturnValid_WhenAllFieldsAreValid()
    {
        // Arrange
        var command = new UpdateToDoCommand(Guid.NewGuid(), "Valid Title", 1, "Valid Note");
        
        // Act
        (bool isValid, string? errorMessage) = command.Validate();
        
        // Assert
        Assert.True(isValid);
        Assert.Null(errorMessage);
    }

    #endregion

    #region HandlerTests

    [Fact]
    public async Task Handle_ShouldReturnErrorResult_WhenEntityNotFound()
    {
        // Arrange
        var command = new UpdateToDoCommand(Guid.NewGuid(), "Valid Title", 1, "Valid Note");
        _toDoItemRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ToDoEntity?)null);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorMessage.NotFound, result.Error.Message);
        _toDoItemRepositoryMock
            .Verify(repo => repo.GetByIdAsync(
                command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnErrorResult_WhenSaveChangesFails()
    {
        // Arrange
        var command = new UpdateToDoCommand(Guid.NewGuid(), "Valid Title", 1, "Valid Note");
        var existingEntity = new ToDoEntity
        {
            Id = command.Id,
            Title = "Old Title",
            Priority = 0,
            Note = "Old Note"
        };
        _toDoItemRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEntity);
        _toDoItemRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Null(result.Data);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorMessage.SomethingWentWrong, result.Error.Message);
        _toDoItemRepositoryMock
            .Verify(repo => repo.GetByIdAsync(
                command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _toDoItemRepositoryMock
            .Verify(repo => repo.SaveChangesAsync(
                It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnUpdatedResource_WhenToDoItemIsUpdated()
    {
        // Arrange
        var command = new UpdateToDoCommand(Guid.NewGuid(), "Updated Title", 2, "Updated Note");
        var existingEntity = new ToDoEntity
        {
            Id = command.Id,
            Title = "Old Title",
            Priority = 0,
            Note = "Old Note"
        };
        _toDoItemRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEntity);
        _toDoItemRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Null(result.Error);
        Assert.NotNull(result.Data);
        TestUtilities.AssertEntityMatchesDto(existingEntity, result.Data);
        _toDoItemRepositoryMock
            .Verify(repo => repo.GetByIdAsync(
                command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _toDoItemRepositoryMock
            .Verify(repo => repo.SaveChangesAsync(
                It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
