using GovOperationsHub.Domain.Common;
using GovOperationsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace GovOperationsHub.Domain.Entities
{
    public class GovernmentEntity : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public GovernmentEntityType EntityType { get; set; }
        public string DirectorName { get; set; } = string.Empty;        
        public string Address { get; set; } = string.Empty;

        // تتبع لوزارة ما
        public int MinistryId { get; set; }
        public Ministry Ministry { get; set; } = null!;

        // حساب بنكي خاص بالمؤسسة
        public int? BankAccountId { get; set; }
        public BankAccount? BankAccount { get; set; }

        // مراكز خدمة المواطن التابعة للمؤسسة
        public ICollection<CitizenServiceCenter> CitizenCenters { get; set; } = new List<CitizenServiceCenter>();
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();

        // جهات وقنوات الاتصال المتعددة الخاصة بالمؤسسة
        public ICollection<ContactRecord> Contacts { get; set; } = new List<ContactRecord>();
    }
}
