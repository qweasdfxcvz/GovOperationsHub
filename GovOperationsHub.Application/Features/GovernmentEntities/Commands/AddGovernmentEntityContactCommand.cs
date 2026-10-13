using FluentValidation;
using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Entities;
using GovOperationsHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.GovernmentEntities.Commands
{
    public record AddGovernmentEntityContactCommand : IRequest<Result<int>>
    {
        public int GovernmentEntityId { get; init; }
        public ContactNature Nature { get; init; }
        public string NameOrChannelTitle { get; init; } = string.Empty;
        public string? JobTitle { get; init; }
        public string? LandlinePhone { get; init; }
        public string? MobileNumber { get; init; }
        public string? Email { get; init; }
    }

    public class AddGovernmentEntityContactCommandValidator : AbstractValidator<AddGovernmentEntityContactCommand>
    {
        public AddGovernmentEntityContactCommandValidator()
        {
            RuleFor(v => v.GovernmentEntityId).GreaterThan(0).WithMessage("يجب تحديد المؤسسة.");
            RuleFor(v => v.NameOrChannelTitle).NotEmpty().WithMessage("اسم جهة الاتصال أو القناة مطلوب.");
        }
    }

    public class AddGovernmentEntityContactCommandHandler : IRequestHandler<AddGovernmentEntityContactCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        public AddGovernmentEntityContactCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<int>> Handle(AddGovernmentEntityContactCommand request, CancellationToken cancellationToken)
        {
            var entityExists = await _context.GovernmentEntities.AnyAsync(g => g.Id == request.GovernmentEntityId, cancellationToken);
            if (!entityExists) return Result<int>.Failure("المؤسسة غير موجودة.");

            var contact = new ContactRecord
            {
                GovernmentEntityId = request.GovernmentEntityId,
                Nature = request.Nature,
                NameOrChannelTitle = request.NameOrChannelTitle,
                JobTitle = request.JobTitle,
                LandlinePhone = request.LandlinePhone,
                MobileNumber = request.MobileNumber,
                Email = request.Email
            };

            _context.ContactRecords.Add(contact);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(contact.Id, "تم إضافة جهة الاتصال للمؤسسة بنجاح.");
        }
    }
}
