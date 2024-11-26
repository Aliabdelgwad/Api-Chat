using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatDataAccessLayer;
namespace ChatAPIBusinessLayer
{
    public class User
    {

        public static async Task<UserDTO> LoginAsync(string username, string password)
        {
            return await UserData.GetUserAsync(username, password);
        }

    }
}
