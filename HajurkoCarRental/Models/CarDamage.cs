namespace HajurkoCarRental.Models
{
    public class CarDamage
    {
        public int Id { get; set; }
        public int CarRentalRequestId { get; set; }
        public string Description { get; set; }
        public decimal? RepairCost { get; set; }
        public bool IsPaid { get; set; }

        public CarRentalRequest CarRentalRequest { get; set; }
    }
}
