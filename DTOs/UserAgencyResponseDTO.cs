using Api.Models;

namespace Api.DTOs
{
    public class UserAgencyResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserAgencyResponseDTO Parent { get; set; }

        public static UserAgencyResponseDTO ValueOf(Agency agency, IEnumerable<Agency> allAgencies)
        {

            return new UserAgencyResponseDTO
            {
                Id = agency.Id,
                Name = agency.Name,
                Parent = agency.ParentId.HasValue ? ValueOf(allAgencies.First(a => a.Id == agency.ParentId), allAgencies) : null
            };
        }
    }
}
