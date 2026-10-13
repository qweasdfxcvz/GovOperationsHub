using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GovOperationsHub.Application.Features.Users.Commands;

public record ResetUserPasswordCommand(string UserId, string NewPassword) : IRequest<Result<bool>>;

public class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, Result<bool>>
{
    private readonly UserManager<IdentityUser> _userManager;

    public ResetUserPasswordCommandHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return Result<bool>.Failure("المستخدم غير موجود.");

        // محرك Identity يتطلب توليد توكن أمان داخلي قبل تغيير كلمة المرور للموثوقية
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

        if (!result.Succeeded)
            return Result<bool>.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));

        return Result<bool>.Success(true, "تم إعادة تعيين كلمة المرور بنجاح.");
    }
}