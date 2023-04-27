using HajurkoCarRental.Data;
using HajurkoCarRental.Dto;
using HajurkoCarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HajurkoCarRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly DataContext _context;

        public RentalController(DataContext context)
        {
            _context = context;
        }

        //Request to rent a car
        [HttpPost]
        public async Task<IActionResult> SendRentalRequest([FromBody] CarRentalRequestDto reqDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = new CarRentalRequest
            {
                CarId = reqDto.CarId,
                UserId = reqDto.UserId,
                RentalStart = reqDto.RentalStart,
                RentalEnd = reqDto.RentalEnd
            };

            _context.CarRentalRequest.Add(request);
            await _context.SaveChangesAsync();
            return Ok("Request sent successfully");
        }

        //Get incoming rental request
        [HttpGet]
        public async Task<IActionResult> GetRentalRequest()
        {
            var request = await _context.CarRentalRequest.ToListAsync();
            return Ok(request);
        }

        //To accept or reject incoming car rental request
        [HttpPost("request/authorize/{reqId}")]
        public async Task<IActionResult> ValidateRequest(int reqId, [FromBody] bool status,[FromBody] int staffId)
        {
            var request = await _context.CarRentalRequest.FindAsync(reqId);
            if(request == null)
            {
                return NotFound();
            }

            //To check if the car exists
            var carExists = await _context.Cars.AnyAsync(c => c.Id == request.CarId);
            if (!carExists)
            {
                return BadRequest("Invalid car request");
            }

            //To check if the user exists
            var userExists = await _context.AppUsers.AnyAsync(u => u.Id == request.UserId);

            if(status == true)
            {
                request.IsApproved = true;
                var car = await _context.Cars.FindAsync(request.CarId);
                if( car != null)
                {
                    car.IsAvailable = false;
                    _context.Cars.Update(car);
                }

                var carRental = new CarRental
                {
                    RentalDate = request.RentalStart,
                    RentalEnd = request.RentalEnd,
                    CarId = request.CarId,
                    CustomerId = request.UserId,
                    StaffId = staffId
                };

                _context.CarRental.Add(carRental);
                
            }else if(status == false)
            {
                request.IsApproved=false;
            }

            await _context.SaveChangesAsync();

            return Ok("Request status updated");
        }

        //Get rental history of a user
        [HttpGet("rentalHistory/{customerId}")]
        public async Task<IActionResult> GetRentalHistory(int customerId) 
        {
            var history = await _context.CarRental
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Select(r => new RentalHistoryDto
                {
                    RentalId = r.Id,
                    CustomerName = r.Customer.FirstName,
                    StaffName = r.Staff.FirstName,
                    RentalFee = r.Charge,
                    RentalStart = r.RentalDate,
                    RentalEnd = r.RentalEnd,
                }).ToListAsync();
            return Ok(history);
        }
    }
}
