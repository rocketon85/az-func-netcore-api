using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace az_func_netcore_api
{
    public static class FuncNewCar_WorkflowStorage
    {
        [FunctionName(nameof(FuncNewCar_WorkflowStorage_Run))]
        public static async Task<string> FuncNewCar_WorkflowStorage_Run(
            [OrchestrationTrigger] IDurableOrchestrationContext ctx)
        {

            string input = ctx.GetInput<string>();
            var output1 = await ctx.CallActivityAsync<string>(nameof(FuncNewCar_WorkflowStorage_Table), input);
            var output2 = await ctx.CallActivityAsync<string>(nameof(FuncNewCar_WorkflowStorage_Redis), output1);

            return "ok 1";
        }

        [FunctionName(nameof(FuncNewCar_WorkflowStorage_Table))]
        public static string FuncNewCar_WorkflowStorage_Table([ActivityTrigger] string name,
          ILogger log)
        {
            log.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }

        [FunctionName(nameof(FuncNewCar_WorkflowStorage_Redis))]
        public static string FuncNewCar_WorkflowStorage_Redis([ActivityTrigger] string name,
          ILogger log)
        {
            log.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }
    }
}
