using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.MyTasks.Queries
{
    public class PendingTaskDto
    {
        public int TaskId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string AnnexNumber { get; set; } = string.Empty;
        public string CenterName { get; set; } = string.Empty;
        public string StageName { get; set; } = string.Empty;
        public DateTime ExpectedCompletionDate { get; set; }
        public int TrackNumber { get; set; }

        // حساب الأيام المتبقية أو أيام التأخير ديناميكياً
        public int RemainingDays => (ExpectedCompletionDate.Date - DateTime.UtcNow.Date).Days;
    }

    // نمرر قائمة صلاحيات المستخدم من الـ Controller للحفاظ على المعمارية النظيفة
    public record GetMyPendingTasksQuery(List<string> UserRoles) : IRequest<Result<List<PendingTaskDto>>>;

    public class GetMyPendingTasksQueryHandler : IRequestHandler<GetMyPendingTasksQuery, Result<List<PendingTaskDto>>>
    {
        private readonly IApplicationDbContext _context;
        public GetMyPendingTasksQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<List<PendingTaskDto>>> Handle(GetMyPendingTasksQuery request, CancellationToken cancellationToken)
        {
            var allowedTracks = new List<int>();
            var roles = request.UserRoles.Select(r => r.Trim().ToLower()).ToList();

            if (roles.Contains("systemadmin") || roles.Contains("networkengineer")) allowedTracks.Add(1);
            if (roles.Contains("systemadmin") || roles.Contains("servicesofficer")) allowedTracks.Add(2);
            if (roles.Contains("systemadmin") || roles.Contains("finance")) allowedTracks.Add(3);

            if (!allowedTracks.Any())
                return Result<List<PendingTaskDto>>.Success(new List<PendingTaskDto>());

            var pendingTasks = await _context.SlaStageTrackers
                .Include(t => t.ContractAnnex).ThenInclude(a => a.Contract)
                .Include(t => t.ContractAnnex).ThenInclude(a => a.TargetCenter)
                .Where(t => t.Status == WorkflowStageStatus.InProgress && allowedTracks.Contains(t.TrackNumber))
                .OrderBy(t => t.ExpectedCompletionDate)
                .Select(t => new PendingTaskDto
                {
                    TaskId = t.Id,
                    // EF Core يقوم بمعالجة القيم الفارغة (nulls) هنا تلقائياً بدون تعقيد
                    ContractNumber = t.ContractAnnex.Contract.ContractNumber,
                    AnnexNumber = t.ContractAnnex.AnnexNumber,
                    CenterName = t.ContractAnnex.TargetCenter.Name,
                    StageName = t.StageName,
                    ExpectedCompletionDate = t.ExpectedCompletionDate,
                    TrackNumber = t.TrackNumber
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result<List<PendingTaskDto>>.Success(pendingTasks);
        }
    }
}
