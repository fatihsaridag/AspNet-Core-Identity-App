using IdentityApp.CustomValidation;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddTransient<IAuthorizationHandler, ExpireDateExchangeHandler>();
            services.AddDbContext<AppIdentityDbContext>(opts =>
            {
                opts.UseSqlServer(Configuration["ConnectionStrings:DefaultConnectionString"]);
            });


            services.AddAuthorization(opts =>
           {
               opts.AddPolicy("AnkaraPolicy", policy =>
                {
                    policy.RequireClaim("city","Ankara"); // Şu kısıtlamaya sahip kısmı zorunlu olması gerek
                });

               opts.AddPolicy("ViolancePolicy", policy =>
              {
                  policy.RequireClaim("violance");          //Kullanıcı bu claime sahipse 15 yaşından büyüktür anlamına geliyor.
              });

               opts.AddPolicy("ExchangePolicy", policy =>
               {
                   policy.AddRequirements(new ExpireDateExchangeRequirement());
               });

           });


            services.AddIdentity<AppUser, AppRole>(opts => {
                opts.User.RequireUniqueEmail = true;
                opts.User.AllowedUserNameCharacters = "abcçdefghıijklmnoöpqrsştuüvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";


                opts.Password.RequiredLength = 4;   //En az 4 karakter uzunlugunda şifre
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false; //Zorla küçük karakter girmesini istemiyoruz.
                opts.Password.RequireUppercase = false; // Kullanıcıdan zorla uppercase girmesini istemiyoruz.
                opts.Password.RequireDigit = false;

            }).AddPasswordValidator<CustomPasswordValidator>().AddUserValidator<CustomUserValidator>().AddErrorDescriber<CustomIdentityErrorDescriber>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();


            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Home/Login");  //Ben kullanýcý giriþi yapmadan admin areaya eriþmek istersek sistem otomatik olarak bu sayfaya yönlendiriyor olacak.
                options.LogoutPath = new PathString("/Member/Logout");
                options.AccessDeniedPath = new PathString("/Member/AccessDenied");
                options.Cookie = new CookieBuilder
                {
                    //Cookie ayarlarýs
                    Name = "BlogProjem",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = System.TimeSpan.FromDays(7);
            });
            services.AddMvc();
            services.AddScoped<IClaimsTransformation, ClaimProvider.ClaimProvider>();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
