using IdentityApp.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.ViewModels
{
    //Kullanıcı kayıt işlemleri yapacağız.
    public class UserViewModel
    {
        [Required(ErrorMessage = "Kullanıcı İsmi gereklidir.")]
        [Display(Name ="Kullanıcı Adı")]
        public string UserName { get; set; }

        [Display(Name = "Telefon Numarası")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage ="Email Adresi Gereklidir.")]
        [Display(Name = "Email Adresiiz")]
        [EmailAddress(ErrorMessage ="Email adresiniz doğru formatta değil.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifreniz Gereklidir.")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Şehir")]
        public string City { get; set; }


        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }


        public string Picture { get; set; }

        [Display(Name = "Cinsiyet")]
        public Gender Gender { get; set; }


    }
}
