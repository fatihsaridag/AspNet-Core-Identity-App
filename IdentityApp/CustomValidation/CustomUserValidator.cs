using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.CustomValidation
{
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            //Kullanıcı username'ini girerken bir sayısal karakter ile başlamasın.
            string[] Digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            foreach (var item in  Digits)
            {
                if (user.UserName[0].ToString() == item)        //Usernamenin 0.indeksi bizim itemlerdan birisiyle eşleşiyor mu eşleşmiyor mu.
                {
                    errors.Add(new IdentityError()
                    {
                        Code = "UserNameContainsFirstLetterDigitContains",
                        Description = "Kullanıcı adının ilk karakteri sayısal karakter içeremez"
                    });
                }
                 
            }
            if (errors.Count == 0)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
        }
    }
}
