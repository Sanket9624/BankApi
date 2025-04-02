using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

namespace BankApi.Attributes
{
    public class PermissionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permission;

        public PermissionAuthorizeAttribute(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity is { IsAuthenticated: false })
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Retrieve the user's permissions from claims
            var userPermissions = user.Claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            if (!userPermissions.Contains(_permission))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
