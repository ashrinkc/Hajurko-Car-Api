using HajurkoCarRental.Data;
using HajurkoCarRental.Dto;
using HajurkoCarRental.Models;
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
            var carDamage = new CarDamage
            { 
                CarRentalId = damageDto.CarRentalId,
                Description = damageDto.Description,
                IsPaid = false
            };

            _context.CarDamages.Add(carDamage);
            await _context.SaveChangesAsync();

            return Ok("Car damage request successfully created");
        }

        //Get damaged car info
        [HttpGet]
        public async Task<IActionResult> GetCarDamage()
        {
            var carDamage = await _context.CarDamages.Select(c => new CarDamageDto
            {
                Id = c.Id,
                CarBrand = c.CarRental.Car.Brand,
                CarModel = c.CarRental.Car.Model,
                Customer = c.CarRental.Customer.FullName,
                Description = c.Description,
            }).ToListAsync();

            return Ok(carDamage);
        }

    }
}
