﻿namespace HajurkoCarRental.Dto
{
    public class OfferDto
    {
        public int CarId { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}