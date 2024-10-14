using System.Net;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ManagerImport = WesternStatesWater.WaDE.Contracts.Import;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace WaDEImportFunctions
{
    public class ExcelConversion
    {
        private readonly ManagerImport.IExcelFileConversionManager _excelFileConversionManager;
        
        private readonly ILogger<ExcelConversion> _logger;
        
        public ExcelConversion(
            ManagerImport.IExcelFileConversionManager excelFileConversionManager,
            ILogger<ExcelConversion> logger
        )
        {
            _excelFileConversionManager = excelFileConversionManager;
            _logger = logger;
        }

        [Function("ExcelConversionToJson")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string container = req.Query["container"];
            string folder = req.Query["folder"];
            string fileName = req.Query["fileName"];

            await _excelFileConversionManager.ConvertExcelFileToJsonFiles(container, folder, fileName);

            var jsonResult = req.CreateResponse(HttpStatusCode.OK);
            var jsonToReturn = JsonConvert.SerializeObject(new { status = "success" });
            await jsonResult.WriteStringAsync(jsonToReturn);
            return jsonResult;
        }
    }
}
