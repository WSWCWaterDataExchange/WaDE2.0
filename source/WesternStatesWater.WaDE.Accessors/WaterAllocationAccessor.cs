using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternStatesWater.WaDE.Accessors.Contracts;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;
using AutoMapper.QueryableExtensions;

namespace WesternStatesWater.WaDE.Accessors
{
    public class WaterAllocationAccessor : IWaterAllocationAccessor
    {
        public WaterAllocationAccessor(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        List<AllocationAmounts> IWaterAllocationAccessor.GetSiteAllocationAmounts(string variableSpecificCV, string siteUuid)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                IQueryable<EntityFramework.AllocationAmountsFact> query = db.AllocationAmountsFact;
                if (!string.IsNullOrWhiteSpace(variableSpecificCV))
                {
                    query = query.Where(a => a.VariableSpecific.VariableSpecificCv == variableSpecificCV);
                }
                if (!string.IsNullOrWhiteSpace(siteUuid))
                {
                    query = query.Where(a => a.Site.SiteUuid == siteUuid);
                }
                return query.ProjectTo<AllocationAmounts>(Mapping.DtoMapper.Configuration).ToList();
            }
        }
    }
}
