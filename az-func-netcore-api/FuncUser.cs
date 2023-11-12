using az_func_netcore_api.Helpers;
using az_func_netcore_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace az_func_netcore_api
{
    public static class FuncUser
    {

        //static async Task<TableResult> GetAll(CloudTable table, String InvocationName)
        //{
            
        //    TableResult x = await table.ExecuteAsync(TableOperation.Retrieve<User>(InvocationName, "rowkey1"));
        //    return x;
        //}

        static List<User> GetAllUsers()
        {
            TableQuery<User> query = new TableQuery<User>();
            var data = new TableHelper<User>().GetAllEntity("user", query);
            return data;
        }

        static User GetUser(int userId)
        {
            TableQuery<User> query = new TableQuery<User>().Where(
                TableQuery.GenerateFilterConditionForInt("UserId", QueryComparisons.Equal, userId)
            );
            var data = new TableHelper<User>().GetEntity("user", query);
            return data;
        }

        [FunctionName("func-netcore-api-getuserdetail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string detail;
            int userId = 0;

            if (!int.TryParse(req.Query["userid"], out userId))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                userId = data?.userId;
            }

            if (userId == 0) return new BadRequestObjectResult("Not parameter");
            var ent = GetUser(userId);
            detail = ent?.Detail;
            if (string.IsNullOrEmpty(detail)) return new BadRequestObjectResult("User not found");
            return new OkObjectResult(detail);

        }
    }
}
