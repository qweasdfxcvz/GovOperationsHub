using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GovOperationsHub.Domain.Enums
{
    public enum ServiceDeliveryChannel
    {
        [Display(Name = "المنصة الإلكترونية فقط")] DigitalPlatformOnly = 1,
        [Display(Name = "مركز خدمة المواطن فقط")] PhysicalCenterOnly = 2,
        [Display(Name = "مشترك (المنصة والمركز معاً)")] Hybrid = 3
    }
}
