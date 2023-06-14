using events.EmailServices;
using events.Identity;
using eventsWeb.business.Abstract;
using eventsWeb.business.Concrete;
using eventWeb.data.Abstract;
using eventWeb.data.Concrete.EfCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace events
{
    public class StartUp
    {

        private IConfiguration _configuration;
        public StartUp(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options=> options.UseSqlite("DataSource=eventsDb"));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
            //kullanıcıyı her sayfa geçişinde login sayfasına göndermemesi için, tarayıcısına cookie gönderiyoruz
            //TokenProviders parola resetleme işlemleri için benzersizbir id oluşturmak için kullanıyoruz
            
            services.Configure<IdentityOptions>(options=>{
                //PASSWORD
                options.Password.RequireDigit= true;
                //mutlaka sayıfal bir değer girmeli
                options.Password.RequireLowercase=true;
                //paralo içerisinde mutlaka küçük harf olmak zorunda
                options.Password.RequireUppercase=true;
                options.Password.RequiredLength=6;
                options.Password.RequireNonAlphanumeric=true;

                //LOCKOUT
                options.Lockout.MaxFailedAccessAttempts=5;
                //parolayı max 5 defe yanlış girebilir
                options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(5);
                //5dk sonra paraloya girmeye devam edebilir

                // options.User.AllowedUserNameCharacters="";
                options.User.RequireUniqueEmail=true;
                //her kullanıcının ayrı bi mail adresi olmalı mı
                options.SignIn.RequireConfirmedEmail=true;
                //onay maili
                options.SignIn.RequireConfirmedPhoneNumber=false;

            });

            services.ConfigureApplicationCookie(option=>{
                option.LoginPath = "/account/login";
                option.LogoutPath="/account/logout";
                //kullanıcının yetkisi olmayan sayfalara erişimi engeller
                option.AccessDeniedPath= "/account/accessdenied";
                //her işlem yaptıktan sonra logout olmak için 20dk var,
                    //false ise işlemin önemi yok, loginden sonra 20 dk
                option.SlidingExpiration = true;
                //login olduktan sonra 365 günün var
                option.ExpireTimeSpan = TimeSpan.FromDays(365);

                option.Cookie= new CookieBuilder
                {
                    //sadece http talebi alabilsin
                    HttpOnly = true,
                    //tarayıcısı cookiesine varsayılan haricinde bir isim vermek için
                    Name = ".Events.Security.Cookie",
                    SameSite = SameSiteMode.Strict
                };
            }); 


            //IProductRepository çağırıldığın EFCoreProductRepository getir
            services.AddScoped<IProductRepository, EfCoreProductRepository>();
            services.AddScoped<IProductService,ProductManager>();
            
            services.AddScoped<ICategoryRepository, EFCoreCategoryRepository>();
            services.AddScoped<ICategoryService,CategoryManager>();

            services.AddScoped<ICartRepository, EfCoreCartRepository>();
            services.AddScoped<ICartService,CartManager>();

            services.AddScoped<IEmailSender,SmtpEmailSender>(i=>
                new SmtpEmailSender(
                    _configuration["EmailSender:Host"],
                    _configuration.GetValue<int>("EmailSender:Port"),
                    _configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    _configuration["EmailSender:UserName"],
                    _configuration["EmailSender:Password"])
                );

            services.AddControllersWithViews();

        }
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
             app.UseStaticFiles();

            if(env.IsDevelopment())
            {
                // SeedDatabase.Seed();
                //test verilerini aldığı komut

                app.UseDeveloperExceptionPage();
                //projede hata varsa gördüğümüz sayfa, canlıya aldığımızda kullanıcı bu uyarıyı da görmeyecek 404 görecek
        
            }

            app.UseAuthentication();

            app.UseRouting();
            //UseRouting: site içerisinde yönlendirme kullanılacağını belirten komut

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {//UseEndpoints içi sitenin yolunu belirteceğimiz
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });


            SeedIdentity.Seed(userManager,roleManager,configuration).Wait();   
        }        
    }
}