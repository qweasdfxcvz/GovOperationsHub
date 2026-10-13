using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Application.Features.Contracts.Commands.CreateContract;
using GovOperationsHub.Application.Features.Contracts.Queries;
using GovOperationsHub.Application.Features.Contracts.Queries.GetContractBottlenecks;
using Microsoft.AspNetCore.Mvc;

namespace GovOperationsHub.WebApi.Controllers
{
    public class ContractsController : ApiControllerBase
    {
        /// <summary>
        /// استعراض قائمة العقود المبرمة
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<List<ContractListDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContracts()
        {
            return Ok(await Mediator.Send(new GetContractsQuery()));
        }

        /// <summary>
        /// إبرام عقد جديد وتوليد مسارات العمل الثلاثة ومراحل الـ SLA تلقائياً
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// استعلام مواطن الخلل والتأخير ومسؤولية التقصير لعقد محدد
        /// </summary>
        [HttpGet("{id:int}/bottlenecks")]
        [ProducesResponseType(typeof(Result<ContractBottlenecksDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContractBottlenecks(int id)
        {
            var result = await Mediator.Send(new GetContractBottlenecksQuery(id));
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }


        /// <summary>
        /// إضافة ملحق جديد ومركز خدمة لعقد موجود
        /// </summary>
        [HttpPost("{contractId}/annexes")]
        public async Task<IActionResult> AddAnnex(int contractId, [FromBody] GovOperationsHub.Application.Features.Contracts.Commands.CreateContractAnnex.CreateContractAnnexCommand command)
        {
            if (contractId != command.ContractId) return BadRequest("معرف العقد غير متطابق.");
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// جلب ملاحق عقد معين
        /// </summary>
        [HttpGet("{contractId}/annexes")]
        public async Task<IActionResult> GetAnnexes(int contractId)
        {
            // سنكتفي حالياً بجلبها عبر استعلام سريع، أو يمكنك لاحقاً بناء Query مخصص لها بـ MediatR
            // للتسريع الآن، سنمرر الاستعلام عبر MediatR بمجرد بناء الـ Query (يمكنك بناء GetContractAnnexesQuery مشابه لـ GetContracts)
            return Ok(); // مؤقتاً لحين بناء شاشة العرض
        }
    }
}

