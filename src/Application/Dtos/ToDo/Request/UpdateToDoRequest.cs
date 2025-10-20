namespace Application.Dtos.ToDo.Request;

public class UpdateToDoRequest
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required int Priority { get; init; }
    public string? Note { get; init; }
    public DateTime? Reminder { get; init; }    
}
