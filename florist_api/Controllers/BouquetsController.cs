using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using florist_api.Models;
using florist_api.Services;

namespace florist_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BouquetsController : ControllerBase
    {
        private readonly IBouquetService _service;

        public BouquetsController(IBouquetService service)
        {
            _service = service;
        }

        // GET: api/bouquets
        // PUBLIC: Anyone can see the catalog
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bouquet>>> GetAll()
        {
            var bouquets = await _service.GetAllBouquetsAsync();
            return Ok(bouquets);
        }

        // GET: api/bouquets/5
        // AUTHORIZED: Customers, Employees, and Admins can view details
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Bouquet>> GetById(int id)
        {
            var bouquet = await _service.GetByIdAsync(id);
            if (bouquet == null) return NotFound();
            
            return Ok(bouquet);
        }

        // POST: api/bouquets
        // ADMIN ONLY: Only the shop owner can add new products
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Bouquet>> Create([FromBody] Bouquet bouquet)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateBouquetAsync(bouquet);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PATCH: api/bouquets/5/price
        // ADMIN ONLY: Update price for a specific bouquet
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] decimal newPrice)
        {
            var success = await _service.UpdatePriceAsync(id, newPrice);
            if (!success) return NotFound();

            return NoContent();
        }

        // DELETE: api/bouquets/5
        // ADMIN ONLY: Remove from inventory
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}