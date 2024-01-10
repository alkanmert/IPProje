using System.ComponentModel.DataAnnotations;

namespace IPProje.ViewModels.Users
{
    public class UserRegisterModel
    {
        [Display(Name = "Adı")]
        [Required(ErrorMessage = "Adınızı Giriniz!")]
        public string FirstName { get; set; }

        [Display(Name = "Soyadı")]
        [Required(ErrorMessage = "Soyadıbızı Giriniz!")]
        public string LastName { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı Adı Giriniz!")]
        public string UserName { get; set; }

        [Display(Name = "E-Posta Adresi")]
        [Required(ErrorMessage = "E-Posta Adresi Giriniz!")]
        [EmailAddress(ErrorMessage = "Geçerli bir E-Posta Adresi Giriniz!")]
        public string Email { get; set; }

        [Display(Name = "Parola")]
        [Required(ErrorMessage = "Parola Giriniz!")]
        public string Password { get; set; }

        [Display(Name = "Parola Tekrar")]
        [Required(ErrorMessage = "Parola Tekrar Giriniz!")]
        [Compare("Password", ErrorMessage = "Parola Tekrarı Tutarsızdır!")]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Telefon Numarası")]
        [Required(ErrorMessage = "Telefon Numaranızı Giriniz!")]
        public string PhoneNumber { get; set; }
    }
}
