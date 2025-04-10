namespace CAT.EF.DAL
{
    public class Reproduction
    {
        public Guid Id { get; set; }
        public Guid? AnimalId { get; set; }
        public Guid? BullId { get; set; }
        public DateTime? InseminationDate { get; set; }
        public string? Type { get; set; }
        public string? SpermBatch { get; set; }
        public string? SpermManufacturer { get; set; }
        public string? Technician { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public DateTime? CheckDate { get; set; }
        public string? CowCondition { get; set; }
        public DateTime? ExpectedCalvingDate { get; set; }
        public DateTime? CalvingDate { get; set; }
        public string? CalvingType { get; set; }
        public string? Complications { get; set; }
        public string? Veterinarian { get; set; }
        public string? Treatments { get; set; }
        public string? Pathology { get; set; }


        public virtual Animal? Animal { get; set; }
        public virtual Animal? Bull { get; set; }
    }
}
