namespace HajurkoCarRental.Dto
{
    public class PaymentDto
    {
        public string? CarModel { get; set; }
        public string? CarBrand { get; set; }
        public string? Customer { get; set; }
        public decimal? RepairCost { get; set; }
        public decimal? TotalAmountPaid { get; set; }
    }
}
