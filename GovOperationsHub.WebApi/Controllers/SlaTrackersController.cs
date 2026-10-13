using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Application.Features.SlaTrackers.Commands.UpdateStageProgress;
using Microsoft.AspNetCore.Mvc;

namespace GovOperationsHub.WebApi.Controllers
{
    public class SlaTrackersController : ApiControllerBase
    {
        /// <summary>
        /// تحديث إنجاز مرحلة من مسارات العمل وتحديد سبب وجهة التأخير
        /// </summary>
        [HttpPut("stages/{stageId:int}/progress")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStageProgress(int stageId, [FromBody] UpdateStageProgressCommand command)
        {
            if (stageId != command.StageId)
            {
                return BadRequest(Result<bool>.Failure("معرف المرحلة غير متطابق مع الرابط."));
            }

            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
