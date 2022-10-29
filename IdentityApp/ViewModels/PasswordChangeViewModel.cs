using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.ViewModels
{
    public class PasswordChangeViewModel
    {
        //Eski Şifre, Yeni Şifre, Yeni Şifre Doğrulama
        [Display(Name="Eski Şifreniz")]
        [Required(ErrorMessage ="Eski Şifreniz gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(4,ErrorMessage ="Şifreniz en az 4 karakterli olmak zorundadır")]
        public string PasswordOld { get; set; }

        [Display(Name = "Yeni Şifreniz")]
        [Required(ErrorMessage = "Eski Şifreniz gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakterli olmak zorundadır")]
        public string PasswordNew { get; set; }

        [Display(Name = "Onay Yeni Şifreniz")]
        [Required(ErrorMessage = "Onay yeni şifreniz tekrar gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakterli olmak zorundadır")]
        [Compare("PasswordNew",ErrorMessage ="Yeni şifreniz ve onay şifreniz birbirinden farklıdır")]
        public string PasswordConfirm { get; set; }
    }
}
