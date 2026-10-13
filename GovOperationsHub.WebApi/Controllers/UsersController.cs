using GovOperationsHub.Application.Features.Users.Commands;
using GovOperationsHub.Application.Features.Users.Queries;
using GovOperationsHub.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovOperationsHub.WebApi.Controllers
{
    [Authorize(Roles = AppRoles.SystemAdmin)] // حماية إجبارية: للمدراء فقط
    public class UsersController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await Mediator.Send(new GetUsersQuery()));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("{id}/revoke-sessions")]
        public async Task<IActionResult> RevokeSessions(string id)
        {
            return Ok(await Mediator.Send(new RevokeUserSessionsCommand(id)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.UserId) return BadRequest("معرف المستخدم غير متطابق.");
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("{id}/toggle-lock")]
        public async Task<IActionResult> ToggleUserLock(string id)
        {
            return Ok(await Mediator.Send(new ToggleUserLockCommand(id)));
        }

        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetUserPasswordCommand command)
        {
            if (id != command.UserId) return BadRequest("معرف المستخدم غير متطابق.");
            return Ok(await Mediator.Send(command));
        }
    }
}
