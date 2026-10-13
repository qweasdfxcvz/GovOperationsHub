using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.Users.Queries
{
    public record GetUsersQuery : IRequest<Result<List<UserDto>>>;
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public int ActiveSessionsCount { get; set; }
        public string LastIpAddress { get; set; } = "غير متوفر";

        public bool IsLocked { get; set; }
    }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<UserDto>>>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IApplicationDbContext _context;

        public GetUsersQueryHandler(UserManager<IdentityUser> userManager, IApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Result<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var activeSessions = await _context.UserSessions
                    .Where(s => s.UserId == user.Id && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync(cancellationToken);

                var isLocked = await _userManager.IsLockedOutAsync(user);

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = roles,
                    IsLocked = isLocked,
                    ActiveSessionsCount = activeSessions.Count,
                    LastIpAddress = activeSessions.FirstOrDefault()?.IpAddress ?? "غير متوفر"
                });
            }

            return Result<List<UserDto>>.Success(userDtos);
        }
    }
}
