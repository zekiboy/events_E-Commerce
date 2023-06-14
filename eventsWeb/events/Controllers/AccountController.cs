using events.EmailServices;
using events.Extensions;
using events.Identity;
using events.Models;
using eventsWeb.business.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace events.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;
        private ICartService _cartService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender, ICartService cartService )
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _emailSender = emailSender;
            _cartService = cartService;

        }
        

        public IActionResult Login(string ReturnUrl=null)
        {
            //kullanıcı başka bir sayfaya tıklayıp logine yönlendirildiğinde,
                // giriş yaptıktan sonra, istediği sayfaya ReturnUrl ile ile geri göndereceğiz
                //bu bilgiyi hidden input olarak tutacağız

            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> Login( LoginModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if(user==null)
            {
                ModelState.AddModelError("","Bu kullanıcı adı ile daha önce bir hesap oluşturulmamış");
                return View(model);
            }

            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("","Lütfen email hesabınıza gelen link ile üyeliğinizi onaylayın");
                return View(model);
            }


            var result = await _signInManager.PasswordSignInAsync(model.UserName,model.Password,false,false);

            if(result.Succeeded)
            {
                return Redirect(model.ReturnUrl??"~/");
            }
            
            ModelState.AddModelError("","Girilen kullanıcı adı veya parola yanlış");
            return View(model);
        }


        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
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
                Email = model.Email
                
            };

            var result = await _userManager.CreateAsync(user, model.Password); 
            if(result.Succeeded)
            {
                //bu satır program çalıştığında bir üye oluşturulması için
                // await userManager.AddToRoleAsync(user, "customer");

                //generate token 
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail","Account", new{
                    userId =user.Id,
                    token = code,
            
                });
                
                // email
                //mail gönderirken hata veriyor. appsetting içerisinde kendi mail adresimi tanımlamam ve
                //gmail smtp ayalarını yapmam lazım onun yerine şimdilik console kullan
                await _emailSender.SendEmailAsync(model.Email,"Hesabınızı onaylayınız.",$"Lütfen email hesabınızı onaylamak için linke <a href='http://localhost:5045{url}'>tıklayınız.</a>");                return RedirectToAction("Login","Account");
            }

            ModelState.AddModelError("","Bir hata oluştu, lütfen tekrar deneyin");
            ModelState.AddModelError("Password","En az 6 karakter olacak şekilde; büyük harf, küçük harf ve özel karakter giriniz");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            //her yapıya artık bu şekilde tempdata ekleyebiliriz, ekstra metodlara ihtiyacımız yok
            TempData.Put("message", new AlertMessage()
            {
                Title=  "Oturum Kapatıldı",
                Message= "Hesabınız güvenli bir şekilde kapatıldı",
                AlertType = "warning"
            });

            return Redirect("~/");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId==null || token ==null)
            {
                TempData.Put("message", new AlertMessage()
                {
                      Title=  "Geçersiz token.",
                      Message= "Geçersiz token.",
                      AlertType = "danger"
                });

                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user!=null)
            {
                var result = await _userManager.ConfirmEmailAsync(user,token);
                if(result.Succeeded)
                {

                    //Bir kart objesi oluştur
                    _cartService.InitializeCard(user.Id);
                    
                    TempData.Put("message", new AlertMessage()
                    {
                        Title=  "Hesabınız onaylandı",
                        Message= "Hesabınız onaylandı",
                        AlertType = "success"
                    });

                    return View();
                }
            }

            TempData.Put("message", new AlertMessage()
            {
                Title=  "Hesabınız onaylanmadı",
                Message= "Hesabınız onaylanmadı",
                AlertType = "warning"
            });
            return View();

        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if(string.IsNullOrEmpty(Email))
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            
            if(user==null)
            {
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            //Generate Token
            var url = Url.Action("ResetPassword","Account", new{
                userId =user.Id,
                token = code,
        
            });
            
            // Email
            // Console.WriteLine(url);
            await _emailSender.SendEmailAsync(Email,"Reset Password.",$"Parolanızı yenilemek için linke <a href='http://localhost:5045{url}'>tıklayınız.</a>");


            return View();
        }

        public IActionResult ResetPassword(string userId, string token)
        {

            if(userId==null || token==null)
            {
                return RedirectToAction("Home","Index");
            }

            var model = new ResetPasswordModel {Token=token};

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user==null)
            {
                return RedirectToAction("Home","Index");
            }

            var result = await _userManager.ResetPasswordAsync(user,model.Token ,model.Password );

            if(result.Succeeded)
            {
                return RedirectToAction("Login","Account");
            }

            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        
    }
}