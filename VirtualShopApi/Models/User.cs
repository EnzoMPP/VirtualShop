using System.ComponentModel.DataAnnotations;

namespace LojaVirtualAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter exatamente 11 números.")]
        public string? CPF { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public bool IsAdmin { get; set; }
    }
}