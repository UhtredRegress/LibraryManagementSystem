using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure;
using UserService.Domain.Model;

namespace UserService.Application.Queries;

public record AllRolesQuery:IRequest<IEnumerable<Role>>;

public class AllRolesQueryHandler(ApplicationDbContext dbContext): IRequestHandler<AllRolesQuery, IEnumerable<Role>>
{
    public async Task<IEnumerable<Role>> Handle(AllRolesQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Roles.ToListAsync(cancellationToken);
    }
}