namespace CAT.Controllers.DTO
{
    public class InseminationDTO
    {
        public Guid CowId { get; set; }               
        public DateOnly Date { get; set; }
        public string InseminationType { get; set; }  
        public string? SpermBatch { get; set; }       
        public string? SpermManufacturer { get; set; }
        public Guid? BullId { get; set; }            
        public Guid? EmbryoId { get; set; }          
        public string? EmbryoManufacturer { get; set; } 
        public string? Technician { get; set; }      
        public string? Notes { get; set; }
        public DateOnly? ExpectedCalvingDate { get; set; }
    }
}
