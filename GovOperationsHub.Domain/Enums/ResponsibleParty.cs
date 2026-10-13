using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GovOperationsHub.Domain.Enums
{
    public enum ResponsibleParty
    {
        [Display(Name = "الفريق البرمجي الداخلي")] InternalDevTeam = 1,
        [Display(Name = "فريق الدعم الفني والربط الشبكي")] InternalNetworkTeam = 2,
        [Display(Name = "الإدارة المالية الداخلية")] InternalFinanceTeam = 3,
        [Display(Name = "الجهة الحكومية (الطرف الثاني)")] ExternalGovernmentClient = 4,
        [Display(Name = "مزود خدمة الإنترنت والاتصالات")] ExternalIsp = 5
    }
}
