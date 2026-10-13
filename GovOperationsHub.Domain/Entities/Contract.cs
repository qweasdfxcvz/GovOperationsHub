using GovOperationsHub.Domain.Common;
using GovOperationsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Domain.Entities
{
    public class Contract : BaseAuditableEntity
    {
        public string ContractNumber { get; set; } = string.Empty; // رقم العقد الرسمي
        public string Title { get; set; } = string.Empty;          // موضوع العقد
        public ContractType ContractType { get; set; }

        public DateTime SigningDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalBudget { get; set; }
        public string? Notes { get; set; }

        // الجهة الطرف الثاني في العقد
        public int MinistryId { get; set; }
        public Ministry Ministry { get; set; } = null!;

        public int? GovernmentEntityId { get; set; }
        public GovernmentEntity? GovernmentEntity { get; set; }

        // ملاحق العقد
        public ICollection<ContractAnnex> Annexes { get; set; } = new List<ContractAnnex>();
    }
}

