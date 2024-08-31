using Api.DTOs;
using Api.Models;

namespace Api.Services
{
    public interface IUserService
    {
        Task<PagedResponseDTO<UserResponseDTO>> GetAllAsync();
        Task<UserResponseDTO> GetByIdAsync(int id);
        Task<UserResponseDTO> CreateAsync(UserDTO userDTO);
        Task<UserResponseDTO> UpdateAsync(int id, UserDTO userDTO);
        Task DeleteAsync(int id);
        Task<UserResponseDTO> FindByUsernameAsync(string username);
        Task<UserResponseDTO> FindByEmailAsync(string email);
        Task<UserResponseDTO> FindByUsernameAndPasswordAsync(string username, string password);
        Task<List<UserResponseDTO>> FindByFullNameAsync(string fullName);
    }
}