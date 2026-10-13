using GovOperationsHub.Domain.Common;
using GovOperationsHub.Domain.Enums;

namespace GovOperationsHub.Domain.Entities
{
    public class NetworkConfiguration : BaseAuditableEntity
    {
        public int CitizenServiceCenterId { get; set; }
        public CitizenServiceCenter CitizenServiceCenter { get; set; } = null!;

        // رقم النسخة وحالة التفعيل (Version 1, Version 2, ...)
        public int VersionNumber { get; set; } = 1;
        public bool IsActive { get; set; } = true;

        // تواريخ سريان هذه الإعدادات
        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
        public DateTime? EffectiveTo { get; set; } // Null يعني أن هذه الإعدادات هي المطبقة في المركز حالياً

        // سبب التغيير والتوثيق الرسمي
        public NetworkChangeReason ChangeReason { get; set; } = NetworkChangeReason.InitialSetup;
        public string? ChangeDescription { get; set; }           // شرح تفصيلي لسبب التعديل
        public string? OfficialLetterOrTicketNumber { get; set; } // رقم كتاب مديرية الاتصالات أو رقم تذكرة الدعم الفني
        public DateTime? OfficialLetterDate { get; set; }         // تاريخ الكتاب الرسمي

        // بيانات الشبكة والراوتر
        public string IpAddress { get; set; } = string.Empty;
        public string SubnetMask { get; set; } = "255.255.255.0";
        public string Gateway { get; set; } = string.Empty;
        public int Port { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public string RouterHardwareType { get; set; } = string.Empty; // مثل: Cisco 2911, MikroTik RB4011
        public string? RouterSerialNumber { get; set; }                 // الرقم التسلسلي للراوتر لتتبع الأصول

        // مواصفات الخادم الافتراضي والـ Domain
        public string VmSpecifications { get; set; } = string.Empty;   // eg: 4 vCPU, 16GB RAM, 200GB SSD
        public string DomainName { get; set; } = string.Empty;         // eg: csc.damascus.gov.sy
        public string DedicatedCenterUrl { get; set; } = string.Empty;   // الرابط المخصص لدخول النظام
        public bool IsJoinedToDomain { get; set; } = false;

        // بيانات قاعدة البيانات والحسابات (تُشفر آلياً في قاعدة البيانات)
        public string DatabaseName { get; set; } = string.Empty;
        public string DatabaseUserName { get; set; } = string.Empty;
        public string EncryptedDatabasePassword { get; set; } = string.Empty;
        public string? EncryptedRouterCredentials { get; set; }        // بيانات دخول الراوتر المشفرة
        public string? AdditionalTechnicalNotes { get; set; }
    }
}
