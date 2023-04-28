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
                Rating = car.Rating,
                Color = car.Color,
                RegNumber = car.RegNumber,
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
                Rating = car.Rating,
                Color = car.Color,
                RegNumber = car.RegNumber,
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
                IsAvailable = carDto.IsAvailable,
                Rating = carDto.Rating,
                Color = carDto.Color,
                RegNumber = carDto.RegNumber,
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return Ok("Car added successfully");
        }

        //Remove car
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

        //Get rented cars
        [HttpGet("rented")]
        public async Task<IActionResult> GetRentedCars()
        {
            var cars = await _context.Cars
                .Where(c => c.IsAvailable == false)
                .ToListAsync();

            return Ok(cars);
        }

        //Get available cars
        [HttpGet("availableCars")]
        public async Task<IActionResult> GetAvailableCars()
        {
            var cars = await _context.Cars
                .Where(c => c.IsAvailable == true)
                .ToListAsync();

            return Ok(cars);   
        }

        //Get frequently rented cars
        [HttpGet("frequentlyRented")]
        public async Task<IActionResult> GetFrequentlyRentedCars()
        {
            var cars = await _context.Cars
                .OrderByDescending(c => c.RentCount)
                .Take(10)
                .ToListAsync();

            return Ok(cars);
        }

        //Get cars that have not been rented
        public async Task<IActionResult> GetNotRentedCars()
        {
            var cars = await _context.Cars
                .Where(c => c.RentCount == 0)
                .ToListAsync();

            return Ok(cars);
        }

    }
}
