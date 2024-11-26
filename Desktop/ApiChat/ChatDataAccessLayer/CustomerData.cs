using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDataAccessLayer
{
    public class CustomerDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
    }

    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }

    public class CustomerData
    {

        public static List<CustomerDTO> GetCustomers(int categoryId)
        {
            var clients = new List<CustomerDTO>();
            string query = "SELECT UserID, Username FROM Users  JOIN Customers  ON Users.CustomerID = Customers.CustomerID " +
                "WHERE Users.UserType = 'Customer' AND Customers.CategoryID = @CategoryID";

            if (categoryId == 0) { query = "SELECT UserID, Username FROM Users WHERE UserType = 'Customer'"; }
            using (var connection = new SqlConnection(Config.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CategoryID", categoryId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var Client = new CustomerDTO
                        {
                            UserID = (int)reader["UserID"],
                            Username = reader["Username"].ToString(),
                        };
                        clients.Add(Client);
                    }

                }

            }
            return clients;
        }

        public static IEnumerable<Category> GetCategories()
        {
            var categories = new List<Category>();

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                var command = new SqlCommand("SELECT * FROM Categories ", connection);
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var category = new Category
                    {
                        CategoryID = (int)reader["CategoryID"],
                        CategoryName = reader["CategoryName"].ToString()
                    };
                    categories.Add(category);
                }
            }
            return categories;


        }


        public static async Task<string> GetConnectionStringByIdAsync(int customerId)
        {
            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT ConnectionString FROM Customers WHERE CustomerID = @CustomerID", connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", customerId);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader["ConnectionString"].ToString();
                        }
                    }
                }

                return null;
            }
        }

    }
}
