using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GovOperationsHub.Application.Features.Users.Commands
{
    public record CreateUserCommand(string Username, string Email, string Password, string Role) : IRequest<Result<string>>;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<string>>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public CreateUserCommandHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var exists = await _userManager.FindByNameAsync(request.Username);
            if (exists != null) return Result<string>.Failure("اسم المستخدم موجود مسبقاً.");

            var user = new IdentityUser { UserName = request.Username, Email = request.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return Result<string>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, request.Role);
            return Result<string>.Success(user.Id, "تم إنشاء المستخدم بنجاح.");
        }
    }
}
