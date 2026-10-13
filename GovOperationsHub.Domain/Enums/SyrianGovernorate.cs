using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GovOperationsHub.Domain.Enums
{
    public enum SyrianGovernorate
    {
        [Display(Name = "دمشق")]
        Damascus = 1,

        [Display(Name = "ريف دمشق")]
        RifDimashq = 2,

        [Display(Name = "حلب")]
        Aleppo = 3,

        [Display(Name = "حمص")]
        Homs = 4,

        [Display(Name = "حماة")]
        Hama = 5,

        [Display(Name = "اللاذقية")]
        Latakia = 6,

        [Display(Name = "طرطوس")]
        Tartus = 7,

        [Display(Name = "إدلب")]
        Idlib = 8,

        [Display(Name = "درعا")]
        Daraa = 9,

        [Display(Name = "السويداء")]
        AsSuwayda = 10,

        [Display(Name = "القنيطرة")]
        Quneitra = 11,

        [Display(Name = "دير الزور")]
        DeirEzZor = 12,

        [Display(Name = "الرقة")]
        Raqqa = 13,

        [Display(Name = "الحسكة")]
        AlHasakah = 14
    }
}
