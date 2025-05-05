namespace CAT.Controllers.DTO
{
    public class InsertCalvingDTO
    {
        public Guid CowId { get; set; }
        public string CowTagNumber { get; set; }
        public DateOnly Date { get; set; }
        public string Complication { get; set; }
        public string Type { get; set; }
        public string Veterinar { get; set; }
        public string Treatments { get; set; }
        public string Pathology { get; set; }
        public string CalfId { get; set; }
    }
}
