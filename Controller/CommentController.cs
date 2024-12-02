using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controller
{
    [ApiController]
    [Route("api/tickets/{ticketId}/comments")]
    [Authorize]
    public class CommentController(ICommentService commentService) : ControllerBase
    {
        
        [HttpGet]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<ActionResult<IEnumerable<CommentResponseDTO>>> GetCommentsByTicketId(int ticketId)
        {
            try
            {
                var comments = await _commentService.GetCommentsByTicketIdAsync(ticketId);
                if (comments == null || !comments.Any())
                {
                    return NotFound("Nenhum comentário encontrado para este ticket.");
                }

                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        private readonly ICommentService _commentService = commentService;

        [HttpPost("{authorId}")]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<ActionResult<CommentResponseDTO>> AddComment(int ticketId, int authorId, [FromForm] string content, [FromForm] List<IFormFile> files = null)
        {
            try
            {
                if (string.IsNullOrEmpty(content) && (files == null || !files.Any()))
                {
                    return BadRequest("O comentário deve ter algum conteúdo ou arquivo.");
                }

                var result = await _commentService.AddCommentAsync(ticketId, authorId, content, files);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPut("{commentId}")]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<IActionResult> UpdateComment(int ticketId, int commentId, [FromBody] CommentDTO commentDTO)
        {
            try
            {
                var result = await _commentService.UpdateCommentAsync(ticketId, commentId, commentDTO.Content);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpDelete("{commentId}")]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<IActionResult> DeleteComment(int ticketId, int commentId)
        {
            try
            {
                await _commentService.DeleteCommentAsync(ticketId, commentId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
