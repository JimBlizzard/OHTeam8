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
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}

namespace GetRatingsFNC
{
    public static class DocByIdFromQueryString
    {
        [FunctionName("GetRatingsFNC")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
                HttpRequest req,
            [CosmosDB(
                databaseName: "icecreamratings",
                collectionName: "Ratings",
                ConnectionStringSetting = "AccountEndpoint=https://bfyocteam8.documents.azure.com:443/;AccountKey=cEkGHZa16gG1ctXj1nltOTUGaFt71itUde6UtPT1bASK85J91yPktH3UgBqdjgtxSZc1T0UrlPZwEfGLYF0rWg==;Database=icecreamratings",
                //Id = "{Query.id}",
                //PartitionKey = "{Query.partitionKey}")] ToDoItem toDoItem,
                 SqlQuery ="SELECT * FROM c WHERE c.id={ratingId} ORDER BY c._ts DESC")] IEnumerable<Rating> ratingItems,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (toDoItem == null)
            {
                log.LogInformation($"ToDo item not found");
            }
            else
            {
                log.LogInformation($"Found ToDo item, Description={toDoItem.Description}");
            }
            return new OkResult();
        }
    }
}