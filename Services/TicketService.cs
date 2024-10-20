using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services
{
    public class TicketService
    (
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        INotificationService notificationRepository,
        IMapper mapper,
        IFileUploadService fileUploadService
    ) : ITicketService
    {
        private readonly ITicketRepository _ticketRepository = ticketRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly INotificationService _notificationService = notificationRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IFileUploadService _fileUploadService = fileUploadService;

        public async Task<PagedResponseDTO<TicketResponseDTO>> GetAllAsync(int page, int size, TicketFilter filter = null)
        {
            var tickets = await _ticketRepository.GetAllAsync(page, size, filter);
            var dtos = tickets.Items.Select(t => _mapper.Map<TicketResponseDTO>(t)).ToList();
            return new PagedResponseDTO<TicketResponseDTO>
            {
                Items = dtos,
                Total = tickets.Total
            };
        }

        public async Task<TicketResponseDTO> GetByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            return TicketResponseDTO.ValueOf(ticket);
        }

        public async Task<TicketResponseDTO> CreateAsync(TicketDTO ticketDTO)
        {
            var location = await _ticketRepository.GetAgencyByIdAsync(ticketDTO.LocationId);
            var customers = await _ticketRepository.GetUsersByIdsAsync(ticketDTO.CustomerIds);
            var assignees = await _ticketRepository.GetUsersByIdsAsync(ticketDTO.AssigneeIds);

            var ticket = _mapper.Map<Ticket>(ticketDTO);

            ticket.Location = location;
            ticket.Customers = customers;
            ticket.Assignees = assignees;

            await _ticketRepository.AddAsync(ticket);
            return TicketResponseDTO.ValueOf(ticket);
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
            return TicketResponseDTO.ValueOf(ticket);
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
            return TicketResponseDTO.ValueOf(ticket);
        }

        public async Task<TicketResponseDTO> AssignCustomersAsync(int ticketId, List<int> customerIds)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");
            var customers = await _ticketRepository.GetUsersByIdsAsync(customerIds);
            ticket.Customers = customers;
            await _ticketRepository.UpdateAsync(ticket);
            return TicketResponseDTO.ValueOf(ticket);
        }

        public async Task<TicketResponseDTO> AssignAssigneesAsync(int ticketId, List<int> assigneeIds)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");
            var assignees = await _ticketRepository.GetUsersByIdsAsync(assigneeIds);
            ticket.Assignees = assignees;
            await _ticketRepository.UpdateAsync(ticket);
            return TicketResponseDTO.ValueOf(ticket);
        }

        public async Task<TicketResponseDTO> RemoveAssigneeAsync(int ticketId, int assigneeId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");
            var assigneeToRemove = ticket.Assignees.FirstOrDefault(a => a.Id == assigneeId) ?? throw new KeyNotFoundException("Assignee não encontrado neste chamado.");
            ticket.Assignees.Remove(assigneeToRemove);
            await _ticketRepository.UpdateAsync(ticket);
            return TicketResponseDTO.ValueOf(ticket);
        }

        public async Task<TicketResponseDTO> RemoveCustomerAsync(int ticketId, int customerId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");
            var customerToRemove = ticket.Customers.FirstOrDefault(c => c.Id == customerId) ?? throw new KeyNotFoundException("Cliente não encontrado neste chamado.");
            ticket.Customers.Remove(customerToRemove);
            await _ticketRepository.UpdateAsync(ticket);
            return TicketResponseDTO.ValueOf(ticket);
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

            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    var filePath = await _fileUploadService.UploadFileAsync(file, "uploads/comments");
                    var commentFile = new CommentFile
                    {
                        FilePath = filePath,
                        FileName = file.FileName,
                        Comment = comment
                    };
                    comment.Files.Add(commentFile);
                }
            }

            await _ticketRepository.AddCommentAsync(comment);
            await _notificationService.Notify(ticket, author, content);

            return CommentResponseDTO.ValueOf(comment);
        }

        public async Task<CommentResponseDTO> UpdateCommentAsync(int ticketId, int commentId, string newContent)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Ticket não encontrado.");
            var comment = await _ticketRepository.GetCommentByIdAsync(commentId) ?? throw new KeyNotFoundException("Comentário não encontrado.");

            if (comment.TicketId != ticket.Id)
            {
                throw new InvalidOperationException("Comentário não pertence ao ticket informado.");
            }

            comment.Content = newContent;
            comment.UpdatedAt = DateTime.UtcNow;
            comment.IsUpdated = true;

            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepository.UpdateCommentAsync(comment);
            return CommentResponseDTO.ValueOf(comment);
        }

        public async Task DeleteCommentAsync(int ticketId, int commentId)
        {
            TimeSpan removalTimeLimit = TimeSpan.FromMinutes(5);
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Ticket não encontrado.");
            var comment = await _ticketRepository.GetCommentByIdAsync(commentId) ?? throw new KeyNotFoundException("Comentário não encontrado.");

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

            await _ticketRepository.DeleteCommentAsync(commentId);
        }

        public async Task<FileResult> DownloadCommentFileAsync(int ticketId, int commentId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException("Ticket não encontrado.");
            }

            var comment = await _ticketRepository.GetCommentByIdAsync(commentId);
            if (comment == null || comment.TicketId != ticketId)
            {
                throw new KeyNotFoundException("Comentário não encontrado ou não pertence ao ticket informado.");
            }

            var commentFile = comment.Files.FirstOrDefault();
            if (commentFile == null)
            {
                throw new InvalidOperationException("Nenhum arquivo associado ao comentário.");
            }

            return await _fileUploadService.DownloadFileAsync(commentFile.FilePath);
        }
    }
}
