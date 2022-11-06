using IdentityApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityApp.ClaimProvider
{
    public class ClaimProvider : IClaimsTransformation
    {
        public UserManager<AppUser> _userManager { get; set; }

        public ClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            //Sadece kullancı üye olanlara eklememiz lazım . Bu yüzden kontrol ediyoruz.  Bu if içine giriyorsa kullanıcı üyedir.
            if (principal!= null && principal.Identity.IsAuthenticated)
            {
                ClaimsIdentity identity = principal.Identity as ClaimsIdentity; //Elimde artık identity Claimsleri var 

                var user = await _userManager.FindByNameAsync(identity.Name); //Kullanıcıyı bulmamız lazım 

                if (user.BirthDay != null)
                {
                    var today = DateTime.Today;
                    var age = today.Year - user.BirthDay?.Year;
                    if (age > 15)
                    {
                        Claim ViolanceClaim = new Claim("violance", true.ToString(), ClaimValueTypes.String, "Internal");     //City isimli bir claim ekliyoruz. Parametreler : Tipi,value,valuetype,ıssuer
                        identity.AddClaim(ViolanceClaim);
                    }

                }

                if (user != null)               //user var ise 
                {
                    if (user.City!=null)        // şehir bilgisi var ise 
                    {
                        if (!principal.HasClaim(c => c.Type == "City"))         //City isimli bir claim var mı yok mu bunu buluyor. Eğer böyle bir claim yoksa ekleyelim
                        {
                            Claim CityClaim = new Claim("City", user.City, ClaimValueTypes.String, "Internal");     //City isimli bir claim ekliyoruz. Parametreler : Tipi,value,valuetype,ıssuer
                            identity.AddClaim(CityClaim);
                        }
                    }
                }

            }

            return principal;

        }
    }
}
