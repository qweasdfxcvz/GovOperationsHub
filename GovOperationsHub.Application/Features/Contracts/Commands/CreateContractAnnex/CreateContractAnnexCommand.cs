using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Entities;
using GovOperationsHub.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.Contracts.Commands.CreateContractAnnex
{
    public record CreateContractAnnexCommand : IRequest<Result<int>>
    {
        public int ContractId { get; init; }
        public string AnnexNumber { get; init; } = string.Empty;
        public string ScopeDescription { get; init; } = string.Empty;

        // إضافة خيار: هل الملحق لمركز جديد أم موجود مسبقاً؟
        public bool IsNewCenter { get; init; } = true;
        public int? ExistingCenterId { get; init; }

        // بيانات المركز المستهدف (في حال كان جديداً)
        public string? CenterName { get; init; }
        public string? CenterCode { get; init; }
        public int? Governorate { get; init; }
        public string? City { get; init; }
        public string? DetailedAddress { get; init; }
    }

    public class CreateContractAnnexCommandValidator : AbstractValidator<CreateContractAnnexCommand>
    {
        public CreateContractAnnexCommandValidator()
        {
            RuleFor(v => v.ContractId).GreaterThan(0);
            RuleFor(v => v.AnnexNumber).NotEmpty().WithMessage("رقم الملحق مطلوب.");

            // التحقق الذكي بناءً على نوع الملحق
            When(v => v.IsNewCenter, () => {
                RuleFor(v => v.CenterName).NotEmpty().WithMessage("اسم المركز مطلوب.");
                RuleFor(v => v.CenterCode).NotEmpty().WithMessage("كود المركز مطلوب للشبكات.");
            });

            When(v => !v.IsNewCenter, () => {
                RuleFor(v => v.ExistingCenterId).NotNull().GreaterThan(0).WithMessage("يجب اختيار المركز الموجود مسبقاً.");
            });
        }
    }

    public class CreateContractAnnexCommandHandler : IRequestHandler<CreateContractAnnexCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public CreateContractAnnexCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(CreateContractAnnexCommand request, CancellationToken cancellationToken)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == request.ContractId, cancellationToken);

            if (contract == null) return Result<int>.Failure("العقد الأساسي غير موجود.");

            var annex = new ContractAnnex
            {
                ContractId = request.ContractId,
                AnnexNumber = request.AnnexNumber,
                ScopeDescription = request.ScopeDescription
            };

            if (request.IsNewCenter)
            {
                //  إنشاء مركز جديد
                var newCenter = new CitizenServiceCenter
                {
                    Name = request.CenterName ?? "",
                    CenterCode = request.CenterCode ?? "",
                    Governorate = (SyrianGovernorate)(request.Governorate ?? 1), // Casting
                    City = request.City ?? "",
                    DetailedAddress = request.DetailedAddress ?? "",
                    MinistryId = contract.MinistryId,
                    GovernmentEntityId = contract.GovernmentEntityId,
                    IsMainCenter = false                    
                };
                annex.TargetCenter = newCenter;
            }
            else
            {
                // ربط الملحق بمركز موجود مسبقاً
                annex.TargetCenterId = request.ExistingCenterId;
            }

            var now = DateTime.UtcNow;

            // توليد مسارات سير العمل (SLA Stages)

            // مسار الشبكات: نحتاجه *فقط* إذا كان المركز جديداً
            if (request.IsNewCenter)
            {
                annex.ExecutionStages.Add(new SlaStageTracker
                {
                    TrackNumber = 1,
                    StageName = "إعداد الربط الشبكي وإضافة البطاقة الشبكية",
                    ExpectedStartDate = now,
                    ExpectedCompletionDate = now.AddDays(7),
                    AllowedDurationInDays = 7,
                    Status = WorkflowStageStatus.InProgress,
                    AssignedResponsibleParty = ResponsibleParty.InternalNetworkTeam
                });
            }

            // مسار أتمتة الخدمات 
            annex.ExecutionStages.Add(new SlaStageTracker
            {
                TrackNumber = 2,
                StageName = "أتمتة واعتماد المعاملات والخدمات للملحق",
                ExpectedStartDate = now,
                ExpectedCompletionDate = now.AddDays(14),
                AllowedDurationInDays = 14,
                Status = WorkflowStageStatus.InProgress,
                AssignedResponsibleParty = ResponsibleParty.InternalDevTeam
            });

            // المسار المالي)
            annex.ExecutionStages.Add(new SlaStageTracker
            {
                TrackNumber = 3,
                StageName = "إضافة الحسابات البنكية ورسوم الخدمات",
                ExpectedStartDate = now.AddDays(14),
                ExpectedCompletionDate = now.AddDays(24),
                AllowedDurationInDays = 10,
                Status = WorkflowStageStatus.NotStarted,
                AssignedResponsibleParty = ResponsibleParty.InternalFinanceTeam
            });

            _context.ContractAnnexes.Add(annex);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(annex.Id, "تم إصدار الملحق وتوليد مهام الموظفين (SLA) بنجاح.");
        }
    }
}
