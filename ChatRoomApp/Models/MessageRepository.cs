using ChatRoomApp.Models.Database;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ChatRoomApp.Models
{
    public class MessageRepository
    {
        public void AddMessage(Message message)
        {
            DbHelper.ExecuteNonQuery("INSERT INTO Messages (SenderName,Content,Timestamp) VALUES (@SenderName, @Content, @Timestamp)",
     "@SenderName", message.SenderName,"@Content", message.Content,"@Timestamp", message.Timestamp);
        }
        
        public IEnumerable<Message> GetMessages(){
            var messages = new List<Message>();
            var query = "SELECT SenderName, Content, Timestamp FROM Messages";
            var dataTable = DbHelper.ExecuteQuery(query);
            foreach (DataRow row in dataTable.Rows)

            {
                var message = new Message
                {
                    SenderName = row["SenderName"].ToString(),
                    Content = row["Content"].ToString(),
                    Timestamp = Convert.ToDateTime(row["Timestamp"])
                };
                messages.Add(message);
            }
            return messages;
        }

    }
}

