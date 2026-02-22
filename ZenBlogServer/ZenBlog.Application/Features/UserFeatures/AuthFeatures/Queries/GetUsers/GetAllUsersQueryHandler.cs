using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.Entities.UserEntities;

namespace ZenBlog.Application.Features.UserAttributeFeatures.AuthFeatures.Queries.GetUsers;

public sealed class GetAllUsersQueryHandler 
{
    private readonly IAuthService _authService;

    public GetAllUsersQueryHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<IQueryable<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        => Task.FromResult(_authService.GetAllUsers());
}
