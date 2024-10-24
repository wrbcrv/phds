using Api.DTOs;
using Api.Models;
using Api.Repositories.Interfaces;
using Api.Services.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IUserRepository userRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

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

        public async Task<TicketResponseDTO> AssignEntitiesAsync(int ticketId, List<int> entityIds, string entityType)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");
            var entities = await _ticketRepository.GetUsersByIdsAsync(entityIds);

            if (entityType == "Customer")
            {
                ticket.Customers = entities;
            }
            else if (entityType == "Assignee")
            {
                ticket.Assignees = entities;
            }
            else
            {
                throw new ArgumentException("Tipo de entidade inválido.");
            }

            await _ticketRepository.UpdateAsync(ticket);
            return TicketResponseDTO.ValueOf(ticket);
        }

        public async Task<TicketResponseDTO> RemoveEntityAsync(int ticketId, int entityId, string entityType)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId) ?? throw new KeyNotFoundException("Chamado não encontrado.");

            if (entityType == "Customer")
            {
                var entityToRemove = ticket.Customers.FirstOrDefault(c => c.Id == entityId) ?? throw new KeyNotFoundException("Cliente não encontrado neste chamado.");
                ticket.Customers.Remove(entityToRemove);
            }
            else if (entityType == "Assignee")
            {
                var entityToRemove = ticket.Assignees.FirstOrDefault(a => a.Id == entityId) ?? throw new KeyNotFoundException("Assignee não encontrado neste chamado.");
                ticket.Assignees.Remove(entityToRemove);
            }
            else
            {
                throw new ArgumentException("Tipo de entidade inválido.");
            }

            await _ticketRepository.UpdateAsync(ticket);
            return TicketResponseDTO.ValueOf(ticket);
        }
    }
}
