using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GovOperationsHub.Domain.Enums
{
    public enum ContractType
    {
        [Display(Name = "عقد بالتراضي عمومي")] ConsensualPublic = 1,
        [Display(Name = "اتفاقية تبادل بيانات (API)")] ApiDataExchange = 2
    }
}
