namespace HajurkoCarRental.Dto
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? UserType { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public byte[] Document { get; set; }
        public bool? IsRegular { get; set; }
    }
}
