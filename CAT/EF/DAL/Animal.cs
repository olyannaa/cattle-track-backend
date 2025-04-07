using System;
using System.Collections.Generic;

namespace CAT.EF.DAL;

public partial class Animal
{
    public Guid Id { get; set; }

    public Guid? OrganizationId { get; set; }

    public string TagNumber { get; set; } = null!;

    public string? Type { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Breed { get; set; }

    public Guid? MotherId { get; set; }

    public Guid? FatherId { get; set; }

    public string? Status { get; set; }

    public Guid? GroupId { get; set; }

    public string? Origin { get; set; }

    public string? OriginLocation { get; set; }

    public virtual ICollection<AnimalIdentification> AnimalIdentifications { get; set; } = new List<AnimalIdentification>();

    public virtual Group? Group { get; set; }

    public virtual Organization? Organization { get; set; }
}
