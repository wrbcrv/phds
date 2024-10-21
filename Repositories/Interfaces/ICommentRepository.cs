using Api.Models;

namespace Api.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment> GetCommentByIdAsync(int commentId);
        Task AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int commentId);
    }
}
