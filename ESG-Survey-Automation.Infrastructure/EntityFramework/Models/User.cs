using System;
using System.Collections.Generic;

namespace ESG_Survey_Automation.Infrastructure.EntityFramework.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string EncryptedPassword { get; set; }

    public DateTime RegistrationDate { get; set; }

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
}
