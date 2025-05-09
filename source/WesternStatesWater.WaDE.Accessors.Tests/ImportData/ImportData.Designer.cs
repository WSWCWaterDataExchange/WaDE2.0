﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WesternStatesWater.WaDE.Accessors.Tests.ImportData {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ImportData {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ImportData() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WesternStatesWater.WaDE.Accessors.Tests.ImportData.ImportData", typeof(ImportData).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OrganizationUUID,VariableSpecificUUID,ReportingUnitUUID,PrimaryUseCategory,BeneficialUseCategory,WaterSourceUUID,MethodUUID,TimeframeStart,TimeframeEnd,DataPublicationDate,DataPublicationDOI,ReportYearCV,Amount,PopulationServed,PowerGeneratedGWh,PowerType,IrrigatedAcreage,InterbasinTransferToID,InterbasinTransferFromID,CustomerTypeCV,AllocationCropDutyAmount,IrrigationMethodCV,CropTypeCV,CommunityWaterSupplySystem,SDWISIdentifierCV
        ///WWDO,Consumptive Use,WY_1,Irrigation,Agricultural Consumptive Use,Fresh_Sur [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetWaterAllocations_IrrigatedAcrageSpecifiedButEmpty {
            get {
                return ResourceManager.GetString("GetWaterAllocations_IrrigatedAcrageSpecifiedButEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MethodUUID,ApplicableResourceTypeCV,DataConfidenceValue,DataCoverageValue,DataQualityValueCV,MethodDescription,MethodName,MethodNEMILink,MethodTypeCV,WaDEDataMappingUrl
        ///abcd,Groundwater,,,,Test Water Rights,Fake Water Rights,http://fake.water.gov/,Adjudicated,http://fake.wade.gov/.
        /// </summary>
        internal static string Methods_OneRecord {
            get {
                return ResourceManager.GetString("Methods_OneRecord", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OrganizationUUID,OrganizationContactEmail,OrganizationContactName,OrganizationName,OrganizationPhoneNumber,OrganizationPurview,OrganizationDataMappingURL,OrganizationWebsite,State
        ///abcd,test@fake.gov,Test Contact,Test Org Name,402-555-1234,&quot;Test Purview, it is awesome&quot;,http://fake.mappingurl.com/,http://fake.gov/,NE
        ///.
        /// </summary>
        internal static string Organizations_IncludesOldColumn {
            get {
                return ResourceManager.GetString("Organizations_IncludesOldColumn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OrganizationUUID,OrganizationContactEmail,OrganizationContactName,OrganizationName,OrganizationPhoneNumber,OrganizationPurview,OrganizationWebsite,State
        ///abcd,test@fake.gov,Test Contact,Test Org Name,402-555-1234,&quot;Test Purview, it is awesome&quot;,http://fake.gov/,NE
        ///.
        /// </summary>
        internal static string Organizations_OneRecord {
            get {
                return ResourceManager.GetString("Organizations_OneRecord", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SiteUUID,OverlayUUIDs,WaterSourceUUIDs,CoordinateAccuracy,CoordinateMethodCV,County,EPSGCodeCV,Geometry,GNISCodeCV,HUC12,HUC8,Latitude,Longitude,PODorPOUSite,SiteName,SiteNativeID,SiteTypeCV,USGSSiteID
        ///NEwr_S1,abcd,&quot;NEwr_WS1,NEwr_WS2&quot;,1.01,Unspecified,Thayer,4326,Fake Geometry,999,103000000000,158400000000,40.01104072,-97.5223804,POD,Unspecified,10443,Unspecified,Fake USGS
        ///.
        /// </summary>
        internal static string Sites_OneRecord {
            get {
                return ResourceManager.GetString("Sites_OneRecord", resourceCulture);
            }
        }
    }
}
