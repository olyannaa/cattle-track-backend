using CAT.Controllers.DTO;
using CAT.EF;
using CAT.EF.DAL;

public static class IQueryableExstension
{
    public static IQueryable<InspectionActionDTO> SelectInspection(this IQueryable<DailyAction> dailyActions)
    {
        return dailyActions.Select(e => new InspectionActionDTO
        {
            Id = e.Id, 
            TagNumber = e.Animal!.TagNumber,
            PerformDate = e.Date,
            PerformedBy = e.Actor,
            Notes = e.Notes,
            Type = e.ActionSubtype,
            Result = e.Result,
            NextActionDate = e.NextDate
        });
    }

    public static IQueryable<TreatmentActionDTO> SelectTreatment(this IQueryable<DailyAction> dailyActions)
    {
        return dailyActions.Select(e => new TreatmentActionDTO
        {
            Id = e.Id,
            TagNumber = e.Animal!.TagNumber,
            PerformDate = e.Date,
            PerformedBy = e.Actor,
            Notes = e.Notes,
            Result = e.Result,
            Medicine = e.Medicine,
            Dose = e.Dose,
            NextActionDate = e.NextDate
        });
    }

    public static IQueryable<VaccinationActionDTO> SelectVaccination(this IQueryable<DailyAction> dailyActions)
    {
        return dailyActions.Select(e => new VaccinationActionDTO
        {
            Id = e.Id,
            TagNumber = e.Animal!.TagNumber,
            PerformDate = e.Date,
            PerformedBy = e.Actor,
            Notes = e.Notes,
            Type = e.ActionSubtype,
            Result = e.Result,
            NextActionDate = e.NextDate
        });
    }

    public static IQueryable<TransferActionDTO> SelectTransfer(this IQueryable<DailyAction> dailyActions)
    {
        return dailyActions.Select(e => new TransferActionDTO
        {
            Id = e.Id,
            TagNumber = e.Animal!.TagNumber,
            PerformDate = e.Date,
            PerformedBy = e.Actor,
            Notes = e.Notes,
            Type = e.ActionSubtype,
            OldGroupId = e.OldGroupId,
            OldGroupName = e.OldGroup!.Name,
            NewGroupId = e.NewGroupId,
            NewGroupName = e.NewGroup!.Name
        });
    }

    public static IQueryable<CullingActionDTO> SelectCulling(this IQueryable<DailyAction> dailyActions)
    {
        return dailyActions.Select(e => new CullingActionDTO
        {
            Id = e.Id,
            TagNumber = e.Animal!.TagNumber,
            PerformDate = e.Date,
            PerformedBy = e.Actor,
            Notes = e.Notes,
            Type = e.ActionSubtype,
        });
    }

    // public static IQueryable<CullingActionDTO> SelectResearch(this IQueryable<DailyAction> dailyActions)
    // {
    //     return dailyActions.Select(e => new ResearchActionDTO
    //     {
    //         Id = e.Id,
    //         TagNumber = e.Animal!.TagNumber,
    //         PerformDate = e.Date,
    //         PerformedBy = e.Actor,
    //         Notes = e.Notes,
    //         Type = e.ActionSubtype,
    //     });
    // }

    public static IQueryable<IdentificationActionDTO> SelectIdentification(this IQueryable<DailyAction> dailyActions)
    {
        return dailyActions.Select(e => new IdentificationActionDTO
        {
            Id = e.Id,
            TagNumber = e.Animal!.TagNumber,
            PerformDate = e.Date,
            PerformedBy = e.Actor,
            Notes = e.Notes
        });
    }
}