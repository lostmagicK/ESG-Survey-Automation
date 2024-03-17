using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESG_Survey_Automation.Domain
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
    }
}
