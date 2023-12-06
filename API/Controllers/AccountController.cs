using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;
        private readonly StoreContext _context;

        public AccountController(UserManager<User> userManager, TokenService tokenService, StoreContext context)
        {
            _context = context;
            _tokenService = tokenService;
            _userManager = userManager;
        }




        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Unauthorized();

            var userBasket = await RetrieveBasket(loginDto.Username);
            var anonBasket = await RetrieveBasket(Request.Cookies["buyerId"]);
            BasketDto returnBasket;

            //items in anon shopper basket
            if (anonBasket != null)
            {
                //items saved in user's basket
                if (userBasket != null)
                {
                    //add anon items to userbasket
                    foreach (var anonItem in anonBasket.Items)
                    {
                        Console.WriteLine("\n\n\nAdding anonItems to userBasket\n\n\n");
                        userBasket.AddItem(anonItem.Product, anonItem.Quantity);
                        Response.Cookies.Delete("buyerId");
                        await _context.SaveChangesAsync();
                    }
                    returnBasket = userBasket.MapBasketToDto();

                }
                //userbasket is null so transfer anon basket to user
                else
                {
                    Console.WriteLine("\n\n\n Transfering anonBasket to user\n\n\n");
                    returnBasket = anonBasket.MapBasketToDto();
                    // _context.Baskets.Remove(userBasket);
                    anonBasket.BuyerId = user.UserName;
                    Response.Cookies.Delete("buyerId");
                    await _context.SaveChangesAsync();
                }

            }
            //anonbasket and user basket are both empty to return empty basket
            else
            {
                returnBasket = anonBasket?.MapBasketToDto();
                await _context.SaveChangesAsync();

            }

            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.GenerateToken(user),
                Basket = returnBasket
            };
        }



        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var user = new User { UserName = registerDto.Username, Email = registerDto.Email };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);

                }
                return ValidationProblem();
            }
            await _userManager.AddToRoleAsync(user, "Member");

            return StatusCode(201);
        }





        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userBasket = await RetrieveBasket(User.Identity.Name);

            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.GenerateToken(user),
                Basket = userBasket?.MapBasketToDto()
            };
        }


        [Authorize]
        [HttpGet("savedAddress")]
        public async Task<ActionResult<UserAddress>> GetSavedAddress()
        {
            Console.WriteLine($"\n\n\nGet Saved Address for {User.Identity.Name}");
            // UserAddress address = await _userManager.Users.Where(x => x.UserName == User.Identity.Name).Select(user => user.Address).FirstOrDefaultAsync();
            // Console.WriteLine($"\n\n\nAddress {address.FullName} {address.Address1} {address.Address2} {address.City} {address.State} {address.Zip} {address.Country} \n\n\n");
            return await _userManager.Users
                .Where(x => x.UserName == User.Identity.Name)
                .Select(user => user.Address)
                .FirstOrDefaultAsync();
        }

        private async Task<Basket> RetrieveBasket(string buyerId)
        {
            Console.WriteLine("\n\n\nAccountController RetrieveBasket Method\n\n\n");
            if (string.IsNullOrEmpty(buyerId))
            {
                Console.WriteLine("\n\n\nAccountController retrieveBasket - Deleting buyerId cookie from response\n\n\n");

                Response.Cookies.Delete("buyerId");
                return null;
            }
            return await _context.Baskets
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
        }


        public async Task<BasketDto> AddAnonItemsToUserBasket(LoginDto loginDto)
        {
            var anonBasket = await RetrieveBasket(Request.Cookies["buyerId"]);
            var userBasket = await RetrieveBasket(loginDto.Username);
            foreach (var anonItem in anonBasket.Items)
            {
                userBasket.AddItem(anonItem.Product, anonItem.Quantity);
                await _context.SaveChangesAsync();
            }
            return userBasket.MapBasketToDto();
        }
    }
}