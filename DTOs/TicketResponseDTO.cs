using System;
using System.Collections.Generic;
using System.Linq;
using Api.DTOs;
using Api.Models;
using Type = Api.Models.Type;

namespace Api.DTOs
{
    public class TicketResponseDTO
    {
        public int Id { get; set; }
        public string? CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public AgencyHierarchyDTO? Location { get; set; }
        public string? Category { get; set; }
        public List<UserSummaryResponseDTO> Customers { get; set; } = [];
        public List<UserSummaryResponseDTO> Assignees { get; set; } = [];
        public List<UserSummaryResponseDTO> Observers { get; set; } = [];
        public List<CommentResponseDTO> Comments { get; set; } = [];

        public static TicketResponseDTO ValueOf(Ticket ticket)
        {
            return new TicketResponseDTO
            {
                Id = ticket.Id,
                CreatedAt = ticket.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                UpdatedAt = ticket.UpdatedAt.ToString("dd/MM/yyyy HH:mm"),
                Subject = ticket.Subject,
                Description = ticket.Description,
                Type = ticket.Type.ToString(),
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                Location = ticket.Location != null ? AgencyHierarchyDTO.ValueOf(ticket.Location) : null,
                Category = ticket.Category.ToString(),
                Customers = ticket.Customers != null ? ticket.Customers.Select(UserSummaryResponseDTO.ValueOf).ToList() : [],
                Assignees = ticket.Assignees != null ? ticket.Assignees.Select(UserSummaryResponseDTO.ValueOf).ToList() : [],
                Observers = ticket.Observers != null ? ticket.Observers.Select(UserSummaryResponseDTO.ValueOf).ToList() : [],
                Comments = ticket.Comments != null ? ticket.Comments.Select(CommentResponseDTO.ValueOf).ToList() : []
            };
        }
    }
}
