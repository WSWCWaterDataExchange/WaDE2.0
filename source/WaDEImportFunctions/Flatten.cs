using System.Net;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ManagerImport = WesternStatesWater.WaDE.Contracts.Import;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace WaDEImportFunctions
{
    public class Flatten
    {
        private readonly ManagerImport.IFlattenManager _flattenManager;
        
        private readonly ILogger<Flatten> _logger;
        
        public Flatten(ManagerImport.IFlattenManager flattenManager, ILogger<Flatten> logger)
        {
            _flattenManager = flattenManager;
            _logger = logger;
        }

        [Function("CoordinateProjection")]
        public async Task<HttpResponseData> RunProjection([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string container = req.Query["container"];
            string folder = req.Query["folder"];
            string sourceFileName = req.Query["sourceFileName"];
            string destinationFileName = req.Query["destinationFileName"];
            string xValueCol = req.Query["xValueCol"];
            string yValueCol = req.Query["yValueCol"];

            await _flattenManager.CoordinateProjection(container, folder, sourceFileName, destinationFileName, xValueCol, yValueCol);

            var jsonResult = req.CreateResponse(HttpStatusCode.OK);
            var jsonToReturn = JsonConvert.SerializeObject(new { status = "success" });
            await jsonResult.WriteStringAsync(jsonToReturn);
            return jsonResult;
        }

        [Function("Flatten")]
        public async Task<HttpResponseData> RunFlatten([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string container = req.Query["container"];
            string folder = req.Query["folder"];
            string sourceFileName = req.Query["sourceFileName"];
            string destinationFileName = req.Query["destinationFileName"];
            string keyCol = req.Query["keyCol"];
            string valueCol = req.Query["valueCol"];

            await _flattenManager.Flatten(container, folder, sourceFileName, destinationFileName, keyCol, valueCol);

            var jsonResult = req.CreateResponse(HttpStatusCode.OK);
            var jsonToReturn = JsonConvert.SerializeObject(new { status = "success" });
            await jsonResult.WriteStringAsync(jsonToReturn);
            return jsonResult;
        }
    }
}
