using ChatDataAccessLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAPIBusinessLayer
{
   

    public class QueryRequest
    {
        public static async Task<QueryResult> QueryExecutor(QueryRequestDTO request)
        {
            return await QueryRequestData.ExecuteQueryAsync(request);
        }
     
    }
}
