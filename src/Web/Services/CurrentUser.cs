using System.Security.Claims;
using Application.Common.Interfaces.Auth;

namespace Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public Guid Id
    {
        get
        {
            var idString = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(idString, out var guid) ? guid : Guid.Empty;
        }
    }
}
