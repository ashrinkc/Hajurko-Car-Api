using HajurkoCarRental.Data;
using HajurkoCarRental.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HajurkoCarRental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Payment : ControllerBase
    {
        private readonly DataContext _context;

        public Payment(DataContext context)
        {
            _context = context;
        }

        //Send payment bill
        [HttpPost("paymentBill/{damageId}")]
        public async Task<IActionResult> GeneratePayment(int damageId, PaymentDto payment)
        {
            var carDamage = await _context.CarDamages.FindAsync(damageId);
            carDamage.RepairCost = payment.RepairCost;
            carDamage.TotalAmountPaid = payment.TotalAmountPaid;
            //var paymentBill = new 
            //{
            //    RepairCost = payment.RepairCost,
            //    TotalAmountPaid = payment.TotalAmountPaid,
            //    CarBrand = carDamage.CarRental.Car.Brand,
            //    CarModel = carDamage.CarRental.Car.Model,
            //    Customer = carDamage.CarRental.Customer.FullName,
            //};
            _context.CarDamages.Update(carDamage);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //Get payment bill
        [HttpGet("bill/{customerId}")]
        public async Task<IActionResult> GetPaymentBill(int customerId)
        {
            var carDamage = await _context.CarDamages
                .Include(cd => cd.CarRental)
                .ThenInclude(cr => cr.Customer)
                .Where(cd => cd.CarRental.Customer.Id == customerId)
                .ToListAsync();

            if (carDamage == null)
            {
                return NotFound();
            }

            var paymentBills = new List<dynamic>();

            foreach (var car in carDamage)
            {
                var paymentBill = new 
                {
                    RepairCost = car.RepairCost,
                    TotalAmountPaid = car.TotalAmountPaid,
                    CarBrand = car.CarRental?.Car?.Brand,
                    CarModel = car.CarRental?.Car?.Model,
                    //Customer = car.CarRental.Customer.FullName,
                };
                paymentBills.Add(paymentBill);
            }
            return Ok(paymentBills);

        }

        //Pay due amount
        [HttpPost("payment/{damageId}")]
        public async Task<IActionResult> amountPayment(int damageId, [FromBody] decimal amount)
        {
            var damage = await _context.CarDamages.FindAsync(damageId);
            if (damage.IsPaid == true)
            {
                return BadRequest("The amount has already been paid");
            }
            if(amount > damage.RepairCost)
            {
                return BadRequest("Higer payment than required");
            }
            damage.TotalAmountPaid += amount;
            if(damage.TotalAmountPaid == damage.RepairCost)
            {
                damage.IsPaid = true;
            }
            _context.CarDamages.Update(damage);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //Payment history (track of paid and unpaid amount)
        [HttpGet("paymentHistory")]
        public async Task<IActionResult> paymentHistory()
        {
            var damages = await _context.CarDamages
                .Include(cd => cd.CarRental)
                .ThenInclude(cr => cr.Customer)
                .ToListAsync();

            var result = damages.Select(damage => new
            {
                CustomerName = damage.CarRental.Customer.FullName,
                RepairCost = damage.RepairCost,
                TotalAmountPaid = damage.TotalAmountPaid,
                UnpaidAmount = damage.RepairCost - damage.TotalAmountPaid
            });

            return Ok(result);
        }

    }
}
