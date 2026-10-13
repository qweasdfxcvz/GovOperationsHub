using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Application.Features.Auth.Commands;
using GovOperationsHub.Application.Features.Users.Commands;
using GovOperationsHub.Application.Features.Auth.Commands.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace GovOperationsHub.WebApi.Controllers
{
    public class AuthController : ApiControllerBase
    {
        /// <summary>
        /// تسجيل دخول المستخدمين وتوليد JWT Token
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(Result<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            // التقاط عنوان الـ IP ونوع المتصفح/الجهاز أوتوماتيكياً من الطلب (لأغراض المراقبة الأمنية)
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var userAgent = Request.Headers.UserAgent.ToString();

            // بما أن الـ Command هو Record، نستخدم 'with' لنسخه وإضافة بيانات التتبع
            var finalCommand = command with
            {
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            return Ok(await Mediator.Send(finalCommand));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // 1. استخراج معرف الجلسة من التوكن الحالي في طبقة الويب
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(jti))
                return BadRequest("لا يمكن العثور على الجلسة الحالية.");

            // 2. تمرير المعرف إلى طبقة التطبيق بأمان
            var result = await Mediator.Send(new LogoutCommand(jti));
            return Ok(result);
        }
    }
}
