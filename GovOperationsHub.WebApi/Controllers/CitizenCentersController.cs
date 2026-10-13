using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Application.Features.CitizenCenters.Commands;
using GovOperationsHub.Application.Features.CitizenCenters.Queries;

using Microsoft.AspNetCore.Mvc;

namespace GovOperationsHub.WebApi.Controllers
{
    public class CitizenCentersController : ApiControllerBase
    {
        /// <summary>
        /// استعراض قائمة مراكز خدمة المواطن مع تصنيفاتها وحالة الربط الشبكي
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<List<CitizenCenterDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCitizenCenters()
        {
            return Ok(await Mediator.Send(new GetCitizenCentersQuery()));
        }

        /// <summary>
        /// إضافة مركز خدمة مواطن جديد (رئيسي أو فرعي)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCitizenCenter([FromBody] CreateCitizenCenterCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
