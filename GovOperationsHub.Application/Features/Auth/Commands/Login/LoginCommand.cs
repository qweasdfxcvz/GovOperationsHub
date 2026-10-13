using FluentValidation;
using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GovOperationsHub.Application.Features.Auth.Commands.Login
{
    public record LoginCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string IpAddress { get; init; } = string.Empty;
        public string UserAgent { get; init; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(v => v.Username).NotEmpty().WithMessage("اسم المستخدم مطلوب.");
            RuleFor(v => v.Password).NotEmpty().WithMessage("كلمة المرور مطلوبة.");
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(UserManager<IdentityUser> userManager, IApplicationDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Result<AuthResponseDto>.Failure("اسم المستخدم أو كلمة المرور غير صحيحة.");
            }

            var activeSessions = await _context.UserSessions
                .Where(s => s.UserId == user.Id && !s.IsRevoked && s.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var session in activeSessions)
            {
                session.IsRevoked = true;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            // تم حل أخطاء CS0104 و CS1503 بكتابة المسار الصريح هنا
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));

            // المسار الصريح لتوليد التوكن
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _configuration["JwtSettings:ValidIssuer"],
                audience: _configuration["JwtSettings:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "60")),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            var newSession = new UserSession
            {
                UserId = user.Id,
                Jti = token.Id,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent
            };

            _context.UserSessions.Add(newSession);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                Roles = userRoles.ToList()
            }, "تم تسجيل الدخول بنجاح.");
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        
    }
}
