using Domain.Common.Interfaces;

namespace Domain.Common.Models;

public class BaseAuditableEntity : IBaseAuditableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset? Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}
