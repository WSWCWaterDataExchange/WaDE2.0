using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ExcelConversion(ManagerImport.IExcelFileConversionManager excelFileConversionManager)
        {
            ExcelFileConversionManager = excelFileConversionManager;
        }

        private ManagerImport.IExcelFileConversionManager ExcelFileConversionManager { get; set; }

        [Function("ExcelConversionToJson")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string container = req.Query["container"];
            string folder = req.Query["folder"];
            string fileName = req.Query["fileName"];

            await ExcelFileConversionManager.ConvertExcelFileToJsonFiles(container, folder, fileName);

            var jsonResult = req.CreateResponse(HttpStatusCode.OK);
            var jsonToReturn = JsonConvert.SerializeObject(new { status = "success" });
            await jsonResult.WriteStringAsync(jsonToReturn);
            return jsonResult;
        }
    }
}
