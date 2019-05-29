using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Contracts.Import
{
    public interface IFlattenManager
    {
        Task Flatten(string container, string folder, string sourceFileName, string destFileName, string keyCol, string valueCol);

        Task CoordinateProjection(string container, string folder, string sourceFileName, string destFileName, string xValueCol, string yValueCol);
    }
}
