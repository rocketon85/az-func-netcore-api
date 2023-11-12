using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using az_func_netcore_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace az_func_netcore_api
{
    public static class FuncNewCar
    {

        [FunctionName("func-netcore-api-newcar")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            dynamic input = context.GetInput<dynamic>();
            //string input = $"Hello {usu.UserId}"; // context.GetInput<string>();
            var stage1Task = context.CallSubOrchestratorAsync<string>(nameof(FuncNewCar_WorkflowNotify.FuncNewCar_WorkflowNotify_Run), input);
            var stage2Task = context.CallSubOrchestratorAsync<string>(nameof(FuncNewCar_WorkflowStorage.FuncNewCar_WorkflowStorage_Run), input);

            await Task.WhenAll(stage1Task, stage2Task);

            return new List<string>() { stage1Task.Result, stage2Task.Result };
        }

        [FunctionName("func-netcore-api-newcar_HttpStart")]
        public static async Task<IActionResult> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            
            dynamic data = 0;
            //if (!int.TryParse(req.Query["userid"], out data))
            //{
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                data = JsonConvert.DeserializeObject(requestBody);
            //}

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync<dynamic>("func-netcore-api-newcar", data);
            
            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}