using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GamersPlatAPI.Authorization
{
    // This authorization handler enforces the ownership rule for student resources.
    // It checks whether the current user is either:
    // - An Admin (full access), OR
    // - The owner of the student record being requested
    public class ClientOwnerOrAdminHandler : AuthorizationHandler<ClientOwnerOrAdminRequirement, int>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClientOwnerOrAdminRequirement requirement, int clientId)
        {
            // Admin override
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Ownership check
            /*
             يحوّل الـ claim NameIdentifier إلى رقم ويفحص إذا كان يطابق clientId المطلوب الوصول إليه.

                  هذا يضمن أن المستخدم العادي يستطيع الوصول فقط لبياناته  

                  إرجاع Task.CompletedTask:

هذا سليم لأنه ينهي handler سواء نجح أم لا.
             */
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userId, out int authenticatedclientId) && authenticatedclientId == clientId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    }
}
