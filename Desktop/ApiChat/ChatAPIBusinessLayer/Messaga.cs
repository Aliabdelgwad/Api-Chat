using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatDataAccessLayer;

namespace ChatAPIBusinessLayer
{
    public class Messaga
    {
       
        public static async Task SendMessageAsync(MessageDTO message)
        {
            await MessageData.AddMessage(message);
        }

        public static List<MessageDTO> GetMessages(int customerId)
        {
            return MessageData.GetMessagesByCustomerId(customerId);
        }
    }
}
