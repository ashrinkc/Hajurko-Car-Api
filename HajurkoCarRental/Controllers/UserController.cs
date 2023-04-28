using HajurkoCarRental.Data;
using HajurkoCarRental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HajurkoCarRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(int userId,string oldPassword, string newPassword)
        {
            var user = await _context.AppUsers.FindAsync(userId);
            if(user == null)
            {
                return NotFound();
            }

            //check if the old password matches
            var passwordHasher = new PasswordHasher<AppUser>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                return BadRequest("Old password is incorrect");
            }

            //Hash the new password and save it
            user.PasswordHash = passwordHasher.HashPassword(user,newPassword);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Password changed successfully");
        }

        //Get customers who have frequently rented cars
        [HttpGet("frequentrenters")]
        public async Task<IActionResult> GetFrequentRenters()
        {
            var customers = await _context.CarRentalRequest
                .Include(r => r.User)
                .Where(r => r.IsApproved)
                .GroupBy(r => r.UserId)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    UserId = g.Key,
                    FullName = g.First().User.FullName,
                    RentalCount = g.Count()
                }).ToListAsync();

            return Ok(customers);
        }

        //Get inactive customers
        [HttpGet("inactivecustomers")]
        public async Task<IActionResult> GetInactiveCustomers()
        {
            var cutOffDate = DateTime.Today.AddMonths(-3);

            var customers = await _context.CarRentalRequest
                .Include(r => r.User)
                .Where(r => r.IsApproved && r.RentalEnd < cutOffDate)
                .GroupBy(r => r.UserId)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    UserId = g.Key,
                    FullName = g.First().User.FullName,
                    RentalCount = g.Count()
                }).ToListAsync();

            return Ok(customers);
        }
       
    }
}
