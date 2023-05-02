namespace HajurkoCarRental.Dto
{
    public class changePasswordDto
    {
        public int? userId { get; set; }
        public string? oldPassword { get; set; }
        public string? newPassword { get; set; }
    }
}
