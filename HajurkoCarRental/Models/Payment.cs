namespace HajurkoCarRental.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int CarRentalId { get; set; }
        public int CustomerId {get; set;}

        public CarRental CarRental { get; set; }
        public AppUser Customer { get; set; }
    }
}
