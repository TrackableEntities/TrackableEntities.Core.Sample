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

            //_context.Entry(order).State = EntityState.Modified;
            _context.ApplyChanges(order);

            try
            {
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
            _context.LoadRelatedEntities(order);

            // Reset tracking state to unchanged
            AcceptChanges(order);

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

            //_context.Orders.Add(order);
            order.TrackingState = TrackingState.Added;
            _context.ApplyChanges(order);

            await _context.SaveChangesAsync();

            // Populate reference properties
            _context.LoadRelatedEntities(order);

            // Reset tracking state to unchanged
            AcceptChanges(order);

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

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            //_context.Orders.Remove(order);
            order.TrackingState = TrackingState.Deleted;
            _context.ApplyChanges(order);

            await _context.SaveChangesAsync();

            return Ok();
        }

        private static void AcceptChanges(Order order)
        {
            order.TrackingState = TrackingState.Unchanged;
            order.Customer.TrackingState = TrackingState.Unchanged;
            order.OrderDetails.ToList().ForEach(m =>
            {
                m.TrackingState = TrackingState.Unchanged;
                m.Product.TrackingState = TrackingState.Unchanged;
            });
        }
    }
}