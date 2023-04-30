namespace HajurkoCarRental.Dto
{
    public class PublishedOffer
    {
        public int Id { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? DiscountPercentage { get; set; }
    }
}
