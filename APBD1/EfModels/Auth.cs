using System;
using System.Collections.Generic;

namespace APBD1.EfModels
{
    public partial class Auth
    {
        public string IndexNumber { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Roles { get; set; }
        public string RefreshToken { get; set; }

        public virtual Students IndexNumberNavigation { get; set; }
    }
}
