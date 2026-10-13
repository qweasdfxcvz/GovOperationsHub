using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

// تأكد أن مسار الـ namespace يطابق مكان الملف لديك
namespace GovOperationsHub.Application.Features.Users.Commands;

// 1. نمرر الـ Jti كمتغير من الـ Controller للحفاظ على المعمارية النظيفة
public record LogoutCommand(string Jti) : IRequest<Result<bool>>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public LogoutCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Jti))
        {
            // 2. البحث عن الجلسة باستخدام الـ Jti الممرر
            var currentSession = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.Jti == request.Jti && !s.IsRevoked, cancellationToken);

            if (currentSession != null)
            {
                // إبطال الجلسة نهائياً
                currentSession.IsRevoked = true;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return Result<bool>.Success(true, "تم تسجيل الخروج وتدمير الجلسة بنجاح.");
    }
}