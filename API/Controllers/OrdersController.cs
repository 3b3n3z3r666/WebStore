using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Entities.OrderAggregate;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly StoreContext _context;
        public OrdersController(StoreContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetOrders()
        {
            return await _context.Orders
                .ProjectOrderToOrderDto()
                .Where(x => x.BuyerId == User.Identity.Name)
                .ToListAsync();

        }

        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            return await _context.Orders
                .ProjectOrderToOrderDto()
                .Where(x => x.BuyerId == User.Identity.Name && x.Id == id)
                .FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateOrder(CreateOrderDto orderDto)
        {
            Console.WriteLine("CreateOrder Called\n\n");
            var basket = await _context.Baskets
                .RetrieveBasketWithItems(User.Identity.Name)
                .FirstOrDefaultAsync();

            Console.WriteLine("Got Basket\n\n");

            if (basket == null) return BadRequest(new ProblemDetails { Title = "Could not locate basket" });

            var items = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                var productItem = await _context.Products.FindAsync(item.ProductId);
                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = productItem.Id,
                    Name = productItem.Name,
                    PictureUrl = productItem.PictureUrl
                };
                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity

                };
                items.Add(orderItem);
                productItem.QuantityInStock -= item.Quantity;
            }

            Console.WriteLine("ProductItemOrdered created\n\n");
            Console.WriteLine("OrderItem created\n\n");

            var subtotal = items.Sum(item => item.Price * item.Quantity);
            var deliveryFee = subtotal > 1000 ? 0 : 500;

            Console.WriteLine("Fees calculated\n\n");

            var order = new Order
            {
                OrderItems = items,
                BuyerId = User.Identity.Name,
                ShippingAddress = orderDto.ShippingAddress,
                Subtotal = subtotal,
                DeliveryFee = deliveryFee,
                PaymentIntentId = basket.PaymentIntentId
            };
            Console.WriteLine("Order instance created\n\n");
            _context.Orders.Add(order);
            Console.WriteLine("Order added to storeContext\n\n");
            _context.Baskets.Remove(basket);
            Console.WriteLine("Basket Removed from storeContext\n\n");

            if (orderDto.SaveAddress)
            {
                Console.WriteLine("Address will be saved\n\n");
                var user = await _context.Users
                .Include(a => a.Address)
                .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                var address = new UserAddress
                {
                    FullName = orderDto.ShippingAddress.FullName,
                    Address1 = orderDto.ShippingAddress.Address1,
                    Address2 = orderDto.ShippingAddress.Address2,
                    City = orderDto.ShippingAddress.City,
                    State = orderDto.ShippingAddress.State,
                    Zip = orderDto.ShippingAddress.Zip,
                    Country = orderDto.ShippingAddress.Country,
                };
                user.Address = address;
                // Console.WriteLine("user.Address: ", user.Address);
                _context.Update(user);

            }else{
                Console.WriteLine("Address will notbe saved\n\n");
            }
            
            var result = await _context.SaveChangesAsync() > 0;
            Console.WriteLine($"Store Context Changes saved. Result is: {result}\n\n");
            if (result) return CreatedAtRoute("GetOrder", new { id = order.Id }, order.Id);

            return BadRequest("Problem creating order");
        }
    }
}