using Api.DTOs;

namespace Api.Services.Interfaces
{
    public interface IAgencyService
    {
        Task<PagedResponseDTO<AgencyResponseDTO>> GetAllAsync();
        Task<AgencyResponseDTO> GetByIdAsync(int id);
        Task<AgencyResponseDTO> CreateAsync(AgencyDTO agencyDTO);
        Task<AgencyResponseDTO> UpdateAsync(int id, AgencyDTO agencyDTO);
        Task DeleteAsync(int id);
    }
}
