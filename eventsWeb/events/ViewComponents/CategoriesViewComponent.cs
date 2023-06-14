using eventsWeb.business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace events.ViewComponents
{
    public class CategoriesViewComponent:ViewComponent
    {
        private ICategoryService _categoryService;

        public CategoriesViewComponent(ICategoryService categoryService)
        {
            this._categoryService  = categoryService;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory=RouteData?.Values["id"];
            return View(_categoryService.GetAll());
        }        
    }
}