using System.ComponentModel.DataAnnotations;

namespace EvtapAz.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "İstifadəçi adı daxil edin")]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "Şifrə daxil edin")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Soyad daxil edin")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "İstifadəçi adı daxil edin")]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "Email daxil edin")]
        [EmailAddress(ErrorMessage = "Düzgün email daxil edin")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Telefon nömrəsi daxil edin")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Şifrə daxil edin")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifrə minimum 6 simvol olmalıdır")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Şifrəni təkrarlayın")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifrələr eyni deyil")]
        public string ConfirmPassword { get; set; } = "";
    }
}
