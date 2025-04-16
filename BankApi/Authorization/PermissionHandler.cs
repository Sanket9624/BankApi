using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using BankApi.Enums;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissions = context.User.FindAll("Permission").Select(p => p.Value);
        if (permissions.Contains(requirement.Permission.ToString()))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
