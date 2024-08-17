using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponseDTO<TicketResponseDTO>>> GetAll()
        {
            var result = await _ticketService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponseDTO>> GetById(int id)
        {
            var result = await _ticketService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<TicketResponseDTO>> Create(TicketDTO ticketDTO)
        {
            var result = await _ticketService.CreateAsync(ticketDTO);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TicketDTO ticketDTO)
        {
            var result = await _ticketService.UpdateAsync(id, ticketDTO);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingTicket = await _ticketService.GetByIdAsync(id);
            if (existingTicket == null)
            {
                return NotFound();
            }

            await _ticketService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{ticketId}/comments/{authorId}")]
        public async Task<ActionResult<CommentResponseDTO>> AddComment(int ticketId, int authorId, [FromBody] CommentDTO commentDTO)
        {
            var result = await _ticketService.AddCommentAsync(ticketId, authorId, commentDTO.Content);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
