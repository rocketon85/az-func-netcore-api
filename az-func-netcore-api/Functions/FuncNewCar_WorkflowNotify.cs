using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace az_func_netcore_api
{
    public static class FuncNewCar_WorkflowNotify
    {
        [FunctionName(nameof(FuncNewCar_WorkflowNotify_Run))]
        public static async Task<string> FuncNewCar_WorkflowNotify_Run(
            [OrchestrationTrigger] IDurableOrchestrationContext ctx)
        {
            dynamic input = ctx.GetInput<dynamic>();
            var output1 = await ctx.CallActivityAsync<string>(nameof(FuncNewCar_WorkflowNotify_Queue), input);
            var output2 = await ctx.CallActivityAsync<string>(nameof(FuncNewCar_WorkflowNotify_Mail), input);

            return "ok 2";
        }

        [FunctionName(nameof(FuncNewCar_WorkflowNotify_Queue))]
        public static string FuncNewCar_WorkflowNotify_Queue([ActivityTrigger] IDurableActivityContext input,
            [Queue("notifyqueue-items"), StorageAccount("ConnectionQueue")] ICollector<string> msg,
            ILogger log)
        {
            string data = JsonConvert.SerializeObject(input.GetInput<dynamic>());
            log.LogTrace("add message to queue {input}.", data);
            msg.Add(data );

            return $"{data}";
        }

        [FunctionName(nameof(FuncNewCar_WorkflowNotify_Mail))]
        public static string FuncNewCar_WorkflowNotify_Mail([ActivityTrigger] IDurableActivityContext input,
           ILogger log)
        {
            string data = JsonConvert.SerializeObject(input.GetInput<dynamic>());
            log.LogTrace("send mail message {input}.", data);

            return $"{data}";
        }
    }
}
