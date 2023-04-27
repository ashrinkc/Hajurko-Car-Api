using HajurkoCarRental.Data;
using HajurkoCarRental.Dto;
using HajurkoCarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HajurkoCarRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly DataContext _context;

        public CarController(DataContext context)
        {
            _context = context;
        }

        //Get all cars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarDto>>> GetCars()
        {
            var cars = await _context.Cars.ToListAsync();
            var carDtos = cars.Select(car => new CarDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Description = car.Description,
                IsAvailable = car.IsAvailable,
                DailyRentalFee = car.DailyRentalFee,
            });
            return Ok(carDtos);
        }

        //Get cars by id
        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if(car == null)
            {
                return NotFound();
            }

            var carDto = new CarDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Description = car.Description,
                IsAvailable = car.IsAvailable,
                DailyRentalFee = car.DailyRentalFee,
            };
            return Ok(carDto);
        }

        //Add car
        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] CarDto carDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var car = new Car
            {
                Brand = carDto.Brand,
                Model = carDto.Model,
                Year = carDto.Year,
                Description = carDto.Description,
                DailyRentalFee = carDto.DailyRentalFee,
                IsAvailable = carDto.IsAvailable
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return Ok("Car added successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if(car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return Ok("Car removes successfully");
        }
    }
}
