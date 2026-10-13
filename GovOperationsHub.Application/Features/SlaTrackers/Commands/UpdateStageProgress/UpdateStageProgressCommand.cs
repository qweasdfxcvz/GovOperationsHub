using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.SlaTrackers.Commands.UpdateStageProgress
{
    public record UpdateStageProgressCommand : IRequest<Result<bool>>
    {
        public int StageId { get; init; }
        public WorkflowStageStatus Status { get; init; }
        public DateTime? ActualStartDate { get; init; }
        public DateTime? ActualCompletionDate { get; init; }
        public ResponsibleParty AssignedResponsibleParty { get; init; }
        public string? OfficialCorrespondenceNumber { get; init; }
        public DateTime? CorrespondenceDate { get; init; }
        public string? DelayReason { get; init; }
    }

    public class UpdateStageProgressCommandHandler : IRequestHandler<UpdateStageProgressCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateStageProgressCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(UpdateStageProgressCommand request, CancellationToken cancellationToken)
        {
            var stage = await _context.SlaStageTrackers
                .FirstOrDefaultAsync(s => s.Id == request.StageId, cancellationToken);

            if (stage == null)
            {
                return Result<bool>.Failure("المرحلة المحددة غير موجودة.");
            }

            stage.ActualStartDate = request.ActualStartDate ?? stage.ActualStartDate;
            stage.ActualCompletionDate = request.ActualCompletionDate;
            stage.AssignedResponsibleParty = request.AssignedResponsibleParty;
            stage.OfficialCorrespondenceNumber = request.OfficialCorrespondenceNumber;
            stage.CorrespondenceDate = request.CorrespondenceDate;
            stage.DelayReason = request.DelayReason;

            var checkDate = request.ActualCompletionDate ?? DateTime.UtcNow;

            if (request.Status == WorkflowStageStatus.CompletedOnTime || request.ActualCompletionDate.HasValue)
            {
                stage.Status = checkDate > stage.ExpectedCompletionDate
                    ? WorkflowStageStatus.CompletedDelayed
                    : WorkflowStageStatus.CompletedOnTime;
            }
            else
            {
                stage.Status = checkDate > stage.ExpectedCompletionDate
                    ? WorkflowStageStatus.Overdue
                    : request.Status;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "تم تحديث بيانات المرحلة ومؤشر المسؤولية بنجاح.");
        }
    }
}
