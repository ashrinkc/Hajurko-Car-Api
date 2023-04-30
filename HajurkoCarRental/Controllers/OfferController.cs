using HajurkoCarRental.Data;
using HajurkoCarRental.Dto;
using HajurkoCarRental.Models;
using HajurkoCarRental.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Data.Common;

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
            //Check if the car exists for which the offer is being made
            var car = await _context.Cars.FindAsync(offerdto.CarId);
            if (car == null)
            {
                return NotFound();
            }

            var offer = new Offer
            {
                CarId = offerdto.CarId,
                DiscountPercentage = offerdto.DiscountPercentage,
                Title = offerdto.Title,
                Description = offerdto.Description,
            };

            car.Offer = offerdto.DiscountPercentage;
            _context.Update(car);
            //Add offer and save
            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            //Send email to all the users 
            var users = await _context.AppUsers.ToListAsync();
            foreach (var user in users)
            {
                var email = new EmailSenderService();
                await email.SendEmailAsync(user.Email, offerdto.Title, offerdto.Description);
            }

            return Ok("Offer successfully added");
        }

        //Get ongoing offer for individual car 
        [HttpGet("ongoingOffer/{Carid}")]
        public async Task<IActionResult> GetOngoingOffer(int Carid)
        {
            //var offer = await _context.Offers
            //    .Include(o => o.Car)
            //    .FirstOrDefaultAsync(o => o.CarId == Carid);

            var offer = await _context.Cars
                .Where(o => o.Offer > 0 && o.Id == Carid)
                .FirstOrDefaultAsync();

            //Check if the offer is yet to be started or has already ended
            if (offer == null)
            {
                return NotFound();
            }

            var publishedOffer = new PublishedOffer
            {
                Id = offer.Id,
                CarBrand = offer.Brand,
                CarModel = offer.Model,
                DiscountPercentage = offer.Offer,
                Description = offer.Description,
            };

            return Ok(publishedOffer);

        }

        //Get all ongoing offers
        [HttpGet("ongoingOffers")]
        public async Task<IActionResult> GetOngoingOffers()
        {
            //var offers = await _context.Offers
            //    .Include(o => o.Car)
            //    .Where(o => o.StartDate < DateTime.Now && o.EndDate > DateTime.Now)
            //    .ToListAsync();

            var offers = await _context.Cars.Where(o => o.Offer > 0).ToListAsync();
            if(offers == null || !offers.Any())
            {
                return NotFound("No offers currently available");
            }

            var publishedOffer = offers.Select(offer => new PublishedOffer
            {
                CarBrand = offer.Brand,
                CarModel = offer.Model,
                DiscountPercentage = offer.Offer,
                Description = offer.Description
            }).ToList();

            return Ok(publishedOffer);
        }

        // Get all past, present and future offers
        [HttpGet("offers")]
        public async Task<IActionResult> GetOffers()
        {
            var offers = await _context.Offers.Include(c => c.Car).ToListAsync();
            if(offers == null)
            {
                return NotFound("No offers created");
            }

            var publishedOffers = offers.Select(offer => new PublishedOffer
            {
                CarBrand = offer.Car.Brand,
                CarModel = offer.Car.Model,
                DiscountPercentage = offer.DiscountPercentage,
                Title = offer.Title,
                Description = offer.Description
            }).ToList();
            //var options = new JsonSerializerOptions
            //{
            //    ReferenceHandler = ReferenceHandler.Preserve,
            //    MaxDepth = 64 // optional, increase the maximum depth if needed
            //};

            //var json = JsonSerializer.Serialize(publishedOffers, options);
            return Ok(publishedOffers);
        }
    }
}
