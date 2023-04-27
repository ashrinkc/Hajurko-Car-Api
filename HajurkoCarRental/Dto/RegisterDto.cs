namespace HajurkoCarRental.Dto
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Additional properties for customer registration
        public string Phone { get; set; }
        public string Address { get; set; }
        //public IFormFile Document { get; set; }
    }
}
