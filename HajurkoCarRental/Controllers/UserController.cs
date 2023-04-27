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
       
    }
}
