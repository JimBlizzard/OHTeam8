using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace CreateRating
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
               databaseName: "icecreamratings",
                collectionName: "Ratings",
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<ratingModel> ratingDoc,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Pull in data from Put query string
            string userId = req.Query["userId"];
            string productId = req.Query["productId"];
            string locationName = req.Query["locationName"];
            string rating = req.Query["rating"];
            string userNotes = req.Query["userNotes"];

            // Pull in data from Push request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userId = userId ?? data?.userId;
            productId = productId ?? data?.productId;
            locationName = locationName ?? data?.locationName;
            rating = rating ?? data?.rating;
            userNotes = userNotes ?? data?.userNotes;

            // Validate userId 
            if (string.IsNullOrEmpty(userId))
            {
                return new OkObjectResult("Please enter a valid userId");
            }
            // Call GetUser API to validate user
            HttpClient userClient = new HttpClient();
            HttpRequestMessage userRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("https://serverlessohuser.trafficmanager.net/api/GetUser?userId={0}", userId));

            // Read response from API
            HttpResponseMessage userResponse = await userClient.SendAsync(userRequest);
            var userContent = userResponse.Content.ReadAsStringAsync().Result;
            dynamic userParameters = JsonConvert.DeserializeObject<userIdModel>(userContent);
            log.LogInformation($"userContent: {userContent}");
            log.LogInformation($"userName: {userParameters.userName}");



            // Validate productId 
            if (string.IsNullOrEmpty(productId))
            {
                return new OkObjectResult("Please enter a valid productId");
            }
            // Call GetProduct API to validate productId
            HttpClient productClient = new HttpClient();
            HttpRequestMessage productRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("https://serverlessohproduct.trafficmanager.net/api/GetProduct?productId={0}", productId));

            // Read response from API
            HttpResponseMessage productResponse = await productClient.SendAsync(productRequest);
            var productContent = productResponse.Content.ReadAsStringAsync().Result;
            dynamic productParameters = JsonConvert.DeserializeObject<productIdModel>(productContent);

            // Validate rating 
            int numRating = Int32.Parse(rating);
            if (numRating > 5)
            {
                return new OkObjectResult("Please enter a valid rating");
            }
            if (numRating < 1)
            {
                  return new OkObjectResult("Please enter a valid rating");
            }

            // Create id GUID
            Guid id = Guid.NewGuid();
            string idStr = id.ToString();

            // get current time
            string currentDateTime = DateTime.UtcNow.ToString();

            /*ratingModel newRating = new ratingModel();

           
            // Create JSON object
            newRating.id = idStr;
            newRating.userId = userId;
            newRating.productId = productId;
            newRating.timeStamp = currentDateTime;
            newRating.locationName = locationName;
            newRating.rating = rating;
            newRating.userNotes = userNotes;
            */

            // send data to cosmosdb
            ratingDoc = new { id = idStr, userId = userId, productId = productId, timeStamp = currentDateTime, locationName = locationName, rating = rating, userNotes = userNotes };

            //string newRatingStr = newRating.ToString();
            //Return json object
            string responseMessage = $"{newRatingStr}";
            return new OkObjectResult(responseMessage);
        }
    }
}

public class userIdModel
{
    public string userId { get; set; }
    public string userName { get; set; }
    public string fullName { get; set; }
}

public class productIdModel
{
    public string productId { get; set; }
    public string productName { get; set; }
    public string productDescription { get; set; }
}

public class ratingModel
{
    public string id { get; set; }
    public string userId { get; set; }
    public string productId { get; set; }
    public string timeStamp { get; set; }
    public string locationName { get; set; }
    public string rating { get; set; }
    public string userNotes { get; set; }
}