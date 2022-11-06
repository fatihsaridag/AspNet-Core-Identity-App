using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.CustomTagHelpers
{
    [HtmlTargetElement("td",Attributes="user-roles")]
    public class UserRolesName : TagHelper
    {
        public UserManager<AppUser>  UserManager { get; set; }
        [HtmlAttributeName("user-roles")]
        public string UserId { get; set; }



        public UserRolesName(UserManager<AppUser> userManager)
        {
            this.UserManager = userManager;
        }



        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = await UserManager.FindByIdAsync(UserId);
            var roles = await UserManager.GetRolesAsync(user);

            string html = string.Empty;
            roles.ToList().ForEach(x =>
            {

                html += $"<span class = 'badge text-info'>{x}</span>";

            });

            output.Content.SetHtmlContent(html);
        }


    }
}
