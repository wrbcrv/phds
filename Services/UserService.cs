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
            return UserResponseDTO.ValueOf(user);
        }

        public async Task<UserResponseDTO> CreateAsync(UserDTO userDTO)
        {
            var agency = await _agencyRepository.GetByIdAsync(userDTO.AgencyId) ?? throw new Exception("Orgão não encontrado");

            var existingUser = await _userRepository.FindByUsernameAsync(userDTO.Username);

            if (existingUser != null)
            {
                throw new Exception("Nome de usuário já existe");
            }

            var user = new User
            {
                FullName = userDTO.FullName,
                Username = userDTO.Username,
                Email = userDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                Role = userDTO.Role,
                Agency = agency
            };

            await _userRepository.AddAsync(user);

            return UserResponseDTO.ValueOf(user);
        }

        public async Task<UserResponseDTO> UpdateAsync(int id, UserDTO userDTO)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new Exception("Usuário não encontrado");
            
            var agency = await _agencyRepository.GetByIdAsync(userDTO.AgencyId) ?? throw new Exception("Órgão não encontrado");

            if (user.Username != userDTO.Username)
            {
                var existingUser = await _userRepository.FindByUsernameAsync(userDTO.Username);
                if (existingUser != null)
                {
                    throw new Exception("Nome de usuário já existe");
                }
            }

            user.FullName = userDTO.FullName;
            user.Username = userDTO.Username;
            user.Email = userDTO.Email;
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
            user.Role = userDTO.Role;
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
            return UserResponseDTO.ValueOf(user);
        }

        public async Task<List<UserResponseDTO>> FindByFullNameAsync(string fullName)
        {
            var users = await _userRepository.FindByFullNameAsync(fullName);
            return users.Select(user => UserResponseDTO.ValueOf(user)).ToList();
        }

        public async Task<UserResponseDTO> FindByEmailAsync(string email)
        {
            var user = await _userRepository.FindByEmailAsync(email);
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
