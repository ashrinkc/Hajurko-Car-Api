namespace HajurkoCarRental.Dto
{
    public class RentalHistoryDto
    {
        public int RentalId { get; set; }
        public string CustomerName { get; set; }
        public string StaffName { get; set; }
        public decimal? RentalFee { get; set; }
        public DateTime? RentalStart { get; set; }
        public DateTime? RentalEnd { get; set; }
    }
}
