using GenericRepository;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.DeleteUserRole;
using ZenBlog.Application.Features.UserAttributeFeatures.UserRoleFeatures.Commands.GiveUserRole;
using ZenBlog.Application.Services.UserAttributeService;
using ZenBlog.Domain.Entities.UserEntities;
using ZenBlog.Domain.Repositories.UserRepositories;

namespace ZenBlog.Persistance.Services.UserServices
{
    public sealed class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserRoleService(IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork)
        {
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task GiveAsync(GiveUserRoleCommand request, CancellationToken cancellationToken)
        {
            bool roleAlreadyAssigned = await _userRoleRepository.AnyAsync(
                ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId,
                cancellationToken
            );

            if (roleAlreadyAssigned)
            {
                throw new InvalidOperationException("This role has already been assigned to this user.");
            }

            var userRole = new UserRole
            {
                UserId = request.UserId.ToString(),
                RoleId = request.RoleId.ToString(),
            };

            await _userRoleRepository.AddAsync(userRole, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(DeleteUserRoleFullCommand request, CancellationToken cancellationToken)
        {
            var userRoles = await _userRoleRepository
                .Where(ur => ur.UserId == request.UserId && request.RoleIds.Contains(ur.RoleId))
                .ToListAsync(cancellationToken);

            if (userRoles == null || userRoles.Count == 0)
            {
                throw new InvalidOperationException("No matching roles found for the given user.");
            }

            foreach (var userRole in userRoles)
            {
                _userRoleRepository.Delete(userRole);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<UserRole> GetAllUserRoles() => _userRoleRepository.GetAll().AsNoTracking();
    }
}
