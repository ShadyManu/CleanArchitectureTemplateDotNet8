using Application.Common.Interfaces.CQRS;
using Application.Common.Interfaces.Repositories;
using Application.Common.Result;
using Application.Dtos.ToDo.Response;
using Domain.Common.Constants;
using Domain.Entities;
using Mapster;

namespace Application.Features.ToDo.Commands.Create;

public record CreateToDoCommand(string Title, int Priority, string? Note = null, DateTime? Reminder = null)
    : ICommand<ToDoResponse?>
{
    private const short MinTitleLength = DbConstraints.MinToDoNameLength;
    private const short MaxTitleLength = DbConstraints.MaxToDoNameLength;
    
    public (bool IsValid, string? ErrorMessage) Validate()
    {
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
        
        return (true, null);
    }
}

internal sealed class CreateToDoCommandHandler : ICommandHandler<CreateToDoCommand, ToDoResponse?>
{
    private readonly IToDoRepository _toDoRepository;

    public CreateToDoCommandHandler(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<Result<ToDoResponse?>> Handle(CreateToDoCommand request, CancellationToken cancellationToken)
    {
        var entity = new ToDoEntity
        {
            Title = request.Title,
            Priority = request.Priority,
            Note = request.Note,
            Reminder = request.Reminder
        };

        await _toDoRepository.AddAsync(entity, cancellationToken);
        var result = await _toDoRepository.SaveChangesAsync(cancellationToken);
        if (result is 0)
        {
            return Result<ToDoResponse?>.Failure(ErrorMessage.SomethingWentWrong);
        }

        var dto = entity.Adapt<ToDoResponse>();
        return Result<ToDoResponse?>.Success(dto);
    }
}
