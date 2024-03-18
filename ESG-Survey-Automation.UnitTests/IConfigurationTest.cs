using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESG_Survey_Automation.UnitTests
{
    public static class IConfigurationTest
    {
        public static Dictionary<string, string> configValues = new Dictionary<string, string>
        {
            { "JwtOptions:SecurityKey", "7920B47B-12DD-48ED-A11C-1AACC17DEFA5-F24CC0AF-D863-41E5-B004-16A2FFDF277F" },
            { "JwtOptions:Issuer", "your-issuer" },
            { "JwtOptions:Audience", "your-audience" },
            { "AiRequestPath", "path" },
            { "GCPConfig:ProjectId", "your-project" },
            { "GCPConfig:StorageBucket", "your-bucket" },
            { "GCPConfig:SurveyQuestionair", "https://example.com/ai" }
        };
    }
}
