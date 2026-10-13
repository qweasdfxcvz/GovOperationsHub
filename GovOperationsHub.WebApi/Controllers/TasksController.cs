using GovOperationsHub.Application.Features.MyTasks.Queries;
using GovOperationsHub.Application.Features.MyTasks.Commands;
using Microsoft.AspNetCore.Mvc;

namespace GovOperationsHub.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ApiControllerBase
    {
        [HttpGet("my-pending")]
        public async Task<IActionResult> GetMyPendingTasks([FromQuery] string roles)
        {
            var userRoles = string.IsNullOrEmpty(roles)
                ? new List<string>()
                : Uri.UnescapeDataString(roles).Split(',').ToList();

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