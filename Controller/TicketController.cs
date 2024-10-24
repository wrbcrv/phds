using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Api.Models;
using System.Security.Claims;

namespace Api.Controller
{
    [ApiController]
    [Route("api/tickets")]
    [Authorize]
    public class TicketController(ITicketService ticketService) : ControllerBase
    {
        private readonly ITicketService _ticketService = ticketService;

        [HttpGet]
        [Authorize(Roles = "Administrator, Agent, Client")]
        public async Task<ActionResult<PagedResponseDTO<TicketResponseDTO>>> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] TicketFilter filter = null)  // Adiciona o filtro como parâmetro
        {
            try
            {
                var result = await _ticketService.GetAllAsync(page, size, filter);
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

        [HttpPut("{ticketId}/assign-current-user")]
        [Authorize(Roles = "Administrator, Agent")]
        public async Task<IActionResult> AssignCurrentUser(int ticketId, [FromQuery] bool asAssignee = true)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var result = await _ticketService.AssignCurrentUserAsync(ticketId, currentUserId, asAssignee);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{ticketId}/{entityType}")]
        [Authorize(Roles = "Administrator, Agent")]
        public async Task<IActionResult> AssignEntities(int ticketId, string entityType, [FromBody] List<int> entityIds)
        {
            try
            {
                var result = await _ticketService.AssignEntitiesAsync(ticketId, entityIds, entityType);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{ticketId}/{entityType}/{entityId}")]
        [Authorize(Roles = "Administrator, Agent")]
        public async Task<IActionResult> RemoveEntity(int ticketId, string entityType, int entityId)
        {
            try
            {
                var result = await _ticketService.RemoveEntityAsync(ticketId, entityId, entityType);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
