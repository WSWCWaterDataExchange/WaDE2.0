using CsvHelper.Configuration;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public sealed class CoordinateRawMap : ClassMap<CoordinateRaw>
    {
        public CoordinateRawMap(string xCoordMap, string yCoordMap)
        {
            Map(x => x.XCoord).Name(xCoordMap);
            Map(x => x.YCoord).Name(yCoordMap);
        }
    }
}
