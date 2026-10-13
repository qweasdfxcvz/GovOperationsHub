using FluentValidation;
using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.Ministries.Commands.CreateMinistry
{
    public record CreateMinistryCommand : IRequest<Result<int>>
    {
        public string Name { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public string MinisterOrRepresentative { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
    }

    public class CreateMinistryCommandValidator : AbstractValidator<CreateMinistryCommand>
    {
        public CreateMinistryCommandValidator(IApplicationDbContext context)
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("اسم الوزارة مطلوب.")
                .MaximumLength(200).WithMessage("اسم الوزارة يجب ألا يتجاوز 200 محرف.");

            RuleFor(v => v.Code)
                .NotEmpty().WithMessage("رمز الوزارة مطلوب (مثل MOI).")
                .MustAsync(async (code, ct) => !await context.Ministries.AnyAsync(m => m.Code == code, ct))
                .WithMessage("هذا الرمز مستخدم لوزارة أخرى.");
        }
    }

    public class CreateMinistryCommandHandler : IRequestHandler<CreateMinistryCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public CreateMinistryCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<int>> Handle(CreateMinistryCommand request, CancellationToken cancellationToken)
        {
            var ministry = new Ministry
            {
                Name = request.Name,
                Code = request.Code,
                MinisterOrRepresentative = request.MinisterOrRepresentative,
                Address = request.Address
            };

            _context.Ministries.Add(ministry);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(ministry.Id, "تم إضافة الوزارة بنجاح.");
        }
    }
}
