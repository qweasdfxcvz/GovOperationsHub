using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GovOperationsHub.Domain.Enums
{
    public enum CurrencyType
    {
        [Display(Name = "ليرة سورية (SYP)")]
        SYP = 1,

        [Display(Name = "دولار أمريكي (USD)")]
        USD = 2
    }
}
