using Api.Models;

namespace Api.DTOs
{
    public class AgencyResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsTopLevel { get; set; }
        public List<AgencyResponseDTO> Children { get; set; } = [];

        public static AgencyResponseDTO ValueOf(Agency agency)
        {
            var dto = new AgencyResponseDTO
            {
                Id = agency.Id,
                Name = agency.Name,
                IsTopLevel = agency.IsTopLevel,
                Children = []
            };

            return dto;
        }

        public static AgencyResponseDTO ValueOf(Agency agency, IEnumerable<Agency> allAgencies)
        {
            var dto = new AgencyResponseDTO
            {
                Id = agency.Id,
                Name = agency.Name,
                IsTopLevel = agency.IsTopLevel,
                Children = allAgencies.Where(a => a.ParentId == agency.Id).Select(a => ValueOf(a, allAgencies)).ToList()
            };

            return dto;
        }
    }
}
