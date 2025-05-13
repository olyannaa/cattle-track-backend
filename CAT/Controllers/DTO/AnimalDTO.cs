
using CAT.EF.DAL;

namespace CAT.Controllers.DTO;
public class AnimalDTO
{
    public Guid Id { get; init; }

    public string TagNumber { get; init; } = null!;

    public DateOnly? BirthDate { get; init; }

    public string? Breed { get; init; }

    public string? GroupName { get; init; }

    public string? Status { get; init; }

    public string? Origin { get; init; }

    public string? OriginLocation { get; init; }

    public string? MotherTagNumber { get; init; }

    public string? FatherTagNumber { get; init; }

    public List<IdentificationFieldDTO>? IdentificationFields { get; init; }

    public static List<AnimalDTO> Parse(IEnumerable<IGrouping<Guid, AnimalCensus>> census)
    {
        return census.Select(g =>
                    {
                        var fields = g.Where(e => e.IdentificationFieldName != null)
                                        .Select(e => new IdentificationFieldDTO
                                        {
                                            IdentificationFieldName = e.IdentificationFieldName,
                                            IdentificationValue = e.IdentificationValue
                                        }).ToList();

                        var e = g.First();
                        return new AnimalDTO()
                        {
                            Id = e.Id,
                            TagNumber = e.TagNumber,
                            BirthDate = e.BirthDate,
                            Breed = e.Breed,
                            GroupName = e.GroupName,
                            Status = e.Status,
                            Origin = e.Origin,
                            OriginLocation = e.OriginLocation,
                            MotherTagNumber = e.MotherTagNumber,
                            FatherTagNumber = e.FatherTagNumber,
                            IdentificationFields = fields
                        };
                    })
                    .ToList();
    }
}

public class IdentificationFieldDTO
{
    public string? IdentificationFieldName { get; init; }
    public string? IdentificationValue { get; init; }
}