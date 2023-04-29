using HajurkoCarRental.Data;
using HajurkoCarRental.Dto;
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

        //Update user
        [HttpPut("updateUser/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId,UpdateUserDto updatedUser )
        {
            var user = await _context.AppUsers.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            //Update the user with their fields
            if (user.FullName != null)
            {
                user.FullName = updatedUser.FullName;
            }

            if (user.UserType != null)
            {
                user.UserType = updatedUser.UserType;
            }

            if (user.Phone != null)
            {
                user.Phone = updatedUser.Phone;
            }

            if (user.Address != null)
            {
                user.Address = updatedUser.Address;
            }

            if (user.IsRegular != null)
            {
                user.IsRegular = updatedUser.IsRegular;
            }

            if(user.Document != null)
            {
                user.Document = updatedUser.Document;
            }

            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
            return Ok(user);
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
