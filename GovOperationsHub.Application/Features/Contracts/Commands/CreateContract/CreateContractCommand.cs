using FluentValidation;
using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Entities;
using GovOperationsHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.Contracts.Commands.CreateContract
{
    // 1. كائن الطلب (Command)
    public record CreateContractCommand : IRequest<Result<int>>
    {
        public string ContractNumber { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public ContractType ContractType { get; init; }
        public DateTime SigningDate { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public decimal TotalBudget { get; init; }
        public string? Notes { get; init; }
        public int MinistryId { get; init; }
        public int? GovernmentEntityId { get; init; }
        //public string InitialAnnexNumber { get; init; } = "1/أ";
       // public string InitialAnnexScope { get; init; } = "الملحق التأسيسي لتجهيز الربط الشبكي وأتمتة الخدمات";
    }

    // 2. التحقق من صحة البيانات (Validator)
    public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
    {
        public CreateContractCommandValidator(IApplicationDbContext context)
        {
            RuleFor(v => v.ContractNumber)
                .NotEmpty().WithMessage("رقم العقد مطلوب ولا يمكن تركه فارغاً.")
                .MustAsync(async (number, ct) => !await context.Contracts.AnyAsync(c => c.ContractNumber == number, ct))
                .WithMessage("رقم العقد هذا مسجل مسبقاً في النظام.");

            RuleFor(v => v.Title).NotEmpty().WithMessage("موضوع العقد أو عنوانه مطلوب.");

            RuleFor(v => v.MinistryId)
                .GreaterThan(0).WithMessage("يجب تحديد الوزارة المعنية بالعقد.")
                .MustAsync(async (id, ct) => await context.Ministries.AnyAsync(m => m.Id == id, ct))
                .WithMessage("الوزارة المحددة غير مسجلة في قاعدة البيانات.");

            RuleFor(v => v.EndDate)
                .GreaterThan(v => v.StartDate).WithMessage("تاريخ نهاية العقد يجب أن يكون لاحقاً لتاريخ البدء.");
        }
    }

    // 3. المعالج الفعلي الذي يبحث عنه MediatR (Handler)
    public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public CreateContractCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(CreateContractCommand request, CancellationToken cancellationToken)
        {
            var contract = new Contract
            {
                ContractNumber = request.ContractNumber.Trim(),
                Title = request.Title.Trim(),
                ContractType = request.ContractType,
                SigningDate = request.SigningDate,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalBudget = request.TotalBudget,
                Notes = request.Notes,
                MinistryId = request.MinistryId,
                GovernmentEntityId = request.GovernmentEntityId
            };

            // إنشاء ملحق افتراضي وتوليد مراحل العمل لتتبع الـ SLA
           /* var annex = new ContractAnnex
            {
                AnnexNumber = request.InitialAnnexNumber,
                ScopeDescription = request.InitialAnnexScope
            };

            annex.ExecutionStages.Add(new SlaStageTracker
            {
                TrackNumber = 1,
                StageName = "إعداد الدراسة والمراسلات اللازمة للربط الشبكي",
                AllowedDurationInDays = 7,
                ExpectedStartDate = request.StartDate,
                ExpectedCompletionDate = request.StartDate.AddDays(7),
                Status = WorkflowStageStatus.InProgress,
                AssignedResponsibleParty = ResponsibleParty.InternalNetworkTeam
            });

            annex.ExecutionStages.Add(new SlaStageTracker
            {
                TrackNumber = 2,
                StageName = "دراسة متطلبات المعاملات الجديدة واعتمادها",
                AllowedDurationInDays = 14,
                ExpectedStartDate = request.StartDate,
                ExpectedCompletionDate = request.StartDate.AddDays(14),
                Status = WorkflowStageStatus.InProgress,
                AssignedResponsibleParty = ResponsibleParty.InternalDevTeam
            });

            annex.ExecutionStages.Add(new SlaStageTracker
            {
                TrackNumber = 3,
                StageName = "تحديد الرسوم والبدلات المالية والحسابات البنكية",
                AllowedDurationInDays = 10,
                ExpectedStartDate = request.StartDate.AddDays(14),
                ExpectedCompletionDate = request.StartDate.AddDays(24),
                Status = WorkflowStageStatus.NotStarted,
                AssignedResponsibleParty = ResponsibleParty.InternalFinanceTeam
            });*/

           //contract.Annexes.Add(annex);
            _context.Contracts.Add(contract);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(contract.Id, "تم إبرام العقد وتوليد مسارات التتبع والمراحل بنجاح.");
        }
    }
}
