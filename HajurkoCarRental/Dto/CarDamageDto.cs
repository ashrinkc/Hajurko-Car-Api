﻿namespace HajurkoCarRental.Dto
{
    public class CarDamageDto
    {
        public int Id { get; set; }
        public int? CarId { get; set; }
        public string CarModel { get; set; }
        public string CarBrand { get; set; }
        public string Customer { get; set; }
        public string? CustomerEmail { get; set; }
        public string Description { get; set; }
    }
}
