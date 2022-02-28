using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using WesternStatesWater.WaDE.Accessors.Mapping;
using WesternStatesWater.WaDE.Common;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;
using EF = WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class DtoMapper
    {
        [TestMethod]
        public void ConfigurationIsValid()
        {
            Mapping.DtoMapper.Configuration.AssertConfigurationIsValid();
        }

        [DataTestMethod]
        [PodSiteToPouSiteFactToPodToPouSiteRelationshipTestDataSource]
        public void Map_PODSiteToPOUSiteFactToPodToPouSiteRelationship_SiteUuid(EF.PODSiteToPOUSiteFact podSiteToPOUSiteFact, Action<IMappingOperationOptions> mappingOperations, bool shouldThrow, string expectedValue)
        {
            if (shouldThrow)
            {
                Action call = () => podSiteToPOUSiteFact.Map<AccessorApi.PodToPouSiteRelationship>(mappingOperations);
                call.Should().Throw<AutoMapperMappingException>()
                    .WithInnerException<WaDEException>();
            }
            else
            {
                var result = podSiteToPOUSiteFact.Map<AccessorApi.PodToPouSiteRelationship>(mappingOperations);
                if (expectedValue == null)
                {
                    result.Should().BeNull();
                }
                else
                {
                    result.SiteUUID.Should().Be(expectedValue);
                }
            }
            
        }

        private class PodSiteToPouSiteFactToPodToPouSiteRelationshipTestDataSourceAttribute : Attribute, ITestDataSource
        {
            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                var podPouSiteFact = new EF.PODSiteToPOUSiteFact { PODSite = new EF.SitesDim { SiteUuid = "POD Value" }, POUSite = new EF.SitesDim { SiteUuid = "POU Value" } };
                yield return new object[] { null, null, false, null };
                yield return new object[] { podPouSiteFact, null, true, null };
                yield return new object[] { podPouSiteFact, new Action<IMappingOperationOptions>(a => { }), true, null };
                yield return new object[] { podPouSiteFact, new Action<IMappingOperationOptions>(a => { a.Items.Add("Fake", ApiProfile.PodValue); }), true, null };
                yield return new object[] { podPouSiteFact, new Action<IMappingOperationOptions>(a => { a.Items.Add(ApiProfile.PodPouKey, "Not Valid"); }), true, null };
                yield return new object[] { podPouSiteFact, new Action<IMappingOperationOptions>(a => { a.Items.Add(ApiProfile.PodPouKey, true); }), true, null };
                yield return new object[] { podPouSiteFact, new Action<IMappingOperationOptions>(a => { a.Items.Add(ApiProfile.PodPouKey, new object()); }), true, null };
                yield return new object[] { podPouSiteFact, new Action<IMappingOperationOptions>(a => { a.Items.Add(ApiProfile.PodPouKey, ApiProfile.PodValue); }), false, "POD Value" };
                yield return new object[] { podPouSiteFact, new Action<IMappingOperationOptions>(a => { a.Items.Add(ApiProfile.PodPouKey, ApiProfile.PouValue); }), false, "POU Value" };
            }

            public string GetDisplayName(MethodInfo methodInfo, object[] data)
            {
                return $"{methodInfo.Name}({string.Join(",",  data)})";
            }
        }
    }

}
