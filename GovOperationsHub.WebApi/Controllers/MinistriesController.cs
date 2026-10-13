using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Application.Features.Ministries.Commands.AddMinistryContact;
using GovOperationsHub.Application.Features.Ministries.Commands.CreateMinistry;
using GovOperationsHub.Application.Features.Ministries.Commands.DeleteMinistry;
using GovOperationsHub.Application.Features.Ministries.Queries.GetMinistries;
using Microsoft.AspNetCore.Mvc;

namespace GovOperationsHub.WebApi.Controllers
{
    public class MinistriesController : ApiControllerBase
    {
        /// <summary>
        /// استعراض قائمة الوزارات مع عدد جهات الاتصال لكل وزارة
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<List<MinistryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMinistries()
        {
            return Ok(await Mediator.Send(new GetMinistriesQuery()));
        }

        /// <summary>
        /// إضافة وزارة جديدة
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMinistry([FromBody] CreateMinistryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// حذف وزارة (حذف مرن Soft Delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMinistry(int id)
        {
            return Ok(await Mediator.Send(new DeleteMinistryCommand(id)));
        }

        /// <summary>
        /// إضافة جهة اتصال أو قناة رسمية جديدة لوزارة
        /// </summary>
        [HttpPost("{id:int}/contacts")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMinistryContact(int id, [FromBody] AddMinistryContactCommand command)
        {
            if (id != command.MinistryId)
            {
                return BadRequest(Result<int>.Failure("معرف الوزارة غير متطابق مع الرابط."));
            }

            return Ok(await Mediator.Send(command));
        }
    }
}
