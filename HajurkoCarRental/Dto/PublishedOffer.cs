﻿namespace HajurkoCarRental.Dto
{
    public class PublishedOffer
    {
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
