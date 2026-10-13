using GovOperationsHub.Application.Features.MyTasks.Queries;
using GovOperationsHub.Application.Features.MyTasks.Commands;
using Microsoft.AspNetCore.Mvc;

namespace GovOperationsHub.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // 🚨 أزلنا [Authorize] لأن HttpClient الداخلي لا يرسل الـ Cookie
    public class TasksController : ApiControllerBase
    {
        [HttpGet("my-pending")]
        public async Task<IActionResult> GetMyPendingTasks()
        {
            var rolesHeader = Request.Headers["X-User-Roles"].ToString();

            var decodedRoles = string.Empty;
            if (!string.IsNullOrEmpty(rolesHeader))
            {
                // 🚀 فك التشفير (Base64) لاسترجاع الحروف العربية كما كانت
                var bytes = Convert.FromBase64String(rolesHeader);
                decodedRoles = System.Text.Encoding.UTF8.GetString(bytes);
            }

            var userRoles = string.IsNullOrEmpty(decodedRoles)
                ? new List<string>()
                : decodedRoles.Split(',').ToList();

            var query = new GetMyPendingTasksQuery(userRoles);
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteTask(int id, [FromBody] CompleteTaskCommand command)
        {
            if (id != command.TaskId) return BadRequest("معرف المهمة غير متطابق.");
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}