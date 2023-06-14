using System.Linq;
using events.Extensions;
using events.Identity;
using events.Models;
using eventsWeb.business.Abstract;
using eventsWeb.entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace events.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;

        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        private ICartService _cartService;

        
        public AdminController(IProductService productService, 
                ICategoryService categoryService, 
                RoleManager<IdentityRole> roleManager,
                UserManager<User> userManager,
                ICartService cartService)    
        {
            _productService = productService;
            _categoryService = categoryService;
            _roleManager = roleManager;
            _userManager = userManager;
            _cartService = cartService;
        }

       public IActionResult UserCreate()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> UserCreate(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed=true
            };
                //Bir kart objesi oluştur
                 _cartService.InitializeCard(user.Id);
                 
            var result = await _userManager.CreateAsync(user, model.Password); 
            if(result.Succeeded)
            {
                return RedirectToAction("UserList");
            }

            ModelState.AddModelError("","Bir hata oluştu, lütfen tekrar deneyin");
            ModelState.AddModelError("Password","En az 6 karakter olacak şekilde; büyük harf, küçük harf ve özel karakter giriniz");
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UserDelete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
                await _userManager.DeleteAsync(user);
            return RedirectToAction("UserList");
        }

        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user!=null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(i=>i.Name);

                ViewBag.Roles = roles;
                return View(new UserDetailsModel(){
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles
                });
            }
            return RedirectToAction("RoleList");
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model, string[] selectedRoles)
        {
          if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if(user!=null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirmed;

                    var result = await _userManager.UpdateAsync(user);

                    if(result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        selectedRoles = selectedRoles?? new string[]{};
                        await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles).ToArray<string>());
                        await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles).ToArray<string>());

                        return RedirectToAction("RoleList");
                    }
                }
                return RedirectToAction("RoleList");
            }

            return View(model);            
        }
 

        public IActionResult UserList()
        {

            return View(_userManager.Users);
        }


        //ROLE PAGES
        [HttpPost]
        public async Task<IActionResult> RoleDelete(string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId); 
                await _roleManager.DeleteAsync(role);
            return RedirectToAction("RoleList");

        }

        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach (var user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user,role.Name)
                                ?members:nonmembers;
                list.Add(user);
            }

            var model = new RoleDetails()
            {
                Role=role,
                Members=members,
                NonMembers=nonmembers
            };

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if(ModelState.IsValid)
            {
                //ADD TO ROLE
                //foreach içindeki son satır , eğer dizi nullsa boş bir dizi tanımlaması yapmak için
                foreach (var userId in model.IdsToAdd ?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user!=null)
                    {
                        var result = await _userManager.AddToRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                              foreach (var error in result.Errors)
                              { 
                                ModelState.AddModelError("", error.Description);  
                              }  
                        }
                    }
                }

                //REMOVE FROM ROLE
                foreach (var userId in model.IdsToDelete ?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user!=null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                              foreach (var error in result.Errors)
                              { 
                                ModelState.AddModelError("", error.Description);  
                              }  
                        }
                    }
                }
          

            }
            return Redirect("/admin/RoleEdit/"+model.RoleId);
        }

        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if(result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                       ModelState.AddModelError("",error.Description);
                       //tempData alternative 
                    }
                }

            }
            return View(model);
        }


        //PRODUCT PAGES

        public IActionResult ProductList()
        {
            var adminProducts = new ProductListViewModel()
            {
                Products = _productService.GetAll()
            };

            return View(adminProducts);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {

     
            return View();
        }

        [HttpPost]
        public async Task<  IActionResult> CreateProduct(ProductModel model, IFormFile file)
        {
            if(ModelState.IsValid)
            {
                var entity = new Product()
                {
                    Name=model.Name,
                    Price=model.Price,
                    Description=model.Description,
                    // ImgUrl=model.ImgUrl,
                    Location=model.Location,
                    sittingUrl=model.sittingUrl,
                    Rules = model.Rules,
                    eventDate=model.eventDate
                    };
                    if(file!=null)
                    {
                        var extention = Path.GetExtension(file.FileName);
                        var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                        entity.ImgUrl=randomName;
                        var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",randomName);

                        using(var stream = new FileStream(path,FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        } 
                    }
                
                _productService.Create(entity);

                TempData.Put("message", new AlertMessage()
                {
                    Title="Create Product",
                    Message = $"{entity.Name} isimli ürün eklendi.",
                    AlertType="success"

                });

                return RedirectToAction("ProductList");
            }
            return View(model);
            
        }

        [HttpGet]
        public IActionResult EditProduct(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
             
            // var entity = _productService.GetById((int)id);
            var entity = _productService.GetByIdWithCategories((int)id);

            if(id==null)
            {
                return NotFound();
            }

            var model = new ProductModel()
            {
                ProductId=entity.ProductId,
                Name=entity.Name,
                Price=entity.Price,
                Description=entity.Description,
                ImgUrl=entity.ImgUrl,
                Location=entity.Location,
                sittingUrl=entity.sittingUrl,
                Rules=entity.Rules,
                eventDate=entity.eventDate,
                SelectedCategories = entity.ProductCategories.Select(i=>i.Category).ToList()
            };

            ViewBag.Categories = _categoryService.GetAll();
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel model, int[] categoryIds, IFormFile file)
        {

            if(ModelState.IsValid)
            {

                var entity = _productService.GetById(model.ProductId);

                    if(entity==null)
                    {
                        return NotFound();
                    }

                        entity.Name = model.Name;
                        entity.Price = model.Price;
                        entity.Description=model.Description;
                        // entity.ImgUrl =model.ImgUrl;
                        entity.Location = model.Location;
                        entity.sittingUrl = model.sittingUrl;
                        entity.Rules=model.Rules;
                        entity.eventDate=model.eventDate;

                        if(file!=null)
                        {
                            var extention = Path.GetExtension(file.FileName);
                            var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                            //isim çakışmaması için random bir isim oluşturuyoruz
                            entity.ImgUrl=randomName;
                            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",randomName);
                            //resim aktarırken aynı isimdeki önceki resmi siler 
                            using(var stream = new FileStream(path,FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                        }

                        _productService.Update(entity,categoryIds);
                        

                    TempData.Put("message", new AlertMessage()
                    {
                        Title="Update Product",
                        Message=$"{model.Name} isimli ürün güncellendi.",
                        AlertType="success"

                    });

                    return RedirectToAction("ProductList");

            }
        
            ViewBag.Categories = _categoryService.GetAll();
            //burada bir linq sorgusuyla boş selectedcategories çek, döngüyü null referenceden kurtar
            return View(model);
 
        }

        [HttpPost]
        public IActionResult deleteproduct(int productId)
        {
            var entity = _productService.GetById(productId);
            
            if(entity!=null)
            {
                _productService.Delete(entity);
            }


            TempData.Put("message", new AlertMessage()
            {
                Title="Delet Product",
                Message=$"{entity.Name} isimli ürün silindi.",
                AlertType="danger"

            });


            return RedirectToAction("ProductList");
        }


        //CATEGORY PAGES
        public IActionResult CategoryList()
        {
            var adminCategories = new CategoryListViewModel()
            {
                Categories = _categoryService.GetAll()
            };

            return View(adminCategories);
        }


        [HttpGet]
        public IActionResult CreateCategory()
        {
     
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model)
        {
            if(ModelState.IsValid)
            {
                var entity = new Category()
                {
                    Name=model.Name
                };
                //burada  entity oluşturmamızın nedeni, sayfa modelini CategoryCreateModel yaptık ama Creta metodu Category nesnesi istiyor
                _categoryService.Create(entity);

   
                TempData.Put("message", new AlertMessage()
                {
                      Title=  "Create",
                      Message= $"{model.Name}  isimli kategori eklendi.",
                      AlertType = "success"
                });
                

                return RedirectToAction("CategoryList");
            }
            return View(model);

        }
        

        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
             
            var entity = _categoryService.GetByIdwithProducts((int)id);

            if(id==null)
            {
                return NotFound();
            }

            var model = new CategoryModel()
            {
                CategoryId=entity.CategoryId,
                Name=entity.Name,
                Products=entity.ProductCategories.Select(p=>p.Product).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryModel model)
        {
            if(ModelState.IsValid)
            {
                var entity = _categoryService.GetById(model.CategoryId);

                if(entity==null)
                {
                    return NotFound();
                }

                    entity.Name = model.Name;

                _categoryService.Update(entity);
                    
                // var msg = new AlertMessage()
                // {
                //     Message = $"{model.Name} isimli kategori güncellendi.",
                //     AlertType = "success"
                // };

                // TempData["message"]= JsonConvert.SerializeObject(msg);

                TempData.Put("message", new AlertMessage()
                {
                      Title=  "Güncelleme",
                      Message= $"{model.Name} isimli kategori güncellendi.",
                      AlertType = "success"
                });

                return RedirectToAction("CategoryList");
            }

            return View(model);
        }


        [HttpPost]
        public IActionResult deletecategory(int CategoryId)
        {
            var entity = _categoryService.GetById(CategoryId);
            
            if(entity!=null)
            {
                _categoryService.Delete(entity);
            }

            TempData.Put("message", new AlertMessage()
            {
                    Title=  "Delete Category",
                    Message= $"{entity.Name} isimli kategori silindi.",
                    AlertType = "danger"
            });


            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteFromCategory(int productId, int categoryId)
        {
            _categoryService.DeleteProductFromCategory( productId ,  categoryId);

            TempData.Put("message", new AlertMessage()
            {
                    Title=  "DeleteFromCategory",
                    Message= $"{productId} ID'li ürün {categoryId} ID'li kategoriden silindi.",
                    AlertType = "danger"
            });
            return Redirect("/admin/EditCategory/"+categoryId);
        }


    }
}