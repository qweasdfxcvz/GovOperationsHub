using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovOperationsHub.Domain.Enums
{
    public enum GovernmentEntityType
    {
        [Display(Name = "هيئة عامة")] Authority = 1,
        [Display(Name = "شركة عامة")] PublicCompany = 2,
        [Display(Name = "مديرية مستقلة")] IndependentDirectorate = 3,
        [Display(Name = "مؤسسة عامة")] PublicInstitution = 4
    }
}
