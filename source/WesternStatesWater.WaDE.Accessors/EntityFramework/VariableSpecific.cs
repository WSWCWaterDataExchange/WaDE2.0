using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class VariableSpecific
    {
        public string Name { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public string Category { get; set; }
        public string SourceVocabularyUri { get; set; }

        public virtual ICollection<VariablesDim> VariablesDims { get; set; }
    }
}
