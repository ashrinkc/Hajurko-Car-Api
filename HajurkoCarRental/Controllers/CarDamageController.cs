using HajurkoCarRental.Data;
using HajurkoCarRental.Dto;
using HajurkoCarRental.Models;
using Microsoft.AspNetCore.Mvc;

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
            var carRentalRequest = await _context.CarRentalRequest.FindAsync(damageDto.CarRentalRequestId);

            if(carRentalRequest == null || !carRentalRequest.IsApproved)
            {
                return BadRequest("Invalid request id or request not approved");
            }
            var carDamage = new CarDamage
            {
                CarRentalRequestId = damageDto.CarRentalRequestId,
                Description = damageDto.Description,
                IsPaid = false
            };

            _context.CarDamages.Add(carDamage);
            await _context.SaveChangesAsync();

            return Ok("Car damage request successfully created");
        }
    }
}
