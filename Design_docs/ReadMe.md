# A Design Document for the Water Data Exchange (WaDE) Program – Phase II  

DRAFT ONLY    

**Adel Abdallah and Sara Larsen, February 2019**  


## 1.	Introduction   
The Water Data Exchange (WaDE) program provides an API that streamlines access to data provided by western state water agencies and related water availability, use, and rights information. WaDE is built using an agreed upon data schema that reconciles syntactic (e.g., structure) and will address semantic differences as shared data standards are adopted by participating data providers (Larsen and Young, 2014).

WaDE supports the following four fundamental distinct types of water data shared by the member states: i) water allocations (e.g., water rights and permitting data), ii) aggregated water budget estimates such as water supply, withdrawal, consumptive use, return flows, and transfers for reporting units (i.e., geospatial delineations used by the states such as counties, HUCs, and custom delineations) over time as time series, iii) site specific reported water data (i.e., water supply, withdrawal, consumptive use, return flows), and iv) regulatory and institutional constraints at play within states/basins that regulate water supply and use in specific locations. Next we describe the need for the Phase II, Section 3 describes the high level motivating use cases, Section 4, provides a high level description of the conceptual design, and Section 5 provides additional specific uses cases.

## 2.	Why WaDE 2.0?   
The WaDE 2.0 schema overcomes many challenges to i) support more uses cases such as the ability to more easily query time series data, ii) be interoperable with the U.S. Geological Survey (USGS) “geofabric” framework for national data discover, if the provider has them, iii) improve data loading and retrieval efficiency, and iv) increase performance and reliability of the system, v) make updates to the system more easily, vi) monitor access and usage more completely, and vii) reduce the effort needed by participating agencies to load data into WaDE and to maintain them.   

Check out the WaDE 2.0 simple schema and its web-services in [Google Sheets](
https://docs.google.com/spreadsheets/d/e/2PACX-1vQEXASrzI-6u_-FXjs-8tkm5EW7GamQKnszP80iuHq8MwzVN6cOtlRsCX-qs3ruJA8K0Cyty3VAVjwK/pubhtml)

WaDE 1.0 implemented a federated “node” approach where participating member states maintain their own physically separate WaDE database with a central catalog that directs data access back to each node. Most participating member states host their own node and maintain its security, mapping their data to the WaDE schema, and updating them over time.   


The WaDE 2.0 approach resolves many of the challenges listed above by integrating data provided by participating members into a cloud-based, central repository where data are organized for each state agency using its unique identifier, maintaining their integrity and state provider control over what is shared in the system (Figure 1). Thus, data for each member state can be uploaded or potentially removed independently from other member states. Each member state may provide their data in a simplified format and host it on a variety of platforms, such as Socrata, ESRI’s Online Data Hub platform, or CKAN/DKAN. 

WaDE will provide a data mediator (similar to the National Groundwater Monitoring Network Portal) which would auto load data from such sources into the cache copy on a cloud. Each agency would only maintain their data source in the best technology that fits their needs. WSWC will work to help states refine their services and shared data using a common vocabulary and similar temporal and spatial scales in order to present a regional picture of water supply and use in the West. The first pilot of WaDE Phase 2.0 will be to consolidate water budget inputs for the four Upper Colorado River Basin states going forward.


The centralized repository approach has two main benefits to the member states and to the administrators of WaDE. First, member states do not have to host a WaDE database server and thus potentially avoid costs, changing security requirements, infrequent updates, and other technical difficulties. Second, it eliminates the need for the operation of a central catalog to direct the data queries to separate databases. Any schema or configuration updates and security monitoring would be undertaken by WaDE admins and the member states would only maintain their data sources as part of their operations. Lastly, a centralized design would support more efficient and faster data responses, which in turn would enable regional, multistate and multi-year data analyses. Figure 2 is a conceptual graphic of the data storage and functions related to the central repository for one potential cloud platform.


![](https://github.com/WSWCWaterDataExchange/WaDE2.0/blob/master/Design_docs/Diagrams/WaDE_workflow.jpg)     

**Figure 1:** Schematic of WaDE 2.0 workflow 


![](https://github.com/WSWCWaterDataExchange/WaDE2.0/blob/master/Design_docs/Diagrams/Azure_workflow.png)   

**Figure 2:** WaDE system implementation workflow in Azure 

## 3.	WaDE Basic Use Cases   

Use cases are categorized based on the supported WaDE datatypes  
  
### i. Water Allocations   

Water allocations are also known as water rights. In the western U.S., water rights are based on the prior appropriation doctrine “First in time, first in right”. Each water right has an allocated amount of water (based on a volumetric quantity or a flow rate) with a designated beneficial use (e.g., agriculture), and sometimes multiples uses, and that may come from one or many specified water sources. Each state has a unique water right system and vocabulary and WaDE enables streamlined access to all the states’ water rights at a high granularity of the data with the same metadata and structure using one query method. New water rights are being continually added where the water is available for use. 

Existing water rights change slowly over time where water rights are updated (e.g., according to legal status, such as “perfected” or “adjudicated”), “abandoned” due to non-use, or change applications may be filed that transfer a right, sometimes with a new designated beneficial use category. Basic water allocation use case questions include:   

•	Discovery of owners, allocated amount(s), beneficial use(s), water sources, and priority dates within a geospatial boundary entirely within a state, or, more interestingly, one that straddles a state border.   

•	Use of a dashboard interface to enable sorting of the returned data by uses and dates, thus becoming a high-level screening tool for water market requests/solicitations within a designated trading area. 


### ii.	Aggregated Water Uses   

Aggregated water use, supply, and transfers may be estimated or calculated from user-reported data over reporting units and are known as water budgets. One of the most difficult parameters of water budgets are components related to human withdrawal and use of the water. Within a state water resource agency, water use estimates are most often estimated annually with a one year lag time, and more rarely on a monthly basis. Water use within a basin has historically been categorized by both withdrawals (i.e., how much water was taken from a stream, spring, or reservoir) and consumptive use (i.e., how much water was used or depleted by the application of the water withdrawal). There are methods used by state agencies to estimate withdrawals, consumptive use, and sometimes both parameters. Examples of basic aggregated water use case questions are:   

•	What is the annual water budget (including withdrawals and consumptive use) for a given year in a reporting unit or multiple reporting units within a state, or, more interestingly, across multiple states where watershed boundaries cross state lines?   

•	What is the annual or monthly time series aggregated water withdrawal, consumptive use, beneficial use categories, water source types, and estimation methods for a reporting unit or multiple reporting units in any member state, or across multiple states? How do states account for each other’s data where watersheds overlap state boundaries?  


### iii.	Site-specific Water Use   

Water is withdrawn from site-specific locations from a water source and then applied or utilized (i.e., depleted), often at a different location. In other words, a water right may have one or many points of diversion, which may in turn have one or many places of use, which may in turn have one or many return flows (i.e., discharges back to the natural hydrologic system) (Figure 3). An example of a site-specific water use case questions is:  

•	Where are related points of diversion and their actual amounts withdrawn? Where are places of use (consumptive use) and their actual amounts consumed? Where are points of return flow/discharge and the amounts returned to the hydrologic system?   

•	What is the annual or monthly time series for a given set of withdrawals or consumptive uses with the beneficial use category, water source type, and/or methodology as selection parameters for a reporting unit in any member state?   


### iv.	Regulatory/Institutional Constraints     

There are regulations, compacts, special management areas, etc. that provide a regulatory framework for water supply and water use across each state in the West. Examples of use cases that involve regulatory/institutional constraints include:  

•	What statues, special management areas, regulatory oversight, compacts, or laws are involved in regulation of water supply or water use in a given reporting unit in any member state? More interestingly, how does these boundaries interact as state boundaries? Are regulations continuous or fragmented across state lines?  

•	What are the extents (i.e., affected reporting units) that coincide with a regulatory/institutional constraint of interest?  

![](https://github.com/WSWCWaterDataExchange/WaDE2.0/blob/master/Design_docs/Diagrams/WaterRights_sites_conceptual.png)   

**Figure 3:** Conceptual diagram of water rights that may or may not have a location (a “site” in the WaDE DB), may have one or many withdrawals, which may have one or many consumptive uses, and return flows as sites.


# 4.	Conceptual design

In WaDE 2.0 each WaDE water data type has metadata that describe its amount over time. Each amount must have the following fundamental metadata (Figure 4).

**What:** the variable of what is being reported or measured (e.g., water use, water supply). It also includes the type of water (i.e., ground water, surface water), beneficial use, and units (cfs, acre feet)  

**Where:** the location either as a site with a latitude and longitude coordinate value, or a selected reporting unit for aggregated data. Each location is geo-referenced in the database to enable geospatial queries

**How:** the method used to derive the data value 

**Who:** the data provider which is the water agency.

**When:** the time period for the data 

![](https://github.com/WSWCWaterDataExchange/WaDE2.0/blob/master/Design_docs/Diagrams/conceptual.png)   

**Figure 4:** High-level metadata elements for WaDE data. Green is the central table (most often updated) with values for variable amounts. Orange coloring is for metadata that is updated less frequently. Blue indicates data that change over time for each amount value. Red denotes water allocation data, which is uniquely regulatory and not associated with fluctuating amounts like other time-series in WaDE.


## 5.	Specific Use Cases to Address   

### 1.	Water Allocations in the Upper Colorado River Basin  

•	What are the appropriated water rights amounts in the Upper Colorado River Basin sorted by date? 

•	What are the withdrawal sites that are associated with a water right? 

•	What is the percentage share of appropriated water rights for agriculture in the Upper Colorado River Basin? How are water rights beneficial use categories changing over time?

•	Show a map for the Upper Colorado at HUC-8 scale with a heat map that shows the highest appropriated water for different sectors with color ramps.

•	Show a map for the Upper Colorado at HUC-8 scale with water rights, water withdrawals, or consumptive use estimates with their reliance on surface vs. groundwater 

### 2.	Aggregated Water Uses 

•	Display a time series of aggregated water withdrawals for agriculture from groundwater in the Bear River Watershed spanning Utah, Idaho, and Wyoming

•	What is the aggregated water budget for Cache County, Utah? Show how this is changing over the years against water supply or climate parameters (e.g. precipitation, snowpack, storage, etc.)

### 3.	Site-Specific Water Use

•	What is the time series annual consumptive water use for public supply at a site and what’s the water source name and method used to estimate it? What is the water rights info for this site in New Mexico?

•	What are the withdrawal sites that are provided from the Snake River?

### 4.	Regulatory Overlay for Reporting Units 

•	What regulatory issues prevent further water appropriation in a selected basin, such as the Sevier River Watershed? 

•	What are regulatory agencies or laws that regulate water use in the Bear River watershed?

•	What are the HUC-8s that are influenced by the Colorado River Compact? The Bear River Compact?

 
## References  
https://acwi.gov/hydrology/sic/presentations/larsen_wade_july2018.pdf  

https://onlinelibrary.wiley.com/doi/full/10.1111/j.1936-704X.2014.03177.x  

https://iopscience.iop.org/article/10.1088/1748-9326/9/6/064009/meta  
