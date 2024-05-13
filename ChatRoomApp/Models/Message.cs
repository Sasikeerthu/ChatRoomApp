namespace ChatRoomApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
