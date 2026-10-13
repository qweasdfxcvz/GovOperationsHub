using GovOperationsHub.Domain.Common;
using GovOperationsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Domain.Entities
{
    public class CitizenServiceCenter : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string CenterCode { get; set; } = string.Empty; // كود مميز للمركز

        // الموقع الجغرافي
        public SyrianGovernorate Governorate { get; set; }
        public string City { get; set; } = string.Empty;
        public string DetailedAddress { get; set; } = string.Empty;
        public string? GpsCoordinates { get; set; } // خطوط الطول والعرض للخرائط

        // تصنيفات نوع المركز (وفق الشرط: يمكن أن يجتمع أكثر من تصنيف في آن واحد)
        public bool IsMainCenter { get; set; } = false;          // مركز رئيسي
        public bool IsBranchCenter { get; set; } = false;        // مركز فرعي
        public bool IsDeliveryCenter { get; set; } = false;      // مركز تسليم لمعاملات المنصة الإلكترونية
        public bool IsSpecializedCenter { get; set; } = false;   // مركز تخصصي

        // معلومات الاتصال بالمركز
        public ICollection<ContactRecord> Contacts { get; set; } = new List<ContactRecord>();

        // التبعية (يتبع لوزارة مباشرة أو لمؤسسة/هيئة)
        public int? MinistryId { get; set; }
        public Ministry? Ministry { get; set; }

        public int? GovernmentEntityId { get; set; }
        public GovernmentEntity? GovernmentEntity { get; set; }

        // الحساب البنكي (مستقل، أو فارغ إذا كان يتبع حساب المركز الرئيسي/المؤسسة)
        public int? BankAccountId { get; set; }
        public BankAccount? BankAccount { get; set; }

        // بيانات البنية التحتية والربط الشبكي
        // سجل البنية التحتية والربط الشبكي عبر التاريخ
        public ICollection<NetworkConfiguration> NetworkConfigurations { get; set; } = new List<NetworkConfiguration>();

        // دالة مساعدة لجلب إعدادات الشبكة النشطة حالياً في المركز
        public NetworkConfiguration? GetActiveNetworkConfiguration()
            => NetworkConfigurations.FirstOrDefault(nc => nc.IsActive && (nc.EffectiveTo == null || nc.EffectiveTo >= DateTime.UtcNow));        
    }
}