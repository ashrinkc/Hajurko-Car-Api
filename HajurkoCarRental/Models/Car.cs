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
        public byte[]? Image { get; set; }
        public string RegNumber { get; set; }
        public string Engine { get; set; }
        public string Mileage { get; set; }
        public string Seating { get; set; }
        public string Fuel { get; set; }
        public string Power { get; set; }
        public string Torque { get; set; }
        public int? Offer { get; set; } = 0;
        public int? RentCount { get; set; }
        public ICollection<Offer> Offers { get; set; }
    }
}
