using System;
using System.Collections.Generic;

namespace ESG_Survey_Automation.Infrastructure.EntityFramework.Models;

public partial class Token
{
    public Guid Token1 { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public virtual User User { get; set; }
}
