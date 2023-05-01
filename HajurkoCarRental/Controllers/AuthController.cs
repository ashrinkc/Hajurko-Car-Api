using HajurkoCarRental.Data;
using HajurkoCarRental.Dto;
using HajurkoCarRental.Models;
using HajurkoCarRental.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace HajurkoCarRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;

        public AuthController(DataContext context)
        {
            _context = context;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var ExistingUser = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (ExistingUser != null)
            {
                return BadRequest("A user with this email already exists");
            }
 
            var user = new AppUser
            {
                Email = model.Email,
                FullName = model.FullName,
                UserType = model.UserType,
                Address = model.Address,
                Phone = model.Phone
            };

            //hash the password before saving it
            var passwordHasher = new PasswordHasher<AppUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

            // Upload document for customer registration
            if (model.Document != null)
            {
                // Handle document upload
                // Check if file type is pdf or png
                //var allowedTypes = new[] { ".pdf", ".png" };
                //var extension = Path.GetExtension(model.Document).ToLower();
                //if (!allowedTypes.Contains(extension))
                //{
                //    return BadRequest("Invalid file type. Please upload a pdf or png file.");
                //}

                // Check if file size is less than or equal to 1.5MB
                if (model.Document.Length > 1572864) // 1.5MB in bytes
                {
                    return BadRequest("File size should not exceed 1.5MB.");
                }

                // Handle document upload
                user.Document = model.Document;

            }

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registered Successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            //get user from database
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == model.Email);
            if(user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            //compare hashed password
            var passwordHasher = new PasswordHasher<AppUser>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password");
            }
            
           //Generate token
           var tokenService = new Token();
           var token = tokenService.GenerateToken(user);
            return Ok(new 
            {
                id = user.Id,
                email = user.Email,
                userType = user.UserType,
                fullName = user.FullName,
                token = token,
            });
        }


    }
}
