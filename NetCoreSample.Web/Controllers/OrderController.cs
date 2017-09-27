using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreSample.Entities;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core;

namespace NetCoreSample.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Order")]
    public class OrderController : Controller
    {
        private readonly NorthwindSlimContext _context;

        public OrderController(NorthwindSlimContext context)
        {
            _context = context;
        }

        // GET: api/Order
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(m => m.Customer)
                .Include(m => m.OrderDetails)
                .ThenInclude(m => m.Product)
                .ToListAsync();
            return Ok(orders);
        }

        // GET: api/Order/ALFKI
        [HttpGet("{customerId:alpha}")]
        public async Task<IActionResult> GetOrders([FromRoute] string customerId)
        {
            var orders = await _context.Orders
                .Include(m => m.Customer)
                .Include(m => m.OrderDetails)
                .ThenInclude(m => m.Product)
                .Where(m => m.CustomerId == customerId)
                .ToListAsync();
            return Ok(orders);
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders
                .Include(m => m.Customer)
                .Include(m => m.OrderDetails)
                .ThenInclude(m => m.Product)
                .SingleOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Order
        [HttpPut]
        public async Task<IActionResult> PutOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Apply changes to context
            _context.ApplyChanges(order);

            try
            {
                // Persist changes
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(o => o.OrderId == order.OrderId))
                {
                    return NotFound();
                }
                throw;
            }

            // Populate reference properties
            await _context.LoadRelatedEntitiesAsync(order);

            // Reset tracking state to unchanged
            _context.AcceptChanges(order);

            //return NoContent();
            return Ok(order);
        }

        // POST: api/Order
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set state to added
            order.TrackingState = TrackingState.Added;

            // Apply changes to context
            _context.ApplyChanges(order);

            // Persist changes
            await _context.SaveChangesAsync();

            // Populate reference properties
            await _context.LoadRelatedEntitiesAsync(order);

            // Reset tracking state to unchanged
            _context.AcceptChanges(order);

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // DELETE: api/Order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Retrieve order with details
            var order = await _context.Orders
                .Include(m => m.OrderDetails)
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            // Set tracking state to deleted
            order.TrackingState = TrackingState.Deleted;

            // Detach object graph
            _context.DetachEntities(order);

            // Apply changes to context
            _context.ApplyChanges(order);

            // Persist changes
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}