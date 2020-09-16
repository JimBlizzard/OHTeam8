using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using System.Collections.Generic;


namespace fncGetRatings
{
    public static class DocByIdFromQueryString
    {
        [FunctionName("GetRating")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GetRating/{ratingId}")]
                HttpRequest req,
            [CosmosDB(
                databaseName: "icecreamratings",
                collectionName: "Ratings",
                ConnectionStringSetting = "cosmosdbconnection",
                 SqlQuery ="SELECT * FROM c WHERE c.id={ratingId} ORDER BY c._ts DESC")] IEnumerable<Rating> ratingItems,
            ILogger log,
            string ratingId)

        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (ratingItems == null)
            {
                log.LogInformation($"ratingItems item not found");
                return new NotFoundResult();
            }
            return new OkObjectResult(ratingItems);
            //return new OkResult();
        }
    }
}