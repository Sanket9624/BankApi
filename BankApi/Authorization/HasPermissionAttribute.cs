using Microsoft.AspNetCore.Authorization;
using BankApi.Enums;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permissions permission)
    {
        Policy = permission.ToString();
    }
}
