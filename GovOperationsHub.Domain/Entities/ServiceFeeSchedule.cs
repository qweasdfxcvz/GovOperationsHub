using GovOperationsHub.Domain.Common;
using GovOperationsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Domain.Entities
{
    public class ServiceFeeSchedule : BaseAuditableEntity
    {
        public int GovernmentServiceId { get; set; }
        public GovernmentService GovernmentService { get; set; } = null!;

        // نوع العملة وقيم الرسوم
        public CurrencyType Currency { get; set; } = CurrencyType.SYP;
        public decimal DirectFee { get; set; }        // رسم مباشر لصالح الجهة
        public decimal IndirectFee { get; set; }      // رسوم وطوابع غير مباشرة
        public decimal OurServiceMargin { get; set; } // بدل الخدمة لصالحنا (المنصة/الشركة)

        // الإجمالي المحسوب للرسم
        public decimal TotalCitizenFee => DirectFee + IndirectFee + OurServiceMargin;

        // تواريخ سريان هذا التعديل
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; } // Null يعني أن هذا التسعير هو النشط حالياً
        public bool IsActive { get; set; } = true;

        // التوثيق القانوني والرسمي لتعديل الرسوم
        public string? OfficialDecreeNumber { get; set; } // رقم القرار أو التعميم الوزاري بتعديل الرسم
        public DateTime? OfficialDecreeDate { get; set; }  // تاريخ القرار الوزاري
        public string? RevisionReason { get; set; }       // سبب التعديل (مثل: زيادة طابع الشهيد، تعديل بدل الخدمة)
    }
}
