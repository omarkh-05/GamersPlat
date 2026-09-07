using Microsoft.AspNetCore.Authorization;

namespace GamersPlatAPI.Authorization
{
    public class ClientOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}
