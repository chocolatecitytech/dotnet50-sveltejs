using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoticRentals.Api.Models
{
    public class AuthResult
    {
        public string Token { get; set; }
        public bool IsAuthenticated { get { return !string.IsNullOrWhiteSpace(Token); }  }
    }
}
