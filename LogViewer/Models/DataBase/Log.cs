namespace LogViewer.Models.DataBase
{
    public class Log
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public long CreatedAt { get; set; }
        public string CreatedAtDateString { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
