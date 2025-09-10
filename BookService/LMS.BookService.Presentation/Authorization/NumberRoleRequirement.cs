using Microsoft.AspNetCore.Authorization;

namespace LMS.BookService.Presentation.Authorization;

public class NumericRoleRequirement : IAuthorizationRequirement
{
    public int RoleRequirement { get; }
    public NumericRoleRequirement(int roleRequirement)
    {
        RoleRequirement = roleRequirement;
    }
}