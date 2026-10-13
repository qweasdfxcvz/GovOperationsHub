using FluentValidation;
using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Entities;
using GovOperationsHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.Ministries.Commands.AddMinistryContact
{
    public record AddMinistryContactCommand : IRequest<Result<int>>
    {
        public int MinistryId { get; init; }
        public ContactNature Nature { get; init; }
        public string NameOrChannelTitle { get; init; } = string.Empty;
        public string? JobTitle { get; init; }
        public string? LandlinePhone { get; init; }
        public string? MobileNumber { get; init; }
        public string? Email { get; init; }
    }

    public class AddMinistryContactCommandValidator : AbstractValidator<AddMinistryContactCommand>
    {
        public AddMinistryContactCommandValidator()
        {
            RuleFor(v => v.MinistryId).GreaterThan(0).WithMessage("يجب تحديد الوزارة.");
            RuleFor(v => v.NameOrChannelTitle).NotEmpty().WithMessage("اسم جهة الاتصال أو القناة مطلوب.");
        }
    }

    public class AddMinistryContactCommandHandler : IRequestHandler<AddMinistryContactCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public AddMinistryContactCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<int>> Handle(AddMinistryContactCommand request, CancellationToken cancellationToken)
        {
            var ministryExists = await _context.Ministries.AnyAsync(m => m.Id == request.MinistryId, cancellationToken);
            if (!ministryExists) return Result<int>.Failure("الوزارة غير موجودة.");

            var contact = new ContactRecord
            {
                MinistryId = request.MinistryId,
                Nature = request.Nature,
                NameOrChannelTitle = request.NameOrChannelTitle,
                JobTitle = request.JobTitle,
                LandlinePhone = request.LandlinePhone,
                MobileNumber = request.MobileNumber,
                Email = request.Email
            };

            _context.ContactRecords.Add(contact);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(contact.Id, "تم إضافة جهة الاتصال للوزارة بنجاح.");
        }
    }
}
