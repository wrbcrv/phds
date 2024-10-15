using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using AutoMapper;

namespace Api.Services
{
    public class TicketService(ITicketRepository ticketRepository, IUserRepository userRepository, INotificationService notificationRepository, IMapper mapper) : ITicketService
    {
        private readonly ITicketRepository _ticketRepository = ticketRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly INotificationService _notificationService = notificationRepository;
        private readonly IMapper _mapper = mapper;

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

        public async Task<TicketResponseDTO> AssignCurrentUserAsync(int ticketId, int userId, bool asAssignee)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");

            var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("Usuário não encontrado.");

            if (asAssignee)
            {
                if (!ticket.Assignees.Contains(user))
                {
                    ticket.Assignees.Add(user);
                }
                else
                {
                    throw new InvalidOperationException("Usuário já está atribuído como Assignee.");
                }
            }
            else
            {
                if (!ticket.Customers.Contains(user))
                {
                    ticket.Customers.Add(user);
                }
                else
                {
                    throw new InvalidOperationException("Usuário já está atribuído como Customer.");
                }
            }

            await _ticketRepository.UpdateAsync(ticket);

            return _mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task<TicketResponseDTO> AssignCustomersAsync(int ticketId, List<int> customerIds)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");

            var customers = await _ticketRepository.GetUsersByIdsAsync(customerIds);

            ticket.Customers = customers;

            await _ticketRepository.UpdateAsync(ticket);

            return _mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task<TicketResponseDTO> AssignAssigneesAsync(int ticketId, List<int> assigneeIds)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");

            var assignees = await _ticketRepository.GetUsersByIdsAsync(assigneeIds);

            ticket.Assignees = assignees;

            await _ticketRepository.UpdateAsync(ticket);

            return _mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task<TicketResponseDTO> RemoveAssigneeAsync(int ticketId, int assigneeId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");

            var assigneeToRemove = ticket.Assignees.FirstOrDefault(a => a.Id == assigneeId) ?? throw new KeyNotFoundException("Assignee não encontrado neste chamado.");

            ticket.Assignees.Remove(assigneeToRemove);

            await _ticketRepository.UpdateAsync(ticket);

            return _mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task<TicketResponseDTO> RemoveCustomerAsync(int ticketId, int customerId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");

            var customerToRemove = ticket.Customers.FirstOrDefault(c => c.Id == customerId) ?? throw new KeyNotFoundException("Cliente não encontrado neste chamado.");

            ticket.Customers.Remove(customerToRemove);

            await _ticketRepository.UpdateAsync(ticket);

            return _mapper.Map<TicketResponseDTO>(ticket);
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

            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepository.AddCommentAsync(comment);

            await _notificationService.Notify(ticket, author, content);

            return CommentResponseDTO.ValueOf(comment);
        }

        public async Task<CommentResponseDTO> UpdateCommentAsync(int ticketId, int commentId, string newContent, int currentUserId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Ticket não encontrado.");
            
            var comment = await _ticketRepository.GetCommentByIdAsync(commentId) ?? throw new KeyNotFoundException("Comentário não encontrado.");

            if (comment.TicketId != ticket.Id)
            {
                throw new InvalidOperationException("Comentário não pertence ao ticket informado.");
            }

            if (comment.AuthorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para editar este comentário.");
            }

            comment.Content = newContent;
            comment.UpdatedAt = DateTime.UtcNow;
            comment.IsUpdated = true;

            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepository.UpdateCommentAsync(comment);

            return CommentResponseDTO.ValueOf(comment);
        }

    }
}
