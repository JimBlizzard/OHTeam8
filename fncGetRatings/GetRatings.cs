using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading;

namespace fncGetRatings
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            // App settings
            var dbConn = System.Environment.GetEnvironmentVariable("DBConnectionString", EnvironmentVariableTarget.Process);
            var databaseId = System.Environment.GetEnvironmentVariable("DatabaseId", EnvironmentVariableTarget.Process);
            var containerId = System.Environment.GetEnvironmentVariable("ContainerId", EnvironmentVariableTarget.Process);
            string userId = req.Query["userId"];
            log.LogInformation(string.Format("dbConn={0}, databaseId={1}, containerId={2}, userId={3}", dbConn, databaseId, containerId, userId));

            // Param validation
            if (string.IsNullOrEmpty(userId)) return new BadRequestObjectResult("No user Id provided"); // Bad request (400)

            // Connect to DB and execute query
            List<object> items = new List<object>();
            try
            {
                var dbClient = new CosmosClient(dbConn);
                var dbContainer = dbClient.GetContainer(databaseId, containerId);
                var dbQuery = string.Format("SELECT * FROM c WHERE c.userId = '{0}'", userId);
                QueryDefinition queryDefinition = new QueryDefinition(dbQuery);
                FeedIterator<object> queryResultSetIterator = dbContainer.GetItemQueryIterator<object>(queryDefinition);

                // Process the data set
                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<object> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (var item in currentResultSet)
                    {
                        log.LogInformation(string.Format("item={0}", item));
                        items.Add(item);
                    }
                }
            }
            catch (CosmosException ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult("Could not connect to the database."); // bad request (400)
            }

            // Process the results
            if (items.Count == 0) return new NotFoundObjectResult("No ratings found for that user."); // Not found (404)
            string responseMessage = JsonConvert.SerializeObject(items);
            return new OkObjectResult(responseMessage);
        }
    }
}
