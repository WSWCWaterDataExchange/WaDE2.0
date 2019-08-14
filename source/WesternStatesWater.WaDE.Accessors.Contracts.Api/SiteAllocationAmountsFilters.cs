﻿using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class SiteAllocationAmountsFilters
    {
        public string SiteUuid { get; set; }
        public string SiteTypeCV { get; set; }
        public string BeneficialUseCv { get; set; }
        public string UsgsCategoryNameCv { get; set; }
        public string Geometry { get; set; }
        public DateTime? StartPriorityDate { get; set; }
        public DateTime? EndPriorityDate { get; set; }
    }
}