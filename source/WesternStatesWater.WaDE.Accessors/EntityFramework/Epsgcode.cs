﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class Epsgcode
    {
        public Epsgcode()
        {
            ReportingUnitsDim = new HashSet<ReportingUnitsDim>();
            SitesDim = new HashSet<SitesDim>();
        }

        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Term { get; set; }
        [MaxLength(4000)]
        public string Definition { get; set; }
        [MaxLength(250)]
        public string State { get; set; }
        [MaxLength(250)]
        public string SourceVocabularyUri { get; set; }

        public virtual ICollection<ReportingUnitsDim> ReportingUnitsDim { get; set; }
        public virtual ICollection<SitesDim> SitesDim { get; set; }
    }
}
