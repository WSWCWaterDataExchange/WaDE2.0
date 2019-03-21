using AutoMapper;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using EF = WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Mapping
{
    internal class ApiProfile : Profile
    {
        public ApiProfile()
        {
            AllowNullDestinationValues = true;
            CreateMap<EF.AllocationsDim, AccessorApi.Allocation>()
                .ForMember(a => a.NativeAllocationID, b => b.MapFrom(c => c.AllocationNativeId))
                .ForMember(a => a.WaterAllocationPriorityDate, b => b.MapFrom(c => c.AllocationPriorityDateNavigation.Date))
                 .ForMember(a => a.AllocationExpirationDate, b => b.MapFrom(c => c.AllocationExpirationDateNavigation.Date))
                 .ForMember(a => a.AllocationApplicationDate, b => b.MapFrom(c => c.AllocationApplicationDateNavigation.Date))
                .ForMember(a => a.AllocationLegalStatusCodeCV, b => b.MapFrom(c => c.AllocationLegalStatusCv))
                .ForMember(a => a.AllocationAcreage, b => b.Ignore());
            CreateMap<EF.AllocationAmountsFact, AccessorApi.AllocationAmounts>()
                .ForMember(a => a.BeneficialUses, b => b.MapFrom(c => c.AllocationBridgeBeneficialUsesFact))
                .ForMember(a => a.TimeframeStart, b => b.MapFrom(c => c.TimeframeStartDate.Date))
                .ForMember(a => a.TimeframeEnd, b => b.MapFrom(c => c.TimeframeEndDate.Date))
                .ForMember(a => a.DataPublicationDate, b => b.MapFrom(c => c.DataPublicationDate.Date))
                .ForMember(a => a.InterbasinTransferToID, b => b.Ignore())
                .ForMember(a => a.InterbasinTransferFromID, b => b.Ignore())
                .ForMember(a => a.Geometry, b => b.Ignore())
                .ForMember(a => a.PrimaryBeneficialUse, b => b.MapFrom(c => c.PrimaryBeneficialUse == null ? null : c.PrimaryBeneficialUse));
            CreateMap<EF.BeneficialUsesDim, AccessorApi.BeneficialUse>();
            CreateMap<EF.AllocationBridgeBeneficialUsesFact, AccessorApi.BeneficialUse>()
                .ForMember(a => a.BeneficialUseUUID, b => b.MapFrom(c => c.BeneficialUse.BeneficialUseUuid))
                .ForMember(a => a.BeneficialUseCategory, b => b.MapFrom(c => c.BeneficialUse.BeneficialUseCategory))
                .ForMember(a => a.USGSCategoryNameCV, b => b.MapFrom(c => c.BeneficialUse.UsgscategoryNameCv))
                .ForMember(a => a.NAICSCodeNameCV, b => b.MapFrom(c => c.BeneficialUse.NaicscodeNameCv));
            CreateMap<EF.MethodsDim, AccessorApi.Method>()
                .ForMember(a => a.ApplicableResourceType, b => b.MapFrom(c => c.ApplicableResourceTypeCv))
                .ForMember(a => a.DataQualityValue, b => b.MapFrom(c => c.DataQualityValueCv));
            CreateMap<EF.Nhdmetadata, AccessorApi.NHDMetadata>();
            CreateMap<EF.OrganizationsDim, AccessorApi.Organization>()
                .ForMember(a => a.OrganizationState, b => b.Ignore());
            CreateMap<EF.SitesDim, AccessorApi.Site>()
                .ForMember(a => a.SiteCode, b => b.MapFrom(c => c.SiteNativeId))
                .ForMember(a => a.VerticalDatumEPSGCodeCV, b => b.Ignore())
                .ForMember(a => a.CoordinateMethod, b => b.MapFrom(c => c.CoordinateMethodCv))
                .ForMember(a => a.State, b => b.Ignore())
                .ForMember(a => a.AllocationAcreage, b => b.Ignore())
                .ForMember(a => a.AllocationBasisCV, b => b.Ignore());
            //.ForMember(a => a.Organization, b => b.Ignore());
            CreateMap<EF.VariablesDim, AccessorApi.VariableSpecific>()
                .ForMember(a => a.VariableSpecificTypeCV, b => b.MapFrom(c => c.VariableSpecificCv))
                .ForMember(a => a.VariableTypeDescription, b => b.MapFrom(c => c.VariableCv))
                .ForMember(a => a.UnitOfMeasureCV, b => b.MapFrom(c => c.AmountUnitCv));
            CreateMap<EF.WaterSourcesDim, AccessorApi.WaterSource>()
                .ForMember(a => a.WaterSourceCode, b => b.MapFrom(c => c.WaterSourceNativeId))
                .ForMember(a => a.FreshSalineIndicatorCV, b => b.MapFrom(c => c.WaterQualityIndicatorCv))
                .ForMember(a => a.Organization, b => b.Ignore());
        }
    }
}
