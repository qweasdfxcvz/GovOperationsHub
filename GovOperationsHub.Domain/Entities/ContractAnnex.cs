using GovOperationsHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Domain.Entities
{
    public class ContractAnnex : BaseAuditableEntity
    {
        public int ContractId { get; set; }
        public Contract Contract { get; set; } = null!;

        public string AnnexNumber { get; set; } = string.Empty;
        public string ScopeDescription { get; set; } = string.Empty; // وصف ما يشمله الملحق

        // تفاصيل ما يحتويه هذا الملحق:
        public int? TargetCenterId { get; set; }
        public CitizenServiceCenter? TargetCenter { get; set; }

        public ICollection<GovernmentService> TargetServices { get; set; } = new List<GovernmentService>();

        // محرك التتبع واكتشاف التقصير المرتبط بهذا الملحق
        public ICollection<SlaStageTracker> ExecutionStages { get; set; } = new List<SlaStageTracker>();
    }
}
