using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Extensions
{
    public static class BasketExtensions
    {
        public static BasketDto MapBasketToDto(this Basket basket)
        {
            return new BasketDto
            {
                Id = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    PictureUrl = item.Product.PictureUrl,
                    Type = item.Product.Type,
                    Brand = item.Product.Brand,
                    Quantity = item.Quantity
                }).ToList()
            };
        }

        public static BasketDto CombineBaskets(BasketDto userBasket, BasketDto anonBasket)
        {
            Console.WriteLine("Basket Extension CombineBasket called");
            for (int i = 0; i < anonBasket.Items.Count; i++)
            {
                for (int j = 0; j < userBasket.Items.Count; j++)
                {
                    if (anonBasket.Items[i].ProductId == userBasket.Items[j].ProductId)
                    {
                        userBasket.Items[j].Quantity += anonBasket.Items[i].Quantity;
                    }
                }
                userBasket.Items.Add(anonBasket.Items[i]);
            }
            return new BasketDto
            {
                Id = userBasket.Id,
                BuyerId = userBasket.BuyerId,
                Items = userBasket.Items
                // Items = userBasket.Items.Select(item => new BasketItemDto
                // {
                //     ProductId = item.ProductId,
                //     Name = item.Product.Name,
                //     Price = item.Product.Price,
                //     PictureUrl = item.Product.PictureUrl,
                //     Type = item.Product.Type,
                //     Brand = item.Product.Brand,
                //     Quantity = item.Quantity
                // }).ToList()
            };
        }
        
    }
}