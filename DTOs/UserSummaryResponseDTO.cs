using Api.Models;

namespace Api.DTOs
{
    public class UserSummaryResponseDTO

    {
        public int Id { get; set; }
        public string? FullName { get; set; }

        public static UserSummaryResponseDTO ValueOf(User user)
        {
            return new UserSummaryResponseDTO

            {
                Id = user.Id,
                FullName = user.FullName,
            };
        }
    }
}