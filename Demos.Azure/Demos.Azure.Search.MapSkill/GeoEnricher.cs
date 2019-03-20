namespace Demos.Azure.Search.MapSkill
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Demos.Azure.Search.MapSkill.MapService;
    using Demos.Azure.Search.MapSkill.SearchSkillInterface;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Newtonsoft.Json;
    
    using Microsoft.Extensions.Configuration;

    public static class GeoEnricher
    {    
        [FunctionName("GeoEnricher")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, TraceWriter log, ExecutionContext context)
        {
            log.Info("Function processing request...");

            var config = new ConfigurationBuilder()
                        .SetBasePath(context.FunctionAppDirectory)
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();

            var mapServiceBaseAddress = config["MapServiceBaseUrl"];
            var mapServiceKey = config["MapServiceKey"];

            // TODO: Move this to static to avoid socket exhaustion.
            var client = new HttpClient();
            client.BaseAddress = new Uri(mapServiceBaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();

            // Call map service
            var mapService = new AzureMapService(client, mapServiceKey);            
            // var mapService = new MockedMapService();

            var requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // Validation
            if (data?.values == null)
            {
                return new BadRequestObjectResult(" Could not find values array");
            }
            if (data?.values.HasValues == false || data?.values.First.HasValues == false)
            {
                // It could not find a record, then return empty values array.
                return new BadRequestObjectResult(" Could not find valid records in values array");
            }

            var recordId = data?.values?.First?.recordId?.Value as string;

            var locations = data?.values?.First?.data?.locations;
            var geoPoints = new List<GeoPoint>();
            foreach (var location in locations)
            {
                var coordinates = await mapService.GetCoordinates(JsonConvert.SerializeObject(location));
                geoPoints.Add(coordinates);
            }
            
            // Create Response
            var responseRecord = new WebApiResponseRecord();
            responseRecord.data = new Dictionary<string, object>();
            responseRecord.recordId = recordId;
            responseRecord.data.Add("lon-lat", geoPoints);

            var response = new WebApiEnricherResponse();
            response.values = new List<WebApiResponseRecord>();
            response.values.Add(responseRecord);

            return new OkObjectResult(response);
        }
    }
}
