using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Corriere.Models
{
    public class User
    {
        public string Nickname { get; set; }
        public string Password { get; set; }  
        public string Role { get; set; }
    }





    public class UserRegistrationViewModel
    {
        
            [Required]
            [Display(Name = "Nickname")]
            public string Nickname { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
        

    }
}