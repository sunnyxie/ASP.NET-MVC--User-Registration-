using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Registration.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage ="FIeld Cannot be empty")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "FIeld Cannot be empty")]
        [DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage ="Password mis-match")]
        public string Confirm_Password { get; set; }

        [Required]
        public string ResetPasswordCode { get; set; }
    }
}