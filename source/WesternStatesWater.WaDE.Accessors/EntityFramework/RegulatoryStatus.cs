﻿using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class RegulatoryStatus
    {
        public string Name { get; set; }
        public string Term { get; set; }
        public string State { get; set; }
        public string Definition { get; set; }
        public string SourceVocabularyUri { get; set; }
    }
}
