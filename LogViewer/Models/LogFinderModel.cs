using System.ComponentModel.DataAnnotations;

namespace LogViewer.Models
{
    public class LogFinderModel
    {
        [Display(Name = "ID пользователя")]
        public int? UserID { get; set; }
        [Display(Name = "Тип действия")]
        public LogTypes.LogType Type { get; set; }
        [Display(Name = "Дата")]
        public DateTime? Date { get; set; }

        public static string ToStrDateFromUnixSeconds(long seconds)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(seconds);
            return dateTimeOffset.ToString("d");
        }
    }
    
}

