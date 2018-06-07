
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace ACECoordination.PatientDemographics
{
    public static class PatientDemographics
    {

        public class Msg
        {
            public string first { get; set; }
            public string last { get; set; }
        }


        [FunctionName("PatientDemographics")]
        public static async Task<object> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = null, WebHookType ="Generic")]
            HttpRequestMessage req,
            TraceWriter log,
            [Queue("%outputQueueName%", Connection = "outputQueueConnection")] IAsyncCollector<Msg> outputQueue
            )

        {
            log.Info($"Webhook was triggered!");

            string jsonContent = await req.Content.ReadAsStringAsync();
            var msg = JsonConvert.DeserializeObject<Msg>(jsonContent);

            log.Info($"Message {msg.first} recieved");
            await outputQueue.AddAsync(msg);
            //return req.CreateResponse(HttpStatusCode.OK);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("Success")
            };


            return response;
        }
    }
}
