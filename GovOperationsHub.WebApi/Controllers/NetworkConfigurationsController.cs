using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Application.Features.NetworkConfigurations.Commands.UpdateNetworkConfiguration;
using Microsoft.AspNetCore.Mvc;

namespace GovOperationsHub.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NetworkConfigurationsController : ApiControllerBase
    {
        /// <summary>
        /// تسجيل ترقية أو تعديل في بيانات الشبكة لمركز خدمة مواطن وإصدار نسخة جديدة
        /// </summary>
        [HttpPost("update")]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateConfiguration([FromBody] UpdateNetworkConfigurationCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
