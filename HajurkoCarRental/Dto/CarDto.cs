namespace HajurkoCarRental.Dto
{
    public class CarDto
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string Description { get; set; }
        public decimal DailyRentalFee { get; set; }
        public bool IsAvailable { get; set; }
    }
}
