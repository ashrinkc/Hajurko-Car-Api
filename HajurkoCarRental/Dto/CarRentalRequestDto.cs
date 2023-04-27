namespace HajurkoCarRental.Dto
{
    public class CarRentalRequestDto
    {
        public int CarId { get; set; }
        public int UserId { get; set; }
        public DateTime RentalStart { get; set; }
        public DateTime RentalEnd { get; set; }
    }
}
