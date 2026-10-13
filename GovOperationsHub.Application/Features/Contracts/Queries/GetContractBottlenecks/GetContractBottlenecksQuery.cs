using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace GovOperationsHub.Application.Features.Contracts.Queries.GetContractBottlenecks
{
    public class ContractBottlenecksDto
    {
        public int ContractId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string MinistryName { get; set; } = string.Empty;
        public string? EntityName { get; set; }
        public int TotalDelayDays { get; set; }
        public bool HasBreachedStages { get; set; }
        public List<StageDelaySummaryDto> Stages { get; set; } = new();
    }

    public class StageDelaySummaryDto
    {
        public int StageId { get; set; }
        public int TrackNumber { get; set; }
        public string TrackName => TrackNumber switch
        {
            1 => "مسار الربط الشبكي والبنية التحتية",
            2 => "مسار أتمتة وتطوير المعاملات",
            3 => "مسار التسعير والبدلات المالية",
            _ => "مسار عام"
        };
        public string StageName { get; set; } = string.Empty;
        public string StatusArabic { get; set; } = string.Empty;
        public DateTime ExpectedCompletionDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }
        public int DelayDays { get; set; }
        public string ResponsiblePartyArabic { get; set; } = string.Empty;
        public string? OfficialCorrespondenceNumber { get; set; }
        public string? DelayReason { get; set; }
    }

    public record GetContractBottlenecksQuery(int ContractId) : IRequest<Result<ContractBottlenecksDto>>;

    public class GetContractBottlenecksQueryHandler : IRequestHandler<GetContractBottlenecksQuery, Result<ContractBottlenecksDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetContractBottlenecksQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ContractBottlenecksDto>> Handle(GetContractBottlenecksQuery request, CancellationToken cancellationToken)
        {
            var contract = await _context.Contracts
                .Include(c => c.Ministry)
                .Include(c => c.GovernmentEntity)
                .Include(c => c.Annexes)
                    .ThenInclude(a => a.ExecutionStages)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.ContractId, cancellationToken);

            if (contract == null)
            {
                return Result<ContractBottlenecksDto>.Failure("لم يتم العثور على العقد المطلوب.");
            }

            var stages = contract.Annexes
                .SelectMany(a => a.ExecutionStages)
                .Select(s => new StageDelaySummaryDto
                {
                    StageId = s.Id,
                    TrackNumber = s.TrackNumber,
                    StageName = s.StageName,
                    StatusArabic = GetDisplayName(s.Status),
                    ExpectedCompletionDate = s.ExpectedCompletionDate,
                    ActualCompletionDate = s.ActualCompletionDate,
                    DelayDays = s.CalculatedDelayDays,
                    ResponsiblePartyArabic = GetDisplayName(s.AssignedResponsibleParty),
                    OfficialCorrespondenceNumber = s.OfficialCorrespondenceNumber,
                    DelayReason = s.DelayReason
                })
                .ToList();

            var dto = new ContractBottlenecksDto
            {
                ContractId = contract.Id,
                ContractNumber = contract.ContractNumber,
                Title = contract.Title,
                MinistryName = contract.Ministry.Name,
                EntityName = contract.GovernmentEntity?.Name,
                TotalDelayDays = stages.Sum(s => s.DelayDays),
                HasBreachedStages = stages.Any(s => s.DelayDays > 0),
                Stages = stages
            };

            return Result<ContractBottlenecksDto>.Success(dto);
        }

        private static string GetDisplayName(Enum value)
        {
            return value.GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName() ?? value.ToString();
        }
    }
}
