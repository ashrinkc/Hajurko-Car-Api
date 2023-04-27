using HajurkoCarRental.Data;
using HajurkoCarRental.Dto;
using HajurkoCarRental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;

namespace HajurkoCarRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly DataContext _context;

        public OfferController(DataContext context)
        {
            _context = context;
        }

        //Publish offer for a car
        [HttpPost]
        public async Task<IActionResult> PublishOffer([FromBody] OfferDto offerdto)
        {
            var car = await _context.Cars.FindAsync(offerdto.CarId);
            if(car == null)
            {
                return NotFound();
            }

            var offer = new Offer
            {
                CarId = offerdto.CarId,
                DiscountPercentage = offerdto.DiscountPercentage,
                StartDate = offerdto.StartDate,
                EndDate = offerdto.EndDate
            };

            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            return Ok("Offer successfully added");
        }

        //Get offer for individual car 
        [HttpGet("{Carid}")]
        public async Task<IActionResult> GetOffer(int Carid)
        {
            var offer = await _context.Offers
                .Include(o => o.Car)
                .FirstOrDefaultAsync(o => o.CarId == Carid);

            if(offer == null)
            {
                return NotFound();
            }

            if(offer.StartDate > DateTime.Now || offer.EndDate < DateTime.Now)
            {
                return NotFound("The offer is not currently available");
            }

            var publishedOffer = new PublishedOffer
            {
                CarBrand = offer.Car.Brand,
                CarModel = offer.Car.Model,
                DiscountPercentage = offer.DiscountPercentage,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate
            };

            return Ok(publishedOffer);
      
        }

        //Get all ongoing offers
        [HttpGet]
        public async Task<IActionResult> GetOffers()
        {
            var offers = await _context.Offers
                .Include(o => o.Car)
                .Where(o => o.StartDate < DateTime.Now && o.EndDate > DateTime.Now)
                .ToListAsync();

            if(offers == null || !offers.Any())
            {
                return NotFound("No offers currently available");
            }

            var publishedOffer = offers.Select(offer => new PublishedOffer
            {
                CarBrand = offer.Car.Brand,
                CarModel = offer.Car.Model,
                DiscountPercentage = offer.DiscountPercentage,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate
            }).ToList();

            return Ok(publishedOffer);
        }
    }
}
