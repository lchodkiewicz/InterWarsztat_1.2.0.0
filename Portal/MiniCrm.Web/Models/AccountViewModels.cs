using System;
using System.ComponentModel.DataAnnotations;

namespace MiniCrm.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }



    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Typ użytkownika")]
        public string UserType { get; set; }

        
    }
    public class RegisterCustomerViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Addres")]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Województwo")]
        public string Region { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Kod pocztowy")]
        public string PostalCode { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Kraj")]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Fax")]
        public string Fax { get; set; }
    }

    public class MyRegisterViewModelForCustomer
    {
        public RegisterViewModel LoginData { get; set; }
        public RegisterCustomerViewModel CustomerData { get; set; }
    }

    public class RegisterEmployeeViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data urodzenia")]
        public DateTime BirthDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data zatrudnienia")]
        public DateTime HirehDate { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Addres")]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Województwo")]
        public string Region { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Kod pocztowy")]
        public string PostalCode { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Kraj")]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Telefon")]
        public string HomePhone { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Notatka")]
        public string Notes { get; set; }

      
        [DataType(DataType.Text)]
        [Display(Name = "Notatka")]
        public string PhotoPath { get; set; }
    }
    public class MyRegisterViewModelForEmployee
    {
        public RegisterViewModel LoginData { get; set; }
        public RegisterEmployeeViewModel EmployeeData { get; set; }
    }
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
