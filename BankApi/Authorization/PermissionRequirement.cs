using Microsoft.AspNetCore.Authorization;
using BankApi.Enums;

public class PermissionRequirement : IAuthorizationRequirement
{
    public Permissions Permission { get; }

    public PermissionRequirement(Permissions permission)
    {
        Permission = permission;
    }
}
