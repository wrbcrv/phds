using System;
using Api.Models;

namespace Api.DTOs
{
    public class AgencyHierarchyDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public AgencyHierarchyDTO? Parent { get; set; }
        
        public static AgencyHierarchyDTO ValueOf(Agency agency)
        {
            return new AgencyHierarchyDTO
            {
                Id = agency.Id,
                Name = agency.Name,
                Parent = agency.Parent != null ? ValueOf(agency.Parent) : null
            };
        }
    }
}
