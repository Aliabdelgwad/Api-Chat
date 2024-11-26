using ChatDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAPIBusinessLayer
{
    public class Customer
    {

        public static List<CustomerDTO> GetCustomers(int categoryId)
        {
            return CustomerData.GetCustomers(categoryId);
        }

        public static IEnumerable<Category> GetCategories()
        {
            return CustomerData.GetCategories();
        }
    }
}
