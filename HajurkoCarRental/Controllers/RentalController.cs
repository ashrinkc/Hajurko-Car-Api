using HajurkoCarRental.Data;
using HajurkoCarRental.Dto;
using HajurkoCarRental.Models;
using HajurkoCarRental.Services;
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

            //check if the requested car is available to rent
            var car = await _context.Cars.FindAsync(reqDto.CarId);

            var damage = await _context.CarDamages.FirstOrDefaultAsync(d =>
                    d.CarRental.CustomerId == reqDto.UserId
            );

            var user = await _context.AppUsers.FindAsync(reqDto.UserId);

            //Check if driving license or citizenship of the user is present
            if (user.Document == null)
            {
                return BadRequest("Citizenship or driving license is mandatory");
            }

            //Check if the user has damaged previous rented cars
            if (damage != null)
            {
                //check if the user has not paid for the damaged car before renting another car
                if (damage.IsPaid == false)
                {
                    return Unauthorized("Damage payment pending");
                }
            }

            //Check if the car is available
            if (!car.IsAvailable)
            {
                return NotFound("The requested car is not currently available");
            }

            var request = new CarRentalRequest
            {
                CarId = reqDto.CarId,
                UserId = reqDto.UserId,
                RentalStart = reqDto.RentalStart,
                RentalEnd = reqDto.RentalEnd,
                Charge = reqDto.Charge
            };

            _context.CarRentalRequest.Add(request);
            await _context.SaveChangesAsync();
            return Ok("Request sent successfully");
        }

        //Get incoming rental request
        [HttpGet]
        public async Task<IActionResult> GetRentalRequest()
        {
            var request = await _context.CarRentalRequest.Where(r => !r.Cancelled && r.Status == "Pending")
                .Include(r => r.Car)
                .Include(r => r.User)
                .Select(r => new
                {
                    Id = r.Id,
                    StartDate = r.RentalStart,
                    EndDate = r.RentalEnd,
                    Status = r.Status,
                    CarBrand = r.Car.Brand,
                    CarModel = r.Car.Model,
                    Charge = r.Charge,
                    Username = r.User.FullName,
                    Email = r.User.Email
                })
                .ToListAsync();
            return Ok(request);
        }

        //To accept or reject incoming car rental request
        [HttpPost("request/authorize/{reqId}")]
        public async Task<IActionResult> ValidateRequest(int reqId, [FromBody] ValidateRequestDto model)
        {
            //Find the request
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
            var userExists = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (userExists == null)
            {
                return BadRequest("No such user found");
            }
            if(model.status == true)
            {
                request.IsApproved = true;
                request.Status = "Approved";
                var email = new EmailSenderService();
                await email.SendEmailAsync(userExists.Email,"Rental Request Status","Your request to rent car has benn approved");
                var car = await _context.Cars.FindAsync(request.CarId);
                if( car != null)
                {
                    car.IsAvailable = false;
                    car.RentCount += 1;
                    _context.Cars.Update(car);
                }

                var carRental = new CarRental
                {
                    RentalDate = request.RentalStart,
                    RentalEnd = request.RentalEnd,
                    CarId = request.CarId,
                    CustomerId = request.UserId,
                    Charge = request.Charge,
                    StaffId = model.staffId
                };

                _context.CarRental.Add(carRental);
                
            }else if(model.status == false)
            {
                request.Status = "Rejected";
                request.IsApproved=false;
                var email = new EmailSenderService();
                await email.SendEmailAsync(userExists.Email, "Rental Request Status", "Your request to rent car has benn rejected");
            }

            await _context.SaveChangesAsync();

            return Ok("Request status updated");
        }

        //Get all rented cars
        [HttpGet("rented")]
        public async Task<IActionResult> GetRentedCars()
        {
            var cars = await _context.CarRental.Where(r => !r.IsReturned)
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .Select(r => new
                {
                    CarModel = r.Car.Model,
                    CarBrand = r.Car.Brand,
                    CustomerName = r.Customer.FullName,
                    CustomerEmail = r.Customer.Email,
                    StartDate = r.RentalDate,
                    EndDate = r.RentalEnd,
                    Charge = r.Charge,
                    Id = r.Id
                }).ToListAsync();

            return Ok(cars); 
        }

        //Return rented car
        [HttpPost("return/{id}")]
        public async Task<IActionResult> ReturnCar(int id, [FromBody] DateTime? returnDate)
        {
            var carRental = await _context.CarRental.FindAsync(id);

            if(carRental == null)
            {
                return NotFound();
            }

            carRental.RentalEnd = returnDate;
            carRental.IsReturned = true;
            _context.CarRental.Update(carRental);
            await _context.SaveChangesAsync();

            return Ok("Car successfully returned");
        }

        //Cancel car request
        [HttpPost("cancelRequest/{id}")]
        public async Task<IActionResult> CancelCarRequest(int id)
        {
            var carRentalRequest = await _context.CarRentalRequest.FindAsync(id);
            if(carRentalRequest == null)
            {
                return NotFound();
            }

            carRentalRequest.Cancelled = true;
            _context.CarRentalRequest.Update(carRentalRequest);
            await _context.SaveChangesAsync();

            return Ok("Car rental request successfully cancelled");
        }

        //Get customers rented car
        [HttpGet("rentedCar/{customerId}")]
        public async Task<IActionResult> GetRentedCar(int customerId)
        {
            var car = await _context.CarRental.Where(r => r.CustomerId == customerId).FirstOrDefaultAsync();
            return Ok(car);
        }

        //Get rental history of a user
        [HttpGet("rentalHistory/{customerId}")]
        public async Task<IActionResult> GetRentalHistory(int customerId, DateTime? startDate = null, DateTime?  endDate = null) 
        {
            var history = await _context.CarRental
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .Select(r => new RentalHistoryDto
                {
                    RentalId = r.Id,
                    CustomerName = r.Customer.FullName,
                    StaffName = r.Staff.FullName,
                    RentalFee = r.Charge,
                    RentalStart = r.RentalDate,
                    RentalEnd = r.RentalEnd,
                }).ToListAsync();

            if(startDate != null)
            {
                history = history.Where(r => r.RentalStart == startDate.Value).ToList();
            }

            if(endDate != null)
            {
                history = history.Where(r => r.RentalEnd == endDate).ToList();
            }
            return Ok(history);
        }

    }
}
