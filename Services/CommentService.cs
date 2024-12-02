using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;

namespace Api.Services
{
    public class CommentService : ICommentService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public CommentService(
            ITicketRepository ticketRepository,
            ICommentRepository commentRepository,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _ticketRepository = ticketRepository;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<CommentResponseDTO>> GetCommentsByTicketIdAsync(int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException("Ticket não encontrado.");
            }

            var comments = await _commentRepository.GetCommentsByTicketIdAsync(ticketId);
            return comments.Select(CommentResponseDTO.ValueOf);
        }

        public async Task<CommentResponseDTO> AddCommentAsync(int ticketId, int authorId, string content, IList<IFormFile> files = null)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null) return null;

            var author = await _userRepository.GetByIdAsync(authorId);
            if (author == null) return null;

            var comment = new Comment
            {
                Content = content,
                Author = author,
                Ticket = ticket,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            ticket.UpdatedAt = DateTime.UtcNow;

            await _commentRepository.AddCommentAsync(comment);
            await _notificationService.Notify(ticket, author, content);

            return CommentResponseDTO.ValueOf(comment);
        }

        public async Task<CommentResponseDTO> UpdateCommentAsync(int ticketId, int commentId, string newContent)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Ticket não encontrado.");
            var comment = await _commentRepository.GetCommentByIdAsync(commentId) ?? throw new KeyNotFoundException("Comentário não encontrado."); // Usando CommentRepository

            if (comment.TicketId != ticket.Id)
            {
                throw new InvalidOperationException("Comentário não pertence ao ticket informado.");
            }

            comment.Content = newContent;
            comment.UpdatedAt = DateTime.UtcNow;
            comment.IsUpdated = true;

            ticket.UpdatedAt = DateTime.UtcNow;

            await _commentRepository.UpdateCommentAsync(comment);
            return CommentResponseDTO.ValueOf(comment);
        }

        public async Task DeleteCommentAsync(int ticketId, int commentId)
        {
            TimeSpan removalTimeLimit = TimeSpan.FromMinutes(5);
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Ticket não encontrado.");
            var comment = await _commentRepository.GetCommentByIdAsync(commentId) ?? throw new KeyNotFoundException("Comentário não encontrado."); // Usando CommentRepository

            if (comment.TicketId != ticket.Id)
            {
                throw new InvalidOperationException("Comentário não pertence ao ticket informado.");
            }

            var timeSinceCommentCreated = DateTime.UtcNow - comment.CreatedAt;
            if (timeSinceCommentCreated > removalTimeLimit)
            {
                throw new InvalidOperationException("O tempo permitido para remover este comentário expirou.");
            }

            ticket.UpdatedAt = DateTime.UtcNow;

            await _commentRepository.DeleteCommentAsync(commentId);
        }
    }
}
