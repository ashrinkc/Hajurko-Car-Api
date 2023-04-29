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
                RentCount = 0
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
        [HttpGet("nonRented")]
        public async Task<IActionResult> GetNotRentedCars()
        {
            var cars = await _context.Cars
                .Where(c => c.RentCount == 0)
                .ToListAsync();

            return Ok(cars);
        }

        //Update car info
        [HttpPut("updateCar/{id}")]
        public async Task<IActionResult> UpdateCar(int id, UpdateCarDto updatedCar)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            //Update the car with their fields
            if (updatedCar.Brand != null)
            {
                car.Brand = updatedCar.Brand;
            }

            if (updatedCar.Model != null)
            {
                car.Model = updatedCar.Model;
            }

            if (updatedCar.Year != null)
            {
                car.Year = updatedCar.Year;
            }

            if (updatedCar.Description != null)
            {
                car.Description = updatedCar.Description;
            }

            if (updatedCar.Rating != null)
            {
                car.Rating = updatedCar.Rating;
            }

            if (updatedCar.Color != null)
            {
                car.Color = updatedCar.Color;
            }

            if (updatedCar.RegNumber != null)
            {
                car.RegNumber = updatedCar.RegNumber;
            }

            _context.Cars.Update(car);
            await _context.SaveChangesAsync();

            return Ok(car);
        }

    }
}
