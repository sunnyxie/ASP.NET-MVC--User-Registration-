using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Registration.Models
{
    //i created 
    [MetadataType(typeof(UsersDBMetadata))]
    public partial class UsersDB
    {
        public string Confirm_Password { get; set; }
    }

    public class UsersDBMetadata
    {
        [Display(Name = "First Name: ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name Required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name: ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "last Name Required")]
        public string LastName { get; set; }

        [Display(Name = "Email Id: ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Id Required")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

        [Display(Name = "Date of Birth: ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date of Birth Required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateofBirth { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        //[MinLength(6, ErrorMessage = "Minimum 6 charcters required")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password is Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password donot match")]
        public string Confirm_Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "UserName is Required")]
        public string UserName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        public bool isEmailVerified { get; set; }
        public System.Guid ActivationCode { get; set; }
    }
}