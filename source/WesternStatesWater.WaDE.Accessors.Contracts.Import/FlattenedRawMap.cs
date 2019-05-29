using CsvHelper.Configuration;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public sealed class FlattenedRawMap : ClassMap<FlattenedRaw>
    {
        public FlattenedRawMap(string keyMap, string valueMap)
        {
            Map(x => x.Key).Name(keyMap);
            Map(x => x.Value).Name(valueMap);
        }
    }

}
