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
    public class CarDamageController : ControllerBase
    {
        private readonly DataContext _context;

        public CarDamageController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCarDamageRequest([FromBody] CarDamageRequestDto damageDto)
        {
            //check if the car rental request exists and is approved
            var carRental = await _context.CarRental.FindAsync(damageDto.CarRentalId);
            if(carRental == null)
            {
                return BadRequest("Invalid request id or request not approved");
            }

            // Check if the car damage request already exists and is not paid
            var existingDamage = await _context.CarDamages.FirstOrDefaultAsync(cd => cd.CarRentalId == damageDto.CarRentalId && cd.IsPaid == false);
            if (existingDamage != null)
            {
                return BadRequest("Car damage request already exists and is not paid.");
            }

            var car = await _context.Cars.FindAsync(carRental.CarId);
            var user = await _context.AppUsers.FindAsync(carRental.CustomerId);
            if(car == null || user == null)
            {
                return NotFound();
            }
            var carDamage = new CarDamage
            { 
                CarRentalId = damageDto.CarRentalId,
                Description = damageDto.Description,
                RepairCost = 0,
                TotalAmountPaid = 0,
                IsPaid = false
            };

            _context.CarDamages.Add(carDamage);
            await _context.SaveChangesAsync();
            var email = new EmailSenderService();
            await email.SendEmailAsync("ashrinkc3@yopmail.com", "Car Damage", car.Model + " has been damaged by " + user.FullName);
            return Ok("Car damage request successfully created");
        }

        //Get damaged car info
        [HttpGet]
        public async Task<IActionResult> GetCarDamage()
        {
            var carDamage = await _context.CarDamages
                .Where(c => !c.IsRepaired)
                .Select(c => new CarDamageDto
            {
                Id = c.Id,
                CarBrand = c.CarRental.Car.Brand,
                CarModel = c.CarRental.Car.Model,
                Customer = c.CarRental.Customer.FullName,
                Description = c.Description,
                CustomerEmail = c.CarRental.Customer.Email,
                CarId = c.CarRental.Customer.Id
            }).ToListAsync();

            return Ok(carDamage);
        }

        //Repair damaged cars
        [HttpPost("repair/{id}")]
        public async Task<IActionResult> RepairDamagedCar(int id, [FromBody] RepairCarDto model)
        {
            var carDamage = await _context.CarDamages.FindAsync(id);
            
            if(carDamage == null)
            {
                return NotFound();
            }

            carDamage.IsRepaired = true;
            _context.Update(carDamage);

            //set the corresponding cars isavailable property to true
            var car = await _context.Cars.FindAsync(model.CarId);
            if(car == null)
            {
                return NotFound();
            }
            car.IsAvailable = true;
            _context.Update(car);

            await _context.SaveChangesAsync();
            return Ok("Car repaired successfull");
        }

    }
}
