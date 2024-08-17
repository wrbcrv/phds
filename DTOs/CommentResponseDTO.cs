using System;
using Api.DTOs;
using Api.Models;

namespace Api.DTOs
{
    public class CommentResponseDTO
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public string Content { get; set; }
        public UserResponseDTO Author { get; set; }

        public static CommentResponseDTO ValueOf(Comment comment)
        {
            return new CommentResponseDTO
            {
                Id = comment.Id,
                CreatedAt = comment.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                Content = comment.Content,
                Author = comment.Author != null ? UserResponseDTO.ValueOf(comment.Author) : null
            };
        }
    }
}
