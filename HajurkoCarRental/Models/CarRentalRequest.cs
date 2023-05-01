namespace HajurkoCarRental.Models
{
    public class CarRentalRequest
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int UserId { get; set; }
        public DateTime? RentalStart { get;set; }
        public DateTime? RentalEnd { get;set; }
        public decimal Charge { get; set; }
        public bool IsApproved { get; set; } = false;
        public string? Status { get; set; } = "Pending";
        public bool Cancelled { get; set; } = false;
        public Car Car { get; set; }
        public AppUser User { get; set; }

    }
}
