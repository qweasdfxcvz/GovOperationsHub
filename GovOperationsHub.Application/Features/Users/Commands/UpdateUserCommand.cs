using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GovOperationsHub.Application.Features.Users.Commands;

public record UpdateUserCommand(string UserId, string Email, string NewRole) : IRequest<Result<bool>>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<bool>>
{
    private readonly UserManager<IdentityUser> _userManager;

    public UpdateUserCommandHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return Result<bool>.Failure("المستخدم غير موجود.");

        // تحديث البريد الإلكتروني
        user.Email = request.Email;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result<bool>.Failure("فشل تحديث بيانات المستخدم.");

        // تحديث الصلاحية (Role)
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(request.NewRole))
        {
            // إزالة الصلاحيات القديمة وإضافة الجديدة
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.NewRole);
        }

        return Result<bool>.Success(true, "تم تحديث بيانات المستخدم بنجاح.");
    }
}