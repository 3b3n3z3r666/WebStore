using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;
        public BasketController(StoreContext context)
        {
            _context = context;

        }

        [HttpGet(Name = "GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetrieveBasket(GetBuyerId());

            if (basket == null) return NotFound();
            return basket.MapBasketToDto();
        }



        [HttpPost]
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            var basket = await RetrieveBasket(GetBuyerId());
            if (basket == null) basket = CreateBasket();
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return BadRequest(new ProblemDetails { Title = "Product Not Found" });

            basket.AddItem(product, quantity);
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return CreatedAtRoute("GetBasket", basket.MapBasketToDto());

            return BadRequest(new ProblemDetails { Title = "Problem saving item to basket" });
        }



        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {
            // get basket
            var basket = await RetrieveBasket(GetBuyerId());
            if (basket == null)
            {
                return NotFound();
            }
            // remove item or reduce quantity
            basket.RemoveItem(productId, quantity);
            var result = await _context.SaveChangesAsync() > 0;
            return result ? Ok() : BadRequest(new ProblemDetails { Title = "Problem removing item from basket" });
        }


        private async Task<Basket> RetrieveBasket(string buyerId)
        {
            Console.WriteLine($"\n\n\nRetrieving basket - Buyer ID is: {buyerId}\n\n\n");
            Console.WriteLine($"\n\n\nGetBuyerId() is: {GetBuyerId()}\n\n\n");
            if (string.IsNullOrEmpty(buyerId))
            {
                Response.Cookies.Delete("buyerId");
                Console.WriteLine($"\n\n\nBuyer ID is empty! buyerId cookie will be deleted!\n\n\n");
            }
            return await _context.Baskets
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
        }

        private string GetBuyerId()
        {

            return User.Identity?.Name ?? Request.Cookies["buyerId"];
        }

        private Basket CreateBasket()
        {
            Console.WriteLine($"\n\n\nCreating Basket! - Buyer id will be set as: {User.Identity?.Name}\n\n\n");
            var buyerId = User.Identity?.Name;
            if (string.IsNullOrEmpty(buyerId))
            {

                buyerId = Guid.NewGuid().ToString();
                Console.WriteLine($"\n\n\nNo Username - New buyerID Guid is {buyerId}\n\n\n");
                var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
                Response.Cookies.Append("buyerId", buyerId, cookieOptions);
                Console.WriteLine($"\n\n\nNew Buyer ID: {buyerId} added to cookies!\n\n\n");
            }


            var basket = new Basket { BuyerId = buyerId };
            _context.Baskets.Add(basket);
            return basket;
        }
    }
}