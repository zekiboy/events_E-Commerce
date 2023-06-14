using events.Models;
using eventsWeb.business.Abstract;
using eventsWeb.entity;
using Microsoft.AspNetCore.Mvc;

namespace events.Controllers
{
    public class EventsController:Controller
    {
        public IProductService _productService;

        public EventsController(IProductService productService)
        {
            this._productService=productService;
        }
 
        public IActionResult list(int? id, string q)
        {
            if(id!=null)
            {
                var productsByCtgId = new ProductListViewModel()
                {
                    Products=_productService.GetProductByCategoryId((int)id)
                };

               return View(productsByCtgId); 
            }
            else if(!string.IsNullOrEmpty(q))
            {
                var productBySearch= new ProductListViewModel()
                {
                    Products= _productService.GetProductsByQSearch(q)
                };
                return View(productBySearch);
            }
            else
            {
                var productListViewModel = new ProductListViewModel()
                {
                    Products=_productService.GetAll()
                };
                return View(productListViewModel);
            }
        }
        
        public IActionResult detail(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            Product product = _productService.GetProductDetails((int)id);
            //Detay sayfasına ürünün kategorilerini de yazdırdım.
            // Buyüzden GetById metodu yerine tabloları bağlayarak bu metodu kullandım
            
            if(product==null)
            {
                return NotFound();
            }

            return View(new ProductDetailModel{
                Product = product,
                Categories = product.ProductCategories.Select(i=>i.Category).ToList()
            });
        }        
    }
}