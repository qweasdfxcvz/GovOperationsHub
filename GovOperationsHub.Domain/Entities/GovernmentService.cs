using GovOperationsHub.Domain.Common;
using GovOperationsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Domain.Entities
{
    public class GovernmentService : BaseAuditableEntity
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceCode { get; set; } = string.Empty;
        public ServiceCategory Category { get; set; }
        public ServiceDeliveryChannel DeliveryChannel { get; set; }        

        // هل الخدمة معتمدة وجاهزة للإنتاج؟
        public bool IsLiveOnProduction { get; set; } = false;
        public DateTime? ProductionGoLiveDate { get; set; }
        public string? Description { get; set; }

        // سجل الرسوم وتعديلاتها التاريخية عبر الزمن
        public ICollection<ServiceFeeSchedule> FeeSchedules { get; set; } = new List<ServiceFeeSchedule>();

        // دالة مساعدة سريعة لجلب التسعير الفعّال حالياً
        public ServiceFeeSchedule? GetCurrentActiveFee()
            => FeeSchedules.FirstOrDefault(f => f.IsActive && (f.EffectiveTo == null || f.EffectiveTo >= DateTime.UtcNow));
    }
}
