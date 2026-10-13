using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GovOperationsHub.Application.Features.Users.Commands;

public record ToggleUserLockCommand(string UserId) : IRequest<Result<bool>>;

public class ToggleUserLockCommandHandler : IRequestHandler<ToggleUserLockCommand, Result<bool>>
{
    private readonly UserManager<IdentityUser> _userManager;

    public ToggleUserLockCommandHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(ToggleUserLockCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return Result<bool>.Failure("المستخدم غير موجود.");

        // التحقق من حالة الحظر الحالية
        bool isCurrentlyLocked = await _userManager.IsLockedOutAsync(user);

        if (isCurrentlyLocked)
        {
            // فك الحظر
            await _userManager.SetLockoutEndDateAsync(user, null);
            return Result<bool>.Success(true, "تم فك الحظر عن المستخدم بنجاح.");
        }
        else
        {
            // حظر لمدة طويلة (مثلاً 100 سنة لإيقافه نهائياً)
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            return Result<bool>.Success(true, "تم حظر المستخدم وإيقاف حسابه.");
        }
    }
}