using FluentValidation;
using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Entities;
using GovOperationsHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.GovernmentEntities.Commands
{
    public record CreateGovernmentEntityCommand : IRequest<Result<int>>
    {
        public string Name { get; init; } = string.Empty;
        public GovernmentEntityType EntityType { get; init; }
        public string DirectorName { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public int MinistryId { get; init; }
    }

    public class CreateGovernmentEntityCommandValidator : AbstractValidator<CreateGovernmentEntityCommand>
    {
        public CreateGovernmentEntityCommandValidator(IApplicationDbContext context)
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("اسم الجهة/المؤسسة مطلوب.")
                .MaximumLength(200).WithMessage("الاسم يجب ألا يتجاوز 200 محرف.");

            RuleFor(v => v.MinistryId)
                .GreaterThan(0).WithMessage("يجب تحديد الوزارة التي تتبع لها هذه المؤسسة.")
                .MustAsync(async (id, ct) => await context.Ministries.AnyAsync(m => m.Id == id, ct))
                .WithMessage("الوزارة المحددة غير مسجلة في قاعدة البيانات.");
        }
    }

    public class CreateGovernmentEntityCommandHandler : IRequestHandler<CreateGovernmentEntityCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public CreateGovernmentEntityCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<int>> Handle(CreateGovernmentEntityCommand request, CancellationToken cancellationToken)
        {
            var entity = new GovernmentEntity
            {
                Name = request.Name,
                EntityType = request.EntityType,
                DirectorName = request.DirectorName,
                Address = request.Address,
                MinistryId = request.MinistryId
            };

            _context.GovernmentEntities.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(entity.Id, "تم إضافة المؤسسة/الجهة بنجاح.");
        }
    }
}
