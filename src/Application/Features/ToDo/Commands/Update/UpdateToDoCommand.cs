﻿using Application.Common.Interfaces.CQRS;
using Application.Common.Interfaces.Repositories;
using Application.Common.Result;
using Application.Dtos.ToDo.Response;
using Domain.Common.Constants;
using Mapster;

namespace Application.Features.ToDo.Commands.Update;

public record UpdateToDoCommand(Guid Id, string Title, int Priority, string? Note)
    : ICommand<ToDoResponse?>
{
    private const short MinTitleLength = DbConstraints.MinToDoNameLength;
    private const short MaxTitleLength = DbConstraints.MaxToDoNameLength;
    private const short MaxNoteLength = DbConstraints.MaxToDoNoteLength;

    public (bool IsValid, string? ErrorMessage) Validate()
    {
        if (Id == Guid.Empty)
        {
            return (false, ValidatorMessage.InvalidGuid);
        }
        
        switch (Title.Length)
        {
            case < MinTitleLength:
                return (false, ValidatorMessage.MinLength(nameof(Title), MinTitleLength));
            case > MaxTitleLength:
                return (false, ValidatorMessage.MaxLength(nameof(Title), MaxTitleLength));
        }
        
        if (Priority < 0)
        {
            return (false, ValidatorMessage.MinLength(nameof(Priority), 0));
        }

        if (Note is not null && Note.Length > MaxNoteLength)
        {
            return (false, ValidatorMessage.MaxLength(nameof(Note), MaxNoteLength));
        }

        return (true, null);
    }
}

internal sealed class UpdateToDoCommandHandler : ICommandHandler<UpdateToDoCommand, ToDoResponse?>
{
    private readonly IToDoRepository _toDoRepository;

    public UpdateToDoCommandHandler(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<Result<ToDoResponse?>> Handle(UpdateToDoCommand request, CancellationToken cancellationToken)
    {
        var entity = await _toDoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Result<ToDoResponse?>.Failure(ErrorMessage.NotFound);
        }
        
        entity.Title = request.Title;
        entity.Priority = request.Priority;
        entity.Note = request.Note ?? string.Empty;

        var result = await _toDoRepository.SaveChangesAsync(cancellationToken);
        if (result is 0) 
        {
            return Result<ToDoResponse?>.Failure(ErrorMessage.SomethingWentWrong);
        }

        var dto = entity.Adapt<ToDoResponse>();
        return Result<ToDoResponse?>.Success(dto);
    }
}
