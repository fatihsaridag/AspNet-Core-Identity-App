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
    public class AdminController : BaseController
    {


        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : base(userManager,null,roleManager)
        {
        }

        public IActionResult Index()
        {
         
            return View();
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
    }
}
