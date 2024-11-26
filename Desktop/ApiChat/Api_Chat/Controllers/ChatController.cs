
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatDataAccessLayer;
using ChatAPIBusinessLayer;
using System.Data;
using System.Collections.Concurrent;


namespace Api_Chat.Controllers
{
    [Route("api/Chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] UserLogin user)
        {
            try
            {
                var loggedInUser = await ChatAPIBusinessLayer.User.LoginAsync(user.Username, user.Password);

                if (loggedInUser != null)
                {
                    return loggedInUser;
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("clients/{categoryId}")]
        public ActionResult<List<CustomerDTO>> GetCustomers(int categoryId)
        {
            var clients = ChatAPIBusinessLayer.Customer.GetCustomers(categoryId);
            return Ok(clients);
        }

        [HttpGet("categories")]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            var Category = ChatAPIBusinessLayer.Customer.GetCategories();
            return Ok(Category);
        }

        [HttpPost("send")]
        public async Task<ActionResult> SendMessage([FromBody] MessageDTO message)
        {
            await ChatAPIBusinessLayer.Messaga.SendMessageAsync(message);
            await _hubContext.Clients.User(message.CustomerID.ToString()).SendAsync("ReceiveMessage", message.MessageContent, message.CustomerID);
            return Ok();
        }

        [HttpGet("Oldmessages/{userId}")]
        public ActionResult<List<MessageDTO>> GetMessages(int userId)
        {
            var messages = ChatAPIBusinessLayer.Messaga.GetMessages(userId);
            return messages;
        }


        [HttpPost("query")]
        public async Task<IActionResult> ExecuteQuery([FromBody] QueryRequestDTO request)
        {
            try
            {
                QueryResult result = await QueryRequest.QueryExecutor(request);

                if (result.IsDataTable)
                {
                    return Ok(result.Data); 
                }
                else
                {
                    return Ok(new { AffectedRows = result.AffectedRows });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    

}

    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, List<string>> userConnections = new ConcurrentDictionary<string, List<string>>();
     
        public override Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext().Request.Query["userId"].ToString();

            if (!userConnections.ContainsKey(userId))
            {
                userConnections[userId] = new List<string>();
            }
            userConnections[userId].Add(Context.ConnectionId);

            Clients.All.SendAsync("UpdateClientStatus", userId, true); 


            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.GetHttpContext().Request.Query["userId"].ToString();
            if (userConnections.ContainsKey(userId))
            {
                userConnections[userId].Remove(Context.ConnectionId);
            }

            Clients.All.SendAsync("UpdateClientStatus", userId, false); 


            return base.OnDisconnectedAsync(exception);
        }
        // إرسال إشعار لجميع الأجهزة عند الضغط على الزر
        public async Task NotifyButtonUpdate(int userId, int clientId)
        {
            var userId1 = userId.ToString();
            var clientId1 = clientId.ToString();

            if (userConnections.ContainsKey(userId1))
            {
                var connectionIds = userConnections[userId1];
                foreach (var connectionId in connectionIds)
                {
                    await Clients.Client(connectionId).SendAsync("UpdateButtonState", clientId);
                }
            }
        }
        public async Task SendMessageToUser(ChatMessaga chatMessaga)
        {
            string senderId = chatMessaga.UserId.ToString(); 
            string receiverId = chatMessaga.SupportId.ToString(); 

            if (userConnections.TryGetValue(receiverId, out List<string> receiverConnectionIds))
            {
                foreach (var connectionId in receiverConnectionIds)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", chatMessaga);
                }
            }

            if (userConnections.TryGetValue(senderId, out List<string> senderConnectionIds))
            {
                foreach (var connectionId in senderConnectionIds)
                {
                    if (connectionId != Context.ConnectionId) 
                    {
                        await Clients.Client(connectionId).SendAsync("UpdateSentMessage", chatMessaga);
                    }
                }
            }
        }

      
    }

    public class ChatMessaga
    {
        public int UserId { get; set; }
        public int SupportId { get; set; }
        public string UserName { get; set; }
        public string MessageContent { get; set; }
    }

}



