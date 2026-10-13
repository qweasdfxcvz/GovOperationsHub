using System.ComponentModel.DataAnnotations;
using System.Reflection;
using GovOperationsHub.Application.Common.Interfaces;
using GovOperationsHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GovOperationsHub.Application.Features.CitizenCenters.Queries
{
    // 1. نموذج عرض البيانات (DTO)
    public class CitizenCenterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CenterCode { get; set; } = string.Empty;
        public string GovernorateArabic { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        // التبعية (يأخذ اسم الهيئة وإلا اسم الوزارة)
        public string AffiliationName { get; set; } = string.Empty;

        // تصنيفات المركز (مجمعة في قائمة نصوص)
        public List<string> Classifications { get; set; } = new();

        // مؤشرات الشبكة
        public bool HasActiveNetworkConfig { get; set; }
        public string CurrentIpAddress { get; set; } = string.Empty;
    }

    // 2. كائن الطلب (Query)
    public record GetCitizenCentersQuery : IRequest<Result<List<CitizenCenterDto>>>;

    // 3. معالج الاستعلام (Handler)
    public class GetCitizenCentersQueryHandler : IRequestHandler<GetCitizenCentersQuery, Result<List<CitizenCenterDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetCitizenCentersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<CitizenCenterDto>>> Handle(GetCitizenCentersQuery request, CancellationToken cancellationToken)
        {
            var centers = await _context.CitizenServiceCenters
                .Include(c => c.Ministry)
                .Include(c => c.GovernmentEntity)
                .Include(c => c.NetworkConfigurations) // نحتاجها لمعرفة إعدادات الشبكة النشطة حالياً
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var dtos = centers.Select(c =>
            {
                // بناء تصنيفات المركز
                var classifications = new List<string>();
                if (c.IsMainCenter) classifications.Add("رئيسي");
                if (c.IsBranchCenter) classifications.Add("فرعي");
                if (c.IsDeliveryCenter) classifications.Add("مركز تسليم");
                if (c.IsSpecializedCenter) classifications.Add("تخصصي");

                // الحصول على الشبكة الفعّالة
                var activeNet = c.GetActiveNetworkConfiguration();

                return new CitizenCenterDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CenterCode = c.CenterCode,
                    GovernorateArabic = GetDisplayName(c.Governorate),
                    City = c.City,
                    AffiliationName = c.GovernmentEntity?.Name ?? c.Ministry?.Name ?? "غير محدد",
                    Classifications = classifications,
                    HasActiveNetworkConfig = activeNet != null,
                    CurrentIpAddress = activeNet?.IpAddress ?? "غير متصل"
                };
            }).ToList();

            return Result<List<CitizenCenterDto>>.Success(dtos);
        }

        // دالة مساعدة لجلب الاسم العربي للمحافظة
        private static string GetDisplayName(Enum value) =>
            value.GetType().GetMember(value.ToString()).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }
}
