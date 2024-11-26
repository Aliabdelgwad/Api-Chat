using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ChatDataAccessLayer
{
    public class QueryRequestDTO
    {
        public string Query { get; set; }
        public int CustomerId { get; set; }
    }

    public class QueryResult
    {
        public List<Dictionary<string, object>> Data { get; set; } 
        public int AffectedRows { get; set; }
        public bool IsDataTable { get; set; }
    }

    public class QueryRequestData
    {
        public static async Task<QueryResult> ExecuteQueryAsync(QueryRequestDTO request)
        {
            string ConnectionString = await CustomerData.GetConnectionStringByIdAsync(request.CustomerId); 

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(request.Query, connection))
                {
                    if (request.Query.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        var listResult = ConvertDataTableToList(dataTable);

                        return new QueryResult
                        {
                            Data = listResult, 
                            IsDataTable = true
                        };
                    }
                    else
                    {
                        int affectedRows = await command.ExecuteNonQueryAsync();
                        return new QueryResult
                        {
                            AffectedRows = affectedRows,
                            IsDataTable = false
                        };
                    }
                }
            }
        }

        public static List<Dictionary<string, object>> ConvertDataTableToList(DataTable dataTable)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in dataTable.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    dict[column.ColumnName] = row[column];
                }
                list.Add(dict);
            }

            return list;
        }
    }
}
