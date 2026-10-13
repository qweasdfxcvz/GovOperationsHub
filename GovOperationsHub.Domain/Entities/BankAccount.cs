using GovOperationsHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Domain.Entities
{
    public class BankAccount : BaseAuditableEntity
    {
        public string BankName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string? Iban { get; set; }
        public string AccountHolderName { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
