using events.Identity;
using events.Models;
using eventsWeb.business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace events.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private ICartService _cartService;
        private UserManager<User> _userManager;
        public CartController(ICartService cartService, UserManager<User> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));

            var model = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i=>new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    Name=i.Product.Name,
                    Price= (double)i.Product.Price,
                    ImgUrl = i.Product.ImgUrl,
                    Quantity=i.Quantity
                }).ToList()
            };

            return View(model);
        } 

        [HttpPost]
         public IActionResult AddToCart(int productId, int  quantity)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.AddToCart(userId, productId, quantity);

            return RedirectToAction("Index");
        }     

        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);   
            _cartService.DeleteFromCart(userId,productId);
            return RedirectToAction("Index");
        }    
    }
}