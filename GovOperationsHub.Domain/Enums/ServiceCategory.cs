using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GovOperationsHub.Domain.Enums
{
    public enum ServiceCategory
    {
        [Display(Name = "معاملات الإدارة المحلية والبلديات")] Municipal = 1,
        [Display(Name = "معاملات المصالح العقارية")] RealEstate = 2,
        [Display(Name = "معاملات وزارة العدل")] Judicial = 3,
        [Display(Name = "معاملات الشؤون المدنية")] CivilAffairs = 4,
        [Display(Name = "معاملات الهجرة والجوازات")] ImmigrationAndPassports = 5,
        [Display(Name = "خدمات الدفع والتحصيل الإلكتروني")] ElectronicPayment = 6,
        [Display(Name = "معاملات أخرى")] Other = 99
    }
}
