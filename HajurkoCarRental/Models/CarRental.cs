namespace HajurkoCarRental.Models
{
    public class CarRental
    {
        public int Id { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime RentalEnd { get; set; }
        public decimal Charge { get; set; }
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public int StaffId { get; set; }

        public Car Car { get; set; }
        public AppUser Customer { get; set; }
        public AppUser Staff { get; set; }
    }
}
