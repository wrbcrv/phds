using Api.DTOs;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controller
{
    [ApiController]
    [Route("api/agencies")]
    [Authorize]
    public class AgencyController(IAgencyService agencyService) : ControllerBase
    {
        private readonly IAgencyService _agencyService = agencyService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var agencies = await _agencyService.GetAllAsync();
                return Ok(agencies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var agency = await _agencyService.GetByIdAsync(id);
                if (agency == null)
                {
                    return NotFound();
                }
                return Ok(agency);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(AgencyDTO agencyDTO)
        {
            try
            {
                var createdAgency = await _agencyService.CreateAsync(agencyDTO);
                return CreatedAtAction(nameof(GetById), new { id = createdAgency.Id }, createdAgency);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(int id, AgencyDTO agencyDTO)
        {
            try
            {
                var updatedAgency = await _agencyService.UpdateAsync(id, agencyDTO);
                return Ok(updatedAgency);
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
                await _agencyService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
