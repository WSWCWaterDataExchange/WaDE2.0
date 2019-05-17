using System.IO;
using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public interface IBlobFileAccessor
    {
        Task<Stream> GetBlobData(string container, string path);

        Task SaveBlobData(string container, string path, byte[] data);

        Task SaveBlobData(string container, string path, string data);
    }
}
