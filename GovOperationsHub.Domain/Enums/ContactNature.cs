using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GovOperationsHub.Domain.Enums
{
    public enum ContactNature
    {
        [Display(Name = "شخص ذو صفة وظيفية")]
        Individual = 1,

        [Display(Name = "قناة اتصال رسمية عامة")]
        OfficialChannel = 2
    }
}
