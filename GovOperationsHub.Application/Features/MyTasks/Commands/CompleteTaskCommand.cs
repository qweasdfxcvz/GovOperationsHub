using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.MyTasks.Commands
{
    public record CompleteTaskCommand(int TaskId, string? Notes) : IRequest<Result<bool>>;

    public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        public CompleteTaskCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<bool>> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _context.SlaStageTrackers
                .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

            if (task == null) return Result<bool>.Failure("المهمة غير موجودة.");
            if (task.Status != WorkflowStageStatus.InProgress) return Result<bool>.Failure("لا يمكن إنهاء مهمة غير نشطة.");

            // 1. إنهاء المهمة الحالية
            var now = DateTime.UtcNow;
            task.ActualCompletionDate = DateTime.UtcNow;
            if (now.Date <= task.ExpectedCompletionDate.Date)
            {
                task.Status = WorkflowStageStatus.CompletedOnTime;
            }
            else
            {
                task.Status = WorkflowStageStatus.CompletedDelayed;
            }
            task.DelayReason = request.Notes; // حفظ أي ملاحظات

            // 2. تأثير الدومينو (Domino Effect):
            // إذا كانت هذه المهمة هي "أتمتة الخدمات" (المسار 2)، يجب تفعيل مهمة "المالية" (المسار 3)
            if (task.TrackNumber == 2)
            {
                var financeTask = await _context.SlaStageTrackers
                    .FirstOrDefaultAsync(t => t.ContractAnnexId == task.ContractAnnexId && t.TrackNumber == 3, cancellationToken);

                if (financeTask != null && financeTask.Status == WorkflowStageStatus.NotStarted)
                {
                    financeTask.Status = WorkflowStageStatus.InProgress;
                    // إعادة ضبط تواريخ البداية والنهاية للمالية لتبدأ من اليوم (إعطائهم مهلتهم الكاملة)
                    financeTask.ExpectedStartDate = DateTime.UtcNow;
                    financeTask.ExpectedCompletionDate = DateTime.UtcNow.AddDays(financeTask.AllowedDurationInDays);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true, "تم إنجاز المهمة بنجاح، وتم تحديث مسار العمل آلياً.");
        }
    }
}
