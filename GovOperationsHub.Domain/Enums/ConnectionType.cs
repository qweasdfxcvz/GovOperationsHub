using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GovOperationsHub.Domain.Enums
{
    public enum ConnectionType
    {
        [Display(Name = "ألياف ضوئية (Fiber)")] Fiber = 1,
        [Display(Name = "ADSL عبر شبكة خاصة افتراضية (ADSL over VPN)")] AdslOverVpn = 2,
        [Display(Name = "SHDSL")] Shdsl = 3,
        [Display(Name = "خط شبكة محلي (Ethernet / Leased Line)")] Ethernet = 4
    }
}
