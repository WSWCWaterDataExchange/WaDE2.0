using CsvHelper.Configuration.Attributes;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class FlattenedValue
    {
        [Name("Text")]
        public string Text { get; set; }
    }
}
