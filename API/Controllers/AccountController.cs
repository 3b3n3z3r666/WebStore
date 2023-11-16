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

            if ((anonBasket == null || anonBasket.Items.Count < 1) && userBasket != null)
            {
                //logged out shopper has nothing in cart
                //logging in should get the user basket
                Console.WriteLine("\n\n\n\nAnon basket is empty at login\n\n\n\n");
                Console.WriteLine("\n\n\n\n User Basket is not empty - Will fetch user basket \n\n\n\n");
                Response.Cookies.Delete("buyerId");
                returnBasket = userBasket.MapBasketToDto();
                await _context.SaveChangesAsync();
            }
            else if (anonBasket?.Items?.Count > 0 && userBasket?.Items?.Count > 0)
            {
                //logged out shopper has items in cart and has a user cart
                Console.WriteLine("\n\n\n\nUser and Anon have baskets at login\n\n\n\n");
                Response.Cookies.Delete("buyerId");
                foreach (var anonItem in anonBasket.Items)
                {
                    userBasket.AddItem(anonItem.Product, anonItem.Quantity);
                    await _context.SaveChangesAsync();
                }
                returnBasket = userBasket.MapBasketToDto();
            }
            else
            {
                // logged out shopper has no user basket
                Console.WriteLine("\n\n\n\nNo user basket at login\n\n\n\n");

                if (_context.Baskets.Any())
                {
                    Console.WriteLine("\n\n\nThere are baskets\n\n\n");
                    // _context.Baskets.Remove(userBasket);
                }
                Console.WriteLine($"loginDto.Username is {loginDto.Username}");
                // anonBasket.BuyerId = loginDto.Username;
                // Response.Cookies.Delete("buyerId");
                // returnBasket = anonBasket.MapBasketToDto();
                // Console.WriteLine($"Return Basket is {returnBasket}");
                await _context.SaveChangesAsync();
                return new UserDto
                {
                    Email = user.Email,
                    Token = await _tokenService.GenerateToken(user),
                    Basket = anonBasket?.MapBasketToDto()
                };
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
            if (string.IsNullOrEmpty(buyerId))
            {
                Response.Cookies.Delete("buyerId");
            }
            return await _context.Baskets
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
        }
    }
}