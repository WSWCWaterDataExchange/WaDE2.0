using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccessorImport = WesternStatesWater.WaDE.Accessors.Contracts.Import;
using ManagerImport = WesternStatesWater.WaDE.Contracts.Import;
using System.IO;
using System.Dynamic;
using Newtonsoft.Json;

namespace WesternStatesWater.WaDE.Managers
{
    public class ExcelFileConversionManager : ManagerImport.IExcelFileConversionManager
    {
        public ExcelFileConversionManager(AccessorImport.IBlobFileAccessor importBlobFileAccessor)
        {
            ImportBlobFileAccessor = importBlobFileAccessor;
        }

        public AccessorImport.IBlobFileAccessor ImportBlobFileAccessor { get; set; }

        async Task ManagerImport.IExcelFileConversionManager.ConvertExcelFileToJsonFiles(string container, string folder, string fileName)
        {
            var rawData = await ImportBlobFileAccessor.GetBlobData(container, Path.Combine(folder, fileName));

            var package = new OfficeOpenXml.ExcelPackage(rawData);

            foreach (var worksheet in package.Workbook.Worksheets)
            {
                var headers = worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column].Select(a => string.IsNullOrWhiteSpace(a.Text) ? $"Column{a.Start.Column}" : a.Text.Replace(" ", "")).ToList();

                var resultData = new List<dynamic>();

                using (var ms = new MemoryStream())
                using (var tw = new StreamWriter(ms))
                using (var jtw = new JsonTextWriter(tw))
                {
                    jtw.WriteStartArray();
                    for (int rowNum = 2; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                    {
                        jtw.WriteStartObject();
                        for (var i = 0; i < headers.Count(); i++)
                        {
                            jtw.WritePropertyName(headers[i]);
                            var value = worksheet.Cells[rowNum, i + 1].Text;
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                jtw.WriteValue((string)null);
                            }
                            else
                            {
                                jtw.WriteValue(worksheet.Cells[rowNum, i + 1].Text);
                            }
                        }
                        jtw.WriteEndObject();
                    }
                    jtw.WriteEndArray();

                    jtw.Flush();
                    tw.Flush();

                    await ImportBlobFileAccessor.SaveBlobData(container, Path.Combine(folder, Path.ChangeExtension(worksheet.Name.Replace(" ", ""), ".json")), ms.ToArray());
                }
            }
        }
    }
}
