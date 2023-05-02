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

        //Get all users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.AppUsers.ToListAsync();
            return Ok(users);
        }

        //get user data
        [HttpGet("indiUser/{id}")]
        public async Task<IActionResult> GetIndiUser(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            return Ok(user);
        }

        // Change user password
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] changePasswordDto model)
        {
            var user = await _context.AppUsers.FindAsync(model.userId);
            if(user == null)
            {
                return NotFound();
            }

            //check if the old password matches
            var passwordHasher = new PasswordHasher<AppUser>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.oldPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                return BadRequest("Old password is incorrect");
            }

            //Hash the new password and save it
            user.PasswordHash = passwordHasher.HashPassword(user,model.newPassword);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok("Password changed successfully");
        }

        //Update user
        [HttpPut("updateUser/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId,[FromBody] UpdateUserDto updatedUser )
        {
            var user = await _context.AppUsers.FindAsync(userId);
           

            if (user == null)
            {
                return NotFound();
            }

            if (updatedUser.Document != null)
            {
               
                
            }
            //Update the user with their fields
            //if (user.FullName != null)
            //{
            //    user.FullName = updatedUser.FullName;
            //}

            if (updatedUser.UserType != null)
            {
                user.UserType = updatedUser.UserType;
            }

            if (updatedUser.Phone != null)
            {
                user.Phone = updatedUser.Phone;
            }

            if (updatedUser.Address != null)
            {
                user.Address = updatedUser.Address;
            }

            if (updatedUser.IsRegular != null)
            {
               user.IsRegular = updatedUser.IsRegular;
            }

            
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        //Update document
        [HttpPut("updateDocument/{userId}")]
        public async Task<IActionResult> UpdateDocument(int userId,[FromBody] UpdateUserDto updatedUser)
        {
            var user = await _context.AppUsers.FindAsync(userId);
            if(user == null)
            {
                return NotFound();
            }
            if(updatedUser.Document == null)
            {
                return BadRequest("Document required");
            }
            if (updatedUser.Document.Length > 1572864) // 1.5MB in bytes
            {
                return BadRequest("File size should not exceed 1.5MB.");
            }

            user.Document = updatedUser.Document;

            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
            return Ok("Document added");
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
                    Email = g.First().User.Email,
                    Address = g.First().User.Address,
                    RentalCount = g.Count()
                }).ToListAsync();

            return Ok(customers);
        }

        //Get inactive customers
        [HttpGet("inactivecustomers")]
        public async Task<IActionResult> GetInactiveCustomers()
        {
            var cutOffDate = DateTime.Today.AddMonths(-3);

           
            //var customers = await _context.CarRentalRequest
            //    .Include(r => r.User)
            //    .Where(r => r.IsApproved && r.RentalEnd < cutOffDate)
            //    .GroupBy(r => r.UserId)
            //    .OrderByDescending(g => g.Count())
            //    .Select(g => new
            //    {
            //        UserId = g.Key,
            //        FullName = g.First().User.FullName,
            //        RentalCount = g.Count()
            //    }).ToListAsync();

            var inactiveuser = await _context.AppUsers
                .Join(_context.CarRental,
                u => u.Id, r => r.CustomerId,
                (u, r) => new { User = u, Rental = r })
                .Where(ur => ur.Rental.RentalEnd < cutOffDate)
                .GroupBy(ur => ur.User.Id)
                .Where(g => g.Count() == 0)
                .Select(g => new
                {
                    UserId = g.Key,
                    FullName = g.First().User.FullName,
                    Email = g.First().User.Email,
                    Address = g.First().User.Address,
                }).ToListAsync();
            return Ok(inactiveuser);
        }


        //Check if the customer is regular
        [HttpGet("isRegular/{userId}")]
        public async Task<IActionResult> CheckRegular(int userId)
        {
            //Get the current date and date one month ago
            var today = DateTime.UtcNow.Date;
            var lastMonth = today.AddMonths(-1);

            // Query the rental database for any rentals within the last month for the customer
            var rentals = await _context.CarRental
                .Where(r => r.RentalDate >= lastMonth && r.CustomerId == userId)
                .ToListAsync();

            var IsRegular = false;
            //If the customer has rental whithin the last month they are regular customer
            if (rentals.Any())
            {
                IsRegular = true;
            }

            return Ok(IsRegular);

        }

    }
}
