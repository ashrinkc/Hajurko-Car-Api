using Microsoft.AspNetCore.Identity;

namespace HajurkoCarRental.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string UserType { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public byte[]? Document { get; set; }
        public bool? IsRegular { get; set; }
        public string? Role { get; set; }
    }
}

public enum UserType
{
    Admin,
    Staff,
    Customer
}
