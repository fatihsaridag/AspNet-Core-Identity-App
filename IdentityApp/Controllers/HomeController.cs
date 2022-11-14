using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityApp.Controllers
{
    public class HomeController : BaseController
    {
  
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(userManager,signInManager)
        {
         
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Member");
            }
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

                    if (_userManager.IsEmailConfirmedAsync(user).Result == false)
                    {
                        ModelState.AddModelError("", "Email Adresini onaylanmamıştır. Lütfen epostanızı kontrol edin ");
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
                    string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string link = Url.Action("ConfirmEmail", "Home", new
                    {
                        userId = user.Id,
                        token = confirmationToken
                    },protocol:HttpContext.Request.Scheme);
                    Helper.EmailConfirmation.SendEmail(link, user.Email);

                    return RedirectToAction("Login");
                }
                else
                {
                    AddModelError(result);
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
            AppUser user = CurrentUser;
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
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Hata meydana gelmiştir. Lütfen daha sonra tekrar deneyiniz.");
            }
            return View(passwordResetViewModel);
        }


        public async Task<IActionResult> ConfirmEmail(string userId , string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            IdentityResult result = await _userManager.ConfirmEmailAsync(user,token);

            if (result.Succeeded)
            {
                ViewBag.status = "Email adresiniz onaylanmıştır.Login ekranından giriş yapabilirsiniz.";
            }
            else
            {
                ViewBag.status = "Bir hata meydana geldi lütfen daha sonra tekrar deneyiniz";
            }
            return View();
        }

        //Kullanıcıyı otomatik olarak facebook sayfasına yönlendiricez
        public IActionResult FacebookLogin(string returnUrl)   
        {
            //Urlimizin işlevi : Kullanıcının facebooktaki işlemleri bitti ve giriş yaptıktan sonraki url olacak.
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = returnUrl }); //ExternalResponse Actionu , Home Controllerı , ReturnUrl (Döneceği sayfayı belirttik)
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", RedirectUrl);  //Hangi sosyal medya ile bağlanmaya çalışıyoruz , Nereye dönücez
            return new ChallengeResult("Facebook", properties); //ChallangeResult => Ne alırsa oraya yönlendirir. hangi değere gidecek içinde döneceği yer de var. 
        }
        

        //FacebookLogin sayfasının dönüşü hangi actiona gelicek onu tasarlıyoruz (Kullanıcı kaydetme işlemleri vs)
        public async Task<IActionResult> ExternalResponse(string returnUrl="/") //ReturnUrl bize tekrar geliyor FacebookLoginden , Default olarak ReturnUrl yoksa anasayfaya yönlendirsin 
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();                                  //Bu login ile kullanıcının bilgilerine ihtiyacımız var ve alıyoruz(Kullancının login oldugu bilgiler)
            if (info == null) //Kullanıcı login sayfasında bilgilerini girmezse boş gelicek                            // Mesela LoginProvider var . Kullanıcı facebookla login olmuşsa facebook yazacak. Google ile login olmuşsa google yazıcak .
            {                                                                                                          //ProviderKey ise kullanıcı facebook ile üye olduysa facebook userId si gelicek.
                return RedirectToAction("LogIn");   //Eğer boş ise login ekranına yönlendir.
            }
            else  //Eğer dolu ise giriş yaptıracağız.
            {
               Microsoft.AspNetCore.Identity.SignInResult result =await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);  //1.Parametre LoginProvider , 2. ProviderKey , 3.isPersistent(startup cookie değeri)
                if (result.Succeeded)      //eğer veritabanında dolu loginProvider, ve ProviderKey kayıtlı ise login olacak. Kullanıcı daha önce facebook ile giriş yapmış 
                {
                    return Redirect(returnUrl);         //Bu urle yönlendiirelim.
                }
                else                       //Eğer bu değerler yok ise kullanıcı ilk kez giriş yapıyor ise veritabanına kaydetmemiz gerekiyor
                {
                    AppUser user = new AppUser();       //Yeni bir kullanıcı oluşturduk
                    user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;                         //Claimlerden Email değerini alıyoruz ve kullanıcı email içine atıyoruz.
                    string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;     //Claimlerden userId yi alıyoruz
                    if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))                           //Kullanıcın facebookdaki adı ve soyadından bir kullanıcı adı oluşturacağız. Eğer kullanıcının ismi var ise 
                    {
                        string userName = info.Principal.FindFirst(ClaimTypes.Name).Value;      //Claimden Kullanıcı adını aldık . (Fatih Sarıdağ)
                        userName = userName.Replace(' ', '-').ToLower() + ExternalUserId.Substring(0,5).ToString();        //Bu userNamede boşlukları sildik küçük harf yaptık ve + olarak UserId nin ilk 5 rakamını ekledik
                        user.UserName = userName;
                    }
                    else
                    {
                        user.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;       //Diyelim ki sistemde bir hata meydana geldi kullanıcının emailini username olarak verdik
                    }
                    IdentityResult createResult = await _userManager.CreateAsync(user);         //Burada kullanıcı kaydetme işlemini gerçekleştiriyoruz.

                    if (createResult.Succeeded)                                                 //Eğer başarılı bir şekilde oluşmuş ise 
                    {
                        //Eğer bunu doldurmazsak Identity Api bizim facebooktan giriş yaptığımızı anlayamaz.
                       IdentityResult loginResult =   await _userManager.AddLoginAsync(user, info); //Veritabanındaki AspNetUserLogins tablosunu dolduruyoruz. Parametre 1 . User, 2.Login(infodan gelicek) 
                        if (loginResult.Succeeded)      //Eğer loginResult success ise  
                        {//
                            await _signInManager.SignInAsync(user, true);   // Kullanıcıyı giriş yaptırıyoruz. 2. Paraemtre Cooki değeri true diyoruz
                            return Redirect(returnUrl);                     //Return Url i dönüyoruz.
                        }
                        else
                        {
                            AddModelError(loginResult);                    
                        }
                    }
                    else
                    {
                        AddModelError(createResult);
                    }
                }
            }
            return RedirectToAction("Error");
        }
    }
}
