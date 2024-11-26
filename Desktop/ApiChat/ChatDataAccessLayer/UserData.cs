
 using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
using System.Resources;

 namespace ChatDataAccessLayer
 {
    public  class UserDTO
     {
       
            public int Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
          
            public string UserType { get; set; }
      }
    public class UserLogin
     {
            public string Username { get; set; }
            public string Password { get; set; }
     }

    public class UserData
    {
          
        public static async Task<UserDTO> GetUserAsync(string username, string password)
        {

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE Username = @Username AND Password = @Password", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var user = new UserDTO();

                            user.Id = (int)reader["UserID"];
                            user.Username = reader["Username"].ToString();
                            user.Password = reader["Password"].ToString();
                            user.UserType = reader["UserType"].ToString();


                            return user;
                        }
                    }

                }

                return null;
            }
        }

     
      


    }
}
  

