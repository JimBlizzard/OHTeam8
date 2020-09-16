using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace fncGetRatings
{
    public static class CreateRating

    {
        [FunctionName("CreateRating")]
        public static void Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [CosmosDB(
        databaseName: "icecreamratings",
        collectionName: "ratings",
        ConnectionStringSetting = "cosmosdbconnection")]out dynamic document,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var input = JsonConvert.DeserializeObject<Rating>(requestBody);
            document = new
            {
                id = Guid.NewGuid(),
                userId = input.userId,
                productId = input.productId,
                locationName = input.locationName,
                rating = input.rating,
                userNotes = input.userNotes
            };
            log.LogInformation($"C# Queue trigger function inserted one row");
            log.LogInformation($"Description={requestBody}");
        }

    }
}
