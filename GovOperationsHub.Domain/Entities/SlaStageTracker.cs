using GovOperationsHub.Domain.Common;
using GovOperationsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Domain.Entities
{
    public class SlaStageTracker : BaseAuditableEntity
    {
        public int ContractAnnexId { get; set; }
        public ContractAnnex ContractAnnex { get; set; } = null!;

        // تحديد مسار العمل (1: شبكات وبنية تحتية، 2: برمجة وتطوير، 3: رسوم ومالية)
        public int TrackNumber { get; set; } // 1, 2, or 3
        public string StageName { get; set; } = string.Empty; // اسم المرحلة (مثل: انتظار موافقة دراسة المعاملة)

        // المواعيد المحددة والـ SLA
        public DateTime ExpectedStartDate { get; set; }
        public DateTime ExpectedCompletionDate { get; set; }
        public int AllowedDurationInDays { get; set; }

        // التواريخ الفعلية
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }

        // حالة المرحلة
        public WorkflowStageStatus Status { get; set; } = WorkflowStageStatus.NotStarted;

        // الجهة المسؤولة عن هذه المرحلة وعن أي تأخير يحدث فيها
        public ResponsibleParty AssignedResponsibleParty { get; set; }

        // التوثيق الرسمي ومراسلات إثبات المسؤولية
        public string? OfficialCorrespondenceNumber { get; set; } // رقم الكتاب الرسمي المرسل
        public DateTime? CorrespondenceDate { get; set; }          // تاريخ المراسلة
        public string? DelayReason { get; set; }                  // توثيق سبب التأخير عند حدوثه

        // خاصية محسوبة لمعرفة عدد أيام التأخير المتراكمة
        public int CalculatedDelayDays
        {
            get
            {
                var targetEnd = ActualCompletionDate ?? DateTime.UtcNow;
                if (targetEnd > ExpectedCompletionDate)
                {
                    return (int)(targetEnd - ExpectedCompletionDate).TotalDays;
                }
                return 0;
            }
        }
    }
}
