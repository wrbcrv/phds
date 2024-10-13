using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using AutoMapper;

namespace Api.Services
{
    public class AgencyService(IAgencyRepository agencyRepository, IMapper mapper) : IAgencyService
    {
        private readonly IAgencyRepository _agencyRepository = agencyRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResponseDTO<AgencyResponseDTO>> GetAllAsync()
        {
            var agencies = await _agencyRepository.GetAllAsync();
            var agencyDtos = agencies.Where(a => a.ParentId == null).Select(a => AgencyResponseDTO.ValueOf(a, agencies)).ToList();

            return new PagedResponseDTO<AgencyResponseDTO>
            {
                Items = agencyDtos,
                Total = agencyDtos.Count
            };
        }

        public async Task<AgencyResponseDTO> GetByIdAsync(int id)
        {
            var agencies = await _agencyRepository.GetAllAsync();
            var agency = agencies.FirstOrDefault(a => a.Id == id);
            return agency != null ? AgencyResponseDTO.ValueOf(agency, agencies) : null;
        }

        public async Task<AgencyResponseDTO> CreateAsync(AgencyDTO agencyDTO)
        {
            var parentAgency = agencyDTO.ParentId.HasValue ? await _agencyRepository.GetByIdAsync(agencyDTO.ParentId.Value) : null;

            var agency = new Agency
            {
                Name = agencyDTO.Name,
                Parent = parentAgency
            };

            await _agencyRepository.AddAsync(agency);

            var agencyResponseDTO = AgencyResponseDTO.ValueOf(agency);

            return agencyResponseDTO;
        }

        public async Task<AgencyResponseDTO> UpdateAsync(int id, AgencyDTO agencyDTO)
        {
            var agency = await _agencyRepository.GetByIdAsync(id);
            if (agency == null)
            {
                return null;
            }

            var parentAgency = agencyDTO.ParentId.HasValue ? await _agencyRepository.GetByIdAsync(agencyDTO.ParentId.Value) : null;

            agency.Name = agencyDTO.Name;
            agency.Parent = parentAgency;

            await _agencyRepository.UpdateAsync(agency);
            return _mapper.Map<AgencyResponseDTO>(agency);
        }

        public async Task DeleteAsync(int id)
        {
            await _agencyRepository.DeleteAsync(id);
        }
    }
}
