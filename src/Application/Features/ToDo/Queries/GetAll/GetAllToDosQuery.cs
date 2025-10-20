using Application.Common.Interfaces.CQRS;
using Application.Common.Interfaces.Repositories;
using Application.Common.Result;
using Application.Dtos.ToDo.Response;
using Mapster;

namespace Application.Features.ToDo.Queries.GetAll;

public record GetAllToDosQuery : IQuery<List<ToDoResponse>>;

internal sealed class GetAllToDosQueryHandler : IQueryHandler<GetAllToDosQuery, List<ToDoResponse>>
{
    private readonly IToDoRepository _toDoRepository;

    public GetAllToDosQueryHandler(IToDoRepository toDoRepository)
    {
        _toDoRepository = toDoRepository;
    }

    public async Task<Result<List<ToDoResponse>>> Handle(GetAllToDosQuery request, CancellationToken cancellationToken)
    {
        var entities = await _toDoRepository.GetAllOrderedByPriorityAsNoTrackingAsync(cancellationToken);
        if (entities.Count is 0)
        {
            return Result<List<ToDoResponse>>.Success([]);
        }
        
        var dtos = entities.Adapt<List<ToDoResponse>>();
        return Result<List<ToDoResponse>>.Success(dtos);
    }
}
