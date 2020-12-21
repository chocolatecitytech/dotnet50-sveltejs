using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoticRentals.Api.Settings
{
    public class JwtOptions
    {
        public string Key { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int   Expiry { get; set; }
    }
}
