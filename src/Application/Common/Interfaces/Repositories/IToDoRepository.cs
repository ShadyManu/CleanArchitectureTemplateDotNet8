using Domain.Entities;

namespace Application.Common.Interfaces.Repositories;

public interface IToDoRepository : IBaseRepository<ToDoEntity>
{
    Task<List<ToDoEntity>> GetAllOrderedByPriorityAsNoTrackingAsync(CancellationToken cancellationToken);
}
