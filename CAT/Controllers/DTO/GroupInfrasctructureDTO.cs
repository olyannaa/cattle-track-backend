﻿namespace CAT.Controllers.DTO
{
    public class GroupInfrasctructureDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? TypeId { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
    }
}
