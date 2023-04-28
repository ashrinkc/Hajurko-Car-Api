namespace HajurkoCarRental.Models
{
    public class CarDamage
    {
        public int Id { get; set; }
        public int CarRentalId { get; set; }
        public string Description { get; set; }
        public decimal? RepairCost { get; set; }
        public decimal? TotalAmountPaid { get; set; }
        public bool? IsPaid { get; set; }
        public DateTime? DateOfDamage { get; set; }
        public CarRental CarRental { get; set; }
    }
}
