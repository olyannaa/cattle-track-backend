using System;
using System.Collections.Generic;

namespace CAT.EF.DAL;

public partial class Role
{
    public Guid Id { get; set; }

    public string Role1 { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
