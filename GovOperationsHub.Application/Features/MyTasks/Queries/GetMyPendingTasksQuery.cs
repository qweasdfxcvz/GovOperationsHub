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
            try
            {
                var allowedTracks = new List<int>();

                // 1. تنظيف الصلاحيات (إزالة المسافات الزائدة وتحويلها لأحرف صغيرة لضمان التطابق 100%)
                var roles = request.UserRoles.Select(r => r.Trim().ToLower()).ToList();

                if (roles.Contains("systemadmin") || roles.Contains("networkengineer")) allowedTracks.Add(1);
                if (roles.Contains("systemadmin") || roles.Contains("servicesofficer")) allowedTracks.Add(2);
                if (roles.Contains("systemadmin") || roles.Contains("finance")) allowedTracks.Add(3);

                // 2. إذا لم يتعرف على الصلاحية، سيرجع لك رسالة حمراء تخبرك بما قرأه بالضبط
                if (!allowedTracks.Any())
                {
                    return Result<List<PendingTaskDto>>.Failure($"تم الاتصال بنجاح. لكن أدوارك المقروءة: ({string.Join(" | ", request.UserRoles)}) لا تملك صلاحية لرؤية أي مسار عمل.");
                }

                var pendingTasks = await _context.SlaStageTrackers
                    .Include(t => t.ContractAnnex).ThenInclude(a => a.Contract)
                    .Include(t => t.ContractAnnex).ThenInclude(a => a.TargetCenter)
                    .Where(t => t.Status == WorkflowStageStatus.InProgress && allowedTracks.Contains(t.TrackNumber))
                    .OrderBy(t => t.ExpectedCompletionDate)
                    .Select(t => new PendingTaskDto
                    {
                        TaskId = t.Id,
                        ContractNumber = t.ContractAnnex != null && t.ContractAnnex.Contract != null ? t.ContractAnnex.Contract.ContractNumber : "غير متوفر",
                        AnnexNumber = t.ContractAnnex != null ? t.ContractAnnex.AnnexNumber : "غير متوفر",
                        CenterName = t.ContractAnnex != null && t.ContractAnnex.TargetCenter != null ? t.ContractAnnex.TargetCenter.Name : "غير متوفر",
                        StageName = t.StageName,
                        ExpectedCompletionDate = t.ExpectedCompletionDate,
                        TrackNumber = t.TrackNumber
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                // 3. الرادار الكاشف لقاعدة البيانات
                if (!pendingTasks.Any())
                {
                    var totalTasks = await _context.SlaStageTrackers.CountAsync(cancellationToken);
                    var inProgressTasks = await _context.SlaStageTrackers.CountAsync(t => t.Status == WorkflowStageStatus.InProgress, cancellationToken);

                    return Result<List<PendingTaskDto>>.Failure(
                        $"الأدوار صالحة ({string.Join(",", roles)})، لكن لا يوجد مهام لك. " +
                        $"إحصائيات الداتا بيز -> إجمالي المهام: {totalTasks} | المهام الفعالة عموماً: {inProgressTasks}");
                }

                return Result<List<PendingTaskDto>>.Success(pendingTasks);
            }
            catch (Exception ex)
            {
                return Result<List<PendingTaskDto>>.Failure($"خطأ في الداتا بيز: {ex.Message}");
            }
        }
    }
}
