using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using ManagerApi = WesternStatesWater.WaDE.Contracts.Api;

namespace WesternStatesWater.WaDE.Managers.Mapping
{
    internal class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<AccessorApi.Allocation, ManagerApi.Allocation>();
            CreateMap<AccessorApi.BeneficialUse, ManagerApi.BeneficialUse>();
            CreateMap<AccessorApi.Method, ManagerApi.Method>();
            CreateMap<AccessorApi.NHDMetadata, ManagerApi.NHDMetadata>();
            CreateMap<AccessorApi.WaterAllocationOrganization, ManagerApi.WaterAllocationOrganization>();
            CreateMap<AccessorApi.Site, ManagerApi.Site>();
            CreateMap<AccessorApi.VariableSpecific, ManagerApi.VariableSpecific>();
            CreateMap<AccessorApi.WaterSource, ManagerApi.WaterSource>();
        }
    }
}
