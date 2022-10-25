﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name ="Email Adresiniz")]
        [Required(ErrorMessage ="Email alanı gereklidir")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Şifreniz")]
        [Required(ErrorMessage = "Şifre alanı gereklidir")] 
        [DataType(DataType.Password)]
        [MinLength(4,ErrorMessage ="şifreniz en az 4 karakterli olmalıdır")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
