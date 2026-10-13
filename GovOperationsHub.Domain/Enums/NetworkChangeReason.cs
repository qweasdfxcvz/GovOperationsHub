using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GovOperationsHub.Domain.Enums
{
    public enum NetworkChangeReason
    {
        [Display(Name = "تهيئة أولية للمركز")]
        InitialSetup = 1,

        [Display(Name = "استبدال أو صيانة تجهيزات الراوتر")]
        HardwareReplacement = 2,

        [Display(Name = "ترقية وسيط الاتصال (مثلاً من ADSL إلى Fiber)")]
        MediumUpgrade = 3,

        [Display(Name = "تعديل عناوين الشبكة والـ IP من المزود")]
        IpReassignment = 4,

        [Display(Name = "ترقية مواصفات الخادم الافتراضي (VM)")]
        VmUpgrade = 5,

        [Display(Name = "تحديث دوري لكلمات المرور وتدابير أمنية")]
        SecurityCredentialsUpdate = 6,

        [Display(Name = "نقل المركز إلى موقع جغرافي جديد")]
        Relocation = 7,

        [Display(Name = "أسباب أخرى")]
        Other = 99
    }
}
