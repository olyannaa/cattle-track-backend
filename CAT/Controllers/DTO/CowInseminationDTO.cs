namespace CAT.Controllers.DTO
{
    public class CowInseminationDTO
    {
        public Guid OrganizationId { get; set; }
        public Guid CowId { get; set; }
        public string? Status { get; set; }  
        public string? InseminationType { get; set; }  
        public DateTime? InseminationDate { get; set; }
        public Guid? BullId { get; set; }
    }
}
