using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace LogViewer.Models.DataBase
{
    public class Account
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указан логин"),
            StringLength(20, MinimumLength = 2, ErrorMessage = "Недопустимая длина (от 2 до 20 симв.)"),
            Display(Name = "Логин")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль"), 
            StringLength(25, MinimumLength = 6, ErrorMessage = "Недопустимая длина (от 6 до 25 симв.)"), 
            Display(Name = "Пароль"), Column(TypeName = "character varying(255)")]
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
        public long TokenExpirate { get; set; }
        public string? LastIp { get; set; }
    }
}
