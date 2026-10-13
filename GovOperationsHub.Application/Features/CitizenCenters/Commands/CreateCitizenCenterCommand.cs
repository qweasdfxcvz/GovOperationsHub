using FluentValidation;
using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Entities;
using GovOperationsHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.CitizenCenters.Commands
{
    // 1. كائن الطلب (Command)
    public record CreateCitizenCenterCommand : IRequest<Result<int>>
    {
        public string Name { get; init; } = string.Empty;
        public string CenterCode { get; init; } = string.Empty;
        public SyrianGovernorate Governorate { get; init; }
        public string City { get; init; } = string.Empty;
        public string DetailedAddress { get; init; } = string.Empty;

        public bool IsMainCenter { get; init; }
        public bool IsBranchCenter { get; init; }
        public bool IsDeliveryCenter { get; init; }
        public bool IsSpecializedCenter { get; init; }

        public int? MinistryId { get; init; }
        public int? GovernmentEntityId { get; init; }
    }

    // 2. التحقق من صحة البيانات (Validator)
    public class CreateCitizenCenterCommandValidator : AbstractValidator<CreateCitizenCenterCommand>
    {
        public CreateCitizenCenterCommandValidator(IApplicationDbContext context)
        {
            RuleFor(v => v.Name).NotEmpty().WithMessage("اسم المركز مطلوب.");

            RuleFor(v => v.CenterCode)
                .NotEmpty().WithMessage("الكود المعرف للمركز مطلوب.")
                .MustAsync(async (code, ct) => !await context.CitizenServiceCenters.AnyAsync(c => c.CenterCode == code, ct))
                .WithMessage("هذا الكود مستخدم لمركز آخر مسبقاً.");

            RuleFor(v => v)
                .Must(v => v.MinistryId.HasValue || v.GovernmentEntityId.HasValue)
                .WithMessage("يجب تحديد التبعية الإدارية للمركز (إما لوزارة أو لمؤسسة حكومية).");
        }
    }

    // 3. معالج الطلب (Handler)
    public class CreateCitizenCenterCommandHandler : IRequestHandler<CreateCitizenCenterCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public CreateCitizenCenterCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(CreateCitizenCenterCommand request, CancellationToken cancellationToken)
        {
            var center = new CitizenServiceCenter
            {
                Name = request.Name,
                CenterCode = request.CenterCode,
                Governorate = request.Governorate,
                City = request.City,
                DetailedAddress = request.DetailedAddress,
                IsMainCenter = request.IsMainCenter,
                IsBranchCenter = request.IsBranchCenter,
                IsDeliveryCenter = request.IsDeliveryCenter,
                IsSpecializedCenter = request.IsSpecializedCenter,
                MinistryId = request.MinistryId,
                GovernmentEntityId = request.GovernmentEntityId
            };

            _context.CitizenServiceCenters.Add(center);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(center.Id, "تم إضافة المركز بنجاح.");
        }
    }
}
