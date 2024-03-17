using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESG_Survey_Automation.Infrastructure.Identity
{
    public interface IAppIdentity
    {
        Task CreateUser();
    }
}
