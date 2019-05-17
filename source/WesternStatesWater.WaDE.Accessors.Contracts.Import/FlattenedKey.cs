using System.Collections.Generic;
using System.Linq;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class FlattenedKey
    {
        public string Text { get; set; }

        public List<FlattenedValue> FlattenedValues { get; set; }

        public string CsvValues
        {
            get
            {
                return FlattenedValues != null ?
                    string.Join(",", FlattenedValues.Select(x => x.Text)) :
                    null;
            }
        }
    }
}
