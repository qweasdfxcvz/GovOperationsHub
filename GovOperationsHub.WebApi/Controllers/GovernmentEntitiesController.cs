using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Application.Features.GovernmentEntities.Commands;
using GovOperationsHub.Application.Features.GovernmentEntities.Queries;
using Microsoft.AspNetCore.Mvc;

namespace GovOperationsHub.WebApi.Controllers
{
    public class GovernmentEntitiesController : ApiControllerBase
    {
        /// <summary>
        /// استعراض قائمة المؤسسات والهيئات مع اسم الوزارة التابعة لها
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<List<GovernmentEntityDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGovernmentEntities()
        {
            return Ok(await Mediator.Send(new GetGovernmentEntitiesQuery()));
        }

        /// <summary>
        /// إضافة مؤسسة أو هيئة حكومية جديدة تابعة لوزارة
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateGovernmentEntity([FromBody] CreateGovernmentEntityCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// حذف مؤسسة (حذف مرن Soft Delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteGovernmentEntity(int id)
        {
            return Ok(await Mediator.Send(new DeleteGovernmentEntityCommand(id)));
        }

        /// <summary>
        /// إضافة جهة اتصال أو قناة رسمية جديدة لمؤسسة
        /// </summary>
        [HttpPost("{id:int}/contacts")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddGovernmentEntityContact(int id, [FromBody] AddGovernmentEntityContactCommand command)
        {
            if (id != command.GovernmentEntityId)
            {
                return BadRequest(Result<int>.Failure("معرف المؤسسة غير متطابق مع الرابط."));
            }

            return Ok(await Mediator.Send(command));
        }
    }
}
