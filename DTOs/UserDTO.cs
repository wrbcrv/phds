using Api.Models;

namespace Api.DTOs
{
    public class UserDTO
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int AgencyId { get; set; }
        public Role Role { get; set; }
    }
}