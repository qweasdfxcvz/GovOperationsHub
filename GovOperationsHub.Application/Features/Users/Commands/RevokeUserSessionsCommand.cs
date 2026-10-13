using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.Users.Commands
{
    public record RevokeUserSessionsCommand(string UserId) : IRequest<Result<bool>>;

    public class RevokeUserSessionsCommandHandler : IRequestHandler<RevokeUserSessionsCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public RevokeUserSessionsCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(RevokeUserSessionsCommand request, CancellationToken cancellationToken)
        {
            var activeSessions = await _context.UserSessions
                .Where(s => s.UserId == request.UserId && !s.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var session in activeSessions)
            {
                session.IsRevoked = true;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true, $"تم إبطال {activeSessions.Count} جلسة نشطة بنجاح.");
        }
    }
}
