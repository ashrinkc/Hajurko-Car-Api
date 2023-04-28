namespace HajurkoCarRental.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string Description { get; set; }
        public decimal DailyRentalFee { get; set; }
        public bool IsAvailable { get; set; }
        public string Rating { get; set; }
        public string Color { get; set; }
        public string RegNumber { get; set; }
        public int RentCount { get; set; }
        public ICollection<Offer> Offers { get; set; }
    }
}
