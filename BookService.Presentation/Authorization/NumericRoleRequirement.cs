using Microsoft.AspNetCore.Authorization;

namespace BookService.Presentation.Authorization;

public class NumericRoleRequirement : IAuthorizationRequirement
{
    public int RoleRequirement { get; }
    public NumericRoleRequirement(int roleRequirement)
    {
        RoleRequirement = roleRequirement;
    }
}