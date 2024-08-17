using Api.DTOs;
using Api.Models;
using Api.Repositories;
using AutoMapper;

namespace Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IAgencyRepository agencyRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _agencyRepository = agencyRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponseDTO<UserResponseDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            /* await _agencyRepository.GetAllAsync(); */
            var userDtos = users.Select(u => UserResponseDTO.ValueOf(u)).ToList();

            return new PagedResponseDTO<UserResponseDTO>
            {
                Items = userDtos,
                Total = userDtos.Count
            };
        }

        public async Task<UserResponseDTO> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            /* await _agencyRepository.GetAllAsync(); */
            return UserResponseDTO.ValueOf(user);
        }

        public async Task<UserResponseDTO> CreateAsync(UserDTO userDTO)
        {
            var agency = await _agencyRepository.GetByIdAsync(userDTO.AgencyId);
            if (agency == null)
            {
                throw new Exception("Agency not found");
            }

            var user = new User
            {
                FullName = userDTO.FullName,
                Username = userDTO.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                Role = userDTO.Role,
                Agency = agency
            };

            await _userRepository.AddAsync(user);
            return UserResponseDTO.ValueOf(user);
        }

        public async Task<UserResponseDTO> UpdateAsync(int id, UserDTO userDTO)
        {
            var user = await _userRepository.GetByIdAsync(id);

            var agency = await _agencyRepository.GetByIdAsync(userDTO.AgencyId);
            if (agency == null)
            {
                throw new Exception("Agency not found");
            }

            _mapper.Map(userDTO, user);
            user.Agency = agency;

            await _userRepository.UpdateAsync(user);
            return UserResponseDTO.ValueOf(user);
        }

        public async Task DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task<UserResponseDTO> FindByUsernameAsync(string username)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            /* await _agencyRepository.GetAllAsync(); */
            return UserResponseDTO.ValueOf(user);
        }

        public async Task<UserResponseDTO> FindByUsernameAndPasswordAsync(string username, string password)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return UserResponseDTO.ValueOf(user);
            }

            return null;
        }
    }
}
