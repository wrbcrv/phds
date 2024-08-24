using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controller
{
    [ApiController]
    [Route("api/tickets")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<ActionResult<PagedResponseDTO<TicketResponseDTO>>> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                var result = await _ticketService.GetAllAsync(page, size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<ActionResult<TicketResponseDTO>> GetById(int id)
        {
            try
            {
                var result = await _ticketService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<ActionResult<TicketResponseDTO>> Create(TicketDTO ticketDTO)
        {
            try
            {
                var result = await _ticketService.CreateAsync(ticketDTO);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<IActionResult> Update(int id, TicketDTO ticketDTO)
        {
            try
            {
                var result = await _ticketService.UpdateAsync(id, ticketDTO);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingTicket = await _ticketService.GetByIdAsync(id);
                if (existingTicket == null)
                {
                    return NotFound();
                }

                await _ticketService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("{ticketId}/comments/{authorId}")]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<ActionResult<CommentResponseDTO>> AddComment(int ticketId, int authorId, [FromBody] CommentDTO commentDTO)
        {
            try
            {
                var result = await _ticketService.AddCommentAsync(ticketId, authorId, commentDTO.Content);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
