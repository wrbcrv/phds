using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using AutoMapper;

namespace Api.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IUserRepository userRepository, INotificationService notificationRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _notificationService = notificationRepository;
            _mapper = mapper;
        }

        public async Task<PagedResponseDTO<TicketResponseDTO>> GetAllAsync(int page, int size, TicketFilter filter = null)
        {
            var tickets = await _ticketRepository.GetAllAsync(page, size, filter);

            var ticketDtos = tickets.Items.Select(t => _mapper.Map<TicketResponseDTO>(t)).ToList();

            return new PagedResponseDTO<TicketResponseDTO>
            {
                Items = ticketDtos,
                Total = tickets.Total
            };
        }

        public async Task<TicketResponseDTO> GetByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            return ticket != null ? _mapper.Map<TicketResponseDTO>(ticket) : null;
        }

        public async Task<TicketResponseDTO> CreateAsync(TicketDTO ticketDTO)
        {
            var location = await _ticketRepository.GetAgencyByIdAsync(ticketDTO.LocationId);
            var customers = await _ticketRepository.GetUsersByIdsAsync(ticketDTO.CustomerIds);
            var assignees = await _ticketRepository.GetUsersByIdsAsync(ticketDTO.AssigneeIds);

            var ticket = new Ticket
            {
                Subject = ticketDTO.Subject,
                Description = ticketDTO.Description,
                Type = ticketDTO.Type,
                Status = ticketDTO.Status,
                Priority = ticketDTO.Priority,
                Location = location,
                Customers = customers,
                Assignees = assignees
            };

            await _ticketRepository.AddAsync(ticket);

            return _mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task<TicketResponseDTO> UpdateAsync(int id, TicketDTO ticketDTO)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                return null;
            }

            _mapper.Map(ticketDTO, ticket);
            await _ticketRepository.UpdateAsync(ticket);
            return _mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task DeleteAsync(int id)
        {
            await _ticketRepository.DeleteAsync(id);
        }

        public async Task<CommentResponseDTO> AddCommentAsync(int ticketId, int authorId, string content)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null) return null;

            var author = await _userRepository.GetByIdAsync(authorId);
            if (author == null) return null;

            var comment = new Comment
            {
                Content = content,
                Author = author,
                Ticket = ticket
            };

            await _ticketRepository.AddCommentAsync(comment);

            await _notificationService.Notify(ticket, author, content);

            return CommentResponseDTO.ValueOf(comment);
        }

        public async Task<CommentResponseDTO> UpdateCommentAsync(int commentId, string newContent, int currentUserId)
        {
            var comment = await _ticketRepository.GetCommentByIdAsync(commentId) ?? throw new KeyNotFoundException("Comentário não encontrado.");

            if (comment.AuthorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para editar este comentário.");
            }

            comment.Content = newContent;
            comment.UpdatedAt = DateTime.UtcNow;
            comment.IsUpdated = true;

            await _ticketRepository.UpdateCommentAsync(comment);

            return CommentResponseDTO.ValueOf(comment);
        }
    }
}
