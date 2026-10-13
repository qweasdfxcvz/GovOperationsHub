using GovOperationsHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace GovOperationsHub.Domain.Entities
{
    public class Ministry : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // مثل: MOI, MOJ
        public string MinisterOrRepresentative { get; set; } = string.Empty;        
        public string Address { get; set; } = string.Empty;

        // الحساب البنكي الخاص بالوزارة
        public int? BankAccountId { get; set; }
        public BankAccount? BankAccount { get; set; }

        // العلاقات
        public ICollection<GovernmentEntity> GovernmentEntities { get; set; } = new List<GovernmentEntity>();
        public ICollection<CitizenServiceCenter> DirectCitizenCenters { get; set; } = new List<CitizenServiceCenter>();
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();

        // جهات وقنوات الاتصال المتعددة الخاصة بالوزارة
        public ICollection<ContactRecord> Contacts { get; set; } = new List<ContactRecord>();
    }
}
