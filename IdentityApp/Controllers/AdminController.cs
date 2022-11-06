using IdentityApp.Models;
using IdentityApp.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : base(userManager,null,roleManager)
        {
        }

        public IActionResult Index()
        {
         
            return View();
        }


        public IActionResult Claims()
        {
            return View(User.Claims.ToList());
        }


        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RoleCreate(RoleViewModel roleViewModel)
        {
            var role = new AppRole();
            role.Name = roleViewModel.Name;
            var result = _roleManager.CreateAsync(role).Result;
            if (result.Succeeded)
            {
                return RedirectToAction("Roles");
            }
            else
            {
                AddModelError(result);
            }
            return View(roleViewModel);
        }


        public IActionResult Roles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        public IActionResult Users()
        {
            return View(_userManager.Users.ToList());
        }

        public async Task<IActionResult> RoleDelete(string id)
        {

            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
               await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Roles");
        }

        [HttpGet]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var userViewModel = role.Adapt<RoleViewModel>();
                return View(userViewModel);
            }
            return RedirectToAction("Roles");
        }

        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleViewModel roleViewModel)
        {
            var role =  await _roleManager.FindByIdAsync(roleViewModel.Id);
            if (role != null)
            {
                role.Name = roleViewModel.Name;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Güncelleme işlemi başarısız oldu");
            }
            return View(roleViewModel);

        }

        [HttpGet]
        public async Task<IActionResult> RoleAssign(string id)
        {
            TempData["userId"] = id;
            var user = await _userManager.FindByIdAsync(id);
            ViewBag.userName = user.UserName;

            var roles =  _roleManager.Roles;        //Rolleri çektik
            var userRoles = await _userManager.GetRolesAsync(user) as List<string>;  //Bu kullanıcı hangi rollere saghip bunu List<string> olarak bize dönecek.


            var roleAssignViewModels = new List<RoleAssignViewModel>();

            foreach (var role in roles)
            {
                RoleAssignViewModel r = new RoleAssignViewModel();
                r.RoleId = role.Id;
                r.RoleName = role.Name;
                if (userRoles.Contains(role.Name))
                {
                    r.Exist = true;
                }
                else
                {
                    r.Exist = false;
                }
                roleAssignViewModels.Add(r);
            }

            return View(roleAssignViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignViewModel> roleAssignViewModels)
        {
            var user = await _userManager.FindByIdAsync(TempData["userId"].ToString());
            foreach(var item in roleAssignViewModels)
            {
                if (item.Exist)
                {
                    await _userManager.AddToRoleAsync(user,item.RoleName);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, item.RoleName);
                }
            }

            return RedirectToAction("Users");
        }
    }
}
