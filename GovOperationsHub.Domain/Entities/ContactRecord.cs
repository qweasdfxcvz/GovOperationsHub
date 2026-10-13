using GovOperationsHub.Domain.Common;
using GovOperationsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Domain.Entities
{
    public class ContactRecord : BaseAuditableEntity
    {
        public ContactNature Nature { get; set; } = ContactNature.Individual;

        // اسم الشخص (مثلاً: م. باسل درويش) أو مسمى القناة (مثلاً: ديوان الإدارة العامة والفاكس المركزي)
        public string NameOrChannelTitle { get; set; } = string.Empty;

        // الصفة الوظيفية (تُملأ في حال كان شخصاً: مدير عام، مدير المعلوماتية، مسؤول الربط)
        public string? JobTitle { get; set; }

        // القسم أو المديرية الفرعية (مثل: قسم الشبكات، دائرة المتابعة)
        public string? DepartmentName { get; set; }

        // قنوات الاتصال المتنوعة
        public string? LandlinePhone { get; set; }     // هاتف أرضي
        public string? MobileNumber { get; set; }      // رقم موبايل
        public string? FaxNumber { get; set; }         // فاكس
        public string? Email { get; set; }             // بريد إلكتروني
        public string? WhatsAppNumber { get; set; }    // واتساب
        public string? TelegramHandleOrNumber { get; set; } // تيليغرام

        public bool IsPrimary { get; set; } = false;   // هل هي جهة الاتصال الأساسية المعتمدة؟
        public string? Notes { get; set; }

        // الربط: يرتبط بوزارة، أو مؤسسة، أو مركز خدمة مواطن
        public int? MinistryId { get; set; }
        public Ministry? Ministry { get; set; }

        public int? GovernmentEntityId { get; set; }
        public GovernmentEntity? GovernmentEntity { get; set; }

        public int? CitizenServiceCenterId { get; set; }
        public CitizenServiceCenter? CitizenServiceCenter { get; set; }
    }
}
