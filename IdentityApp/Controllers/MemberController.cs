﻿using IdentityApp.Enums;
using IdentityApp.Models;
using IdentityApp.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        public MemberController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager): base(userManager,signInManager)
        {

        }

        public IActionResult Index()
        {
            var user = CurrentUser;//Veritabanından userNameyi aldık.
            var userViewModel = user.Adapt<UserViewModel>();
            return View(userViewModel);
        }

        [HttpGet]
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = CurrentUser;       //Bu name bilgisini cookieden okuyor.

                //Şifresini kontrol edelim eski girmiş oldugu şifre doğru mu ?
                var exist = _userManager.CheckPasswordAsync(user, passwordChangeViewModel.PasswordOld).Result;
                if (exist)
                {
                    var result = _userManager.ChangePasswordAsync(user, passwordChangeViewModel.PasswordOld, passwordChangeViewModel.PasswordOld).Result;
                    if (result.Succeeded)
                    {
                        _userManager.UpdateSecurityStampAsync(user);    //30 dakika süreyle Identity api cookiemizdeki değerleri kontrol ediyor. Doğru olup olmadıgına bakıyor.
                        _signInManager.SignOutAsync();
                        _signInManager.PasswordSignInAsync(user, passwordChangeViewModel.PasswordNew, true, false); //Otomatik olarak çıkış yaptırıp giriş yaptırdık . Cookie bilgimiz güncel
                        ViewBag.success = "true";

                    }
                    else
                    {
                        AddModelError(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Eski şifreniz yanlış");
                }
            }

            return View(passwordChangeViewModel);
        }


        public IActionResult UserEdit()
        {
            var user = CurrentUser;
            var userViewModel = user.Adapt<UserViewModel>();
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel userViewModel,IFormFile userPicture)
        {
            ModelState.Remove("Password");
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            if (ModelState.IsValid)
            {

                var user = CurrentUser;


                if (userPicture != null && userPicture.Length>0)
                {
                    //Rastgele dosya ismi belirttik ve onun ardına extension yapıştırdık.
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserPicture", fileName);
                    using(var stream = new FileStream(path, FileMode.Create))
                    {
                        await userPicture.CopyToAsync(stream);

                        user.Picture = "/UserPicture/" + fileName;

                    }

                }

                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.PhoneNumber = userViewModel.PhoneNumber;
                user.City = userViewModel.City;
                user.BirthDay = userViewModel.BirthDay;
                user.Gender = (int)userViewModel.Gender;    //Bay ise 1 , bayan ise 2

                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    //Username sini değiştiriyorsak securityStampini değiştirmemiz gerekiyor.
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(user, true);

                    ViewBag.success = "true";

                }
                else
                {
                    AddModelError(result);
                }

            }
            return View(userViewModel);
        }

        public void Logout()
        {
            _signInManager.SignOutAsync();
        } 


        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
