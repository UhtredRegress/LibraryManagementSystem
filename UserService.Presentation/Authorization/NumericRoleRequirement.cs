using Microsoft.AspNetCore.Authorization;

namespace UserService.Presentation.Authorization;

public class NumericRoleRequirement : IAuthorizationRequirement
{
    public int NumberRequirement { get; }

    public NumericRoleRequirement(int numberRequirement)
    {
        NumberRequirement = numberRequirement;
    }
}