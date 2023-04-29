namespace HajurkoCarRental.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public int DiscountPercentage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Car Car { get; set; }
    }
}
