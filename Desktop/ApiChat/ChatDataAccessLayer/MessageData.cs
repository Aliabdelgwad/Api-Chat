using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ChatDataAccessLayer
{
    public class MessageDTO
    {
        public int MessageID { get; set; }
        public int CustomerID { get; set; }
        public int SupportUserID { get; set; }
        public string MessageContent { get; set; }
        public string SenderType { get; set; }
        public DateTime SentAt { get; set; }

    }

    public class MessageData
    {
       
        public static async Task AddMessage(MessageDTO message)
        {
            using (var connection = new SqlConnection(Config.ConnectionString))
            using (var command = new SqlCommand("INSERT INTO Messages (CustomerID,SupportUserID, MessageContent, SenderType) VALUES (@CustomerID,@SupportUserID, @MessageContent, @SenderType)", connection))
            {
                command.Parameters.AddWithValue("@CustomerID", message.CustomerID);
                command.Parameters.AddWithValue("@SupportUserID", message.SupportUserID);
                command.Parameters.AddWithValue("@MessageContent", message.MessageContent);
                command.Parameters.AddWithValue("@SenderType", message.SenderType);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                
            }
        }

        public static List<MessageDTO> GetMessagesByCustomerId(int customerId)
        {
            var messages = new List<MessageDTO>();


            using (var connection = new SqlConnection(Config.ConnectionString))
            using (var command = new SqlCommand("SELECT * FROM Messages WHERE CustomerID = @CustomerID ORDER BY SentAt", connection))
            {
                command.Parameters.AddWithValue("@CustomerID", customerId);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
 
                        var message = new MessageDTO
                        {
                          MessageID = (int)reader["MessageID"],
                          CustomerID = (int)reader["CustomerID"],
                          SupportUserID = (int)reader["SupportUserID"],
                          MessageContent = reader["MessageContent"].ToString(),
                          SenderType = reader["SenderType"].ToString(),
                          SentAt = reader.GetDateTime(reader.GetOrdinal("SentAt")),
                        };
                        messages.Add(message);
                    }
                }
                return messages;
            }
        }


    }


}

