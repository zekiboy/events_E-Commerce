using events.Models;
using eventsWeb.business.Abstract;
using eventsWeb.entity;
using eventWeb.data.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace events.Controllers
{
    public class HomeController:Controller
    {
        private IProductService _productService;
        private IProductRepository _productRepository;

        public HomeController(IProductService productService, IProductRepository productRepository)
        {
            this._productService=productService;
            this._productRepository=productRepository;

        }
        

        public IActionResult index()
        {
                // GetProductsWithCategories metodunu deneme amaçlı yazdım
            var productsWithCategories = new ProductListViewModel()
            {
                Products= _productService.GetProductsWithCategories()
            };
            return View(productsWithCategories); 

            //detail cat çekme
            // Categories = product.ProductCategories.Select(i=>i.Category).ToList()

        
        }


        public IActionResult contact()
        {
            return View();
        }

        public IActionResult about()
        {
            return View();
        }  
    }
}