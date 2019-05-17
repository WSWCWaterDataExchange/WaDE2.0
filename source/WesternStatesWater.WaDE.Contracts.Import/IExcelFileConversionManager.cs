using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Import
{
    public interface IExcelFileConversionManager
    {
        Task ConvertExcelFileToJsonFiles(string container, string folder, string fileName);
    }
}
