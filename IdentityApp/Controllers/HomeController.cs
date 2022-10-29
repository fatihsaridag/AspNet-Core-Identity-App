using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Login(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel userlogin)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(userlogin.Email);
                if (user !=null)
                {
                    //Böyle bir kullanıcı var ise bunun kilitli olup olmadıgını veri tabanından öğrenelim

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabınız bir süreliğine kilitlenmiştir. Lüfen daha sonra tekrar deneyiniz");
                        return View(userlogin);
                    }


                    await _signInManager.SignOutAsync();            //Bizim yazdıgımız önceden bir cooki varsa onu silsin
                    var result = await _signInManager.PasswordSignInAsync(user, userlogin.Password,userlogin.RememberMe,false);       //3. parametrede 60 gün cookiyi aktif hale getirdik. 4.Parametrede kullanıcıyı kitleyip kitlememe

                    if (result.Succeeded)
                    {
                        //Kullanıcı başarılıysa başarısız giriş sayısını sıfırlamamız lazım.
                        await _userManager.ResetAccessFailedCountAsync(user);
                        if (TempData["ReturnUrl"]!= null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "Member");
                    }
                    else //Eğer kullanıcı başarılı giriş yapamadıysa sayıyı arttıralım.
                    {
                        await _userManager.AccessFailedAsync(user);

                     

                        int fail = await _userManager.GetAccessFailedCountAsync(user);  // Bu userın kaç başarısız giriş yaptığına bakıyoruz.
                        ModelState.AddModelError("", $"{fail} kez başarısız giriş");
                        if (fail == 3)
                        {
                            await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(20)));   // Kullanıcıyı 20 dakika kilitledik.
                            ModelState.AddModelError("", "Hesabınız 3 başarısız girişten dolayı 20 dakika süreyle kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Email adresiniz veya şifreniz yanlış");
                        }

                    }
                }
                else
                {
                    ModelState.AddModelError("", "Bu email adresine kayıtlı kullanıcı bulunamamıştır.");
                }
            }

            return View(userlogin);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel userViewModel)
        {
            if (ModelState.IsValid) //UserViewModel de girilen tüm değerler geçerli o ihtiyaçları karşılıyor.
            {
                AppUser user = new AppUser();
                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.PhoneNumber = userViewModel.PhoneNumber;

               IdentityResult result =  await _userManager.CreateAsync(user, userViewModel.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var item in result.Errors)     //IdentityErrordan dönüyor.
                    {
                        ModelState.AddModelError("", item.Description);   //Yalnızca özet kısmı altında hatalarımızın gözükmesini istiyoruz o yüzden boş
                    }
                }

            }


            return View(userViewModel); //Hata varsa hataları da ekliyor.
        }


        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(PasswordResetViewModel passwordResetViewModel)
        {
            // Böyle bir kullanıcı var mı? Test edelim
            AppUser user = _userManager.FindByEmailAsync(passwordResetViewModel.Email).Result;
            // böyle bir kullanıcı varsa 
            if (user != null)
            {
                // Token Oluşturuyoruz
                string passwordResetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                // Linki Burada gönderiyoruz
                string passworResetLink = Url.Action("ResetPasswordConfirm", "Home", new
                {
                    userId = user.Id,
                    token = passwordResetToken
                }, HttpContext.Request.Scheme);

                // deneme.com/Home/ResetPasswordConfirm?userId=jdkjasbhdtoken=kdlksamdl

                Helper.PasswordReset.PasswordResetSendEmail(passworResetLink, user.Email);
                ViewBag.status = "success";

            }
            // hata varsa
            else
            {
                ModelState.AddModelError("", "Sistemde kayıtlı email adresi bulunamamıştır!!!");
            }


            return View(passwordResetViewModel);
        }



        public IActionResult ResetPasswordConfirm(string userId , string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("PasswordNew")]PasswordResetViewModel passwordResetViewModel)
        {
            var token = TempData["token"].ToString();
            var userId = TempData["userId"].ToString();

            var user = await _userManager.FindByIdAsync(userId);

            if (user!=null)
            {
                var result = await _userManager.ResetPasswordAsync(user,token,passwordResetViewModel.PasswordNew);
                if (result.Succeeded)
                {
                    // Kullanıcının securityStampini aktif edicez. Yeni bir security
                    await _userManager.UpdateSecurityStampAsync(user);  //Bunu yapmazsak kullanıcı eski şifreyle dolaşmaya devam eder. Bunu engellemek için
                    ViewBag.status = "success";
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Hata meydana gelmiştir. Lütfen daha sonra tekrar deneyiniz.");
            }
            return View(passwordResetViewModel);
        }

    }
}
