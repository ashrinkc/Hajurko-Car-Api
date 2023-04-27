using Microsoft.AspNetCore.Identity;

namespace HajurkoCarRental.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
    }
}

public enum UserType
{
    Admin,
    Staff,
    Customer
}
