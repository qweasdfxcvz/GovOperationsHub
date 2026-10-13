using FluentValidation;
using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using GovOperationsHub.Domain.Entities;
using GovOperationsHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.NetworkConfigurations.Commands.UpdateNetworkConfiguration
{
    public record UpdateNetworkConfigurationCommand : IRequest<Result<int>>
    {
        public int CitizenServiceCenterId { get; init; }
        public NetworkChangeReason ChangeReason { get; init; }
        public string? ChangeDescription { get; init; }
        public string? OfficialLetterOrTicketNumber { get; init; }
        public DateTime? OfficialLetterDate { get; init; }
        public string IpAddress { get; init; } = string.Empty;
        public string SubnetMask { get; init; } = "255.255.255.0";
        public string Gateway { get; init; } = string.Empty;
        public int Port { get; init; }
        public ConnectionType ConnectionType { get; init; }
        public string RouterHardwareType { get; init; } = string.Empty;
        public string? RouterSerialNumber { get; init; } = string.Empty;
        public string VmSpecifications { get; init; } = string.Empty;
        public string DomainName { get; init; } = string.Empty;
        public string DedicatedCenterUrl { get; init; } = string.Empty;
        public bool IsJoinedToDomain { get; init; }
        public string DatabaseName { get; init; } = string.Empty;
        public string DatabaseUserName { get; init; } = string.Empty;
        public string DatabasePassword { get; init; } = string.Empty;
        public string? RouterCredentials { get; init; }
        public string? AdditionalTechnicalNotes { get; init; }
    }

    public class UpdateNetworkConfigurationCommandValidator : AbstractValidator<UpdateNetworkConfigurationCommand>
    {
        public UpdateNetworkConfigurationCommandValidator()
        {
            RuleFor(v => v.CitizenServiceCenterId)
                .GreaterThan(0).WithMessage("يجب تحديد مركز خدمة المواطن المعني بالتعديل.");

            RuleFor(v => v.IpAddress)
                .NotEmpty().WithMessage("عنوان الـ IP مطلوب.")
                .Matches(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$")
                .WithMessage("عنوان الـ IP المدخل غير صالح (Format IPv4 Invalid).");

            RuleFor(v => v.Gateway)
                .NotEmpty().WithMessage("البوابة الافتراضية (Gateway) مطلوبة.");

            RuleFor(v => v.RouterHardwareType)
                .NotEmpty().WithMessage("نوع وتجهيزة الراوتر مطلوبة لتتبع الأصول التقنية.");

            RuleFor(v => v.DatabasePassword)
                .NotEmpty().WithMessage("كلمة مرور قاعدة البيانات مطلوبة لحفظها مشفرة.");

            RuleFor(v => v.OfficialLetterOrTicketNumber)
                .NotEmpty().WithMessage("يجب إدخال رقم الكتاب الرسمي أو رقم التذكرة لتوثيق سبب التعديل.");
        }
    }

    public class UpdateNetworkConfigurationCommandHandler : IRequestHandler<UpdateNetworkConfigurationCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateNetworkConfigurationCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(UpdateNetworkConfigurationCommand request, CancellationToken cancellationToken)
        {
            var centerExists = await _context.CitizenServiceCenters
                .AnyAsync(c => c.Id == request.CitizenServiceCenterId, cancellationToken);

            if (!centerExists)
            {
                return Result<int>.Failure("مركز خدمة المواطن المحدد غير موجود في النظام.");
            }

            var activeConfig = await _context.NetworkConfigurations
                .Where(n => n.CitizenServiceCenterId == request.CitizenServiceCenterId && n.IsActive)
                .OrderByDescending(n => n.VersionNumber)
                .FirstOrDefaultAsync(cancellationToken);

            int nextVersion = 1;
            var now = DateTime.UtcNow;

            if (activeConfig != null)
            {
                activeConfig.IsActive = false;
                activeConfig.EffectiveTo = now;
                nextVersion = activeConfig.VersionNumber + 1;
            }

            var newConfig = new NetworkConfiguration
            {
                CitizenServiceCenterId = request.CitizenServiceCenterId,
                VersionNumber = nextVersion,
                IsActive = true,
                EffectiveFrom = now,
                ChangeReason = request.ChangeReason,
                ChangeDescription = request.ChangeDescription,
                OfficialLetterOrTicketNumber = request.OfficialLetterOrTicketNumber,
                OfficialLetterDate = request.OfficialLetterDate,
                IpAddress = request.IpAddress,
                SubnetMask = request.SubnetMask,
                Gateway = request.Gateway,
                Port = request.Port,
                ConnectionType = request.ConnectionType,
                RouterHardwareType = request.RouterHardwareType,
                RouterSerialNumber = request.RouterSerialNumber,
                VmSpecifications = request.VmSpecifications,
                DomainName = request.DomainName,
                DedicatedCenterUrl = request.DedicatedCenterUrl,
                IsJoinedToDomain = request.IsJoinedToDomain,
                DatabaseName = request.DatabaseName,
                DatabaseUserName = request.DatabaseUserName,
                EncryptedDatabasePassword = request.DatabasePassword,
                EncryptedRouterCredentials = request.RouterCredentials,
                AdditionalTechnicalNotes = request.AdditionalTechnicalNotes
            };

            _context.NetworkConfigurations.Add(newConfig);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(newConfig.Id, $"تم تحديث إعدادات الشبكة وتوليد الإصدار رقم ({nextVersion}) بنجاح.");
        }
    }
}
