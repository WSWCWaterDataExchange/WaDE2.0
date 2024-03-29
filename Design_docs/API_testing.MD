

# WaDE2 Swagger API Tested queries and Responses
(To be updated with more info avalabe)

## A.	Site Allocation Amounts

Test 1: Beneficial use of Irrigation with end priority date 1890

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?BeneficialUseCV=Irrigation&EndPriorityDate=01%2F01%2F1890

Response: Works

Test 2: Reservoir water allocated for Fire in Colorado

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?SiteTypeCV=Reservoir&BeneficialUseCV=Fire&SearchGeometry=Colorado 

Response: 500 currently unable to handle this request.

2.2: Add priority end date (if it helps)

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?SiteTypeCV=Reservoir&BeneficialUseCV=Fire&SearchGeometry=Colorado&EndPriorityDate=01/01/1890

Response: 500 currently unable to handle this request.

Test 3: Reservoir site type with Domestic beneficial use

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?SiteTypeCV=Reservoir&BeneficialUseCV=Domestic

Response: 500 currently unable to handle this request.

Test 4. Beneficial use Fire in Colorado with priority end date 1990

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?SiteTypeCV=Reservoir&BeneficialUseCV=Fire&SearchGeometry=Colorado&EndPriorityDate=01/01/1990

Response: 500 currently unable to handle this request.

Test 5. Beneficial use Evaporative from Reservoirs in Colorado

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?SiteTypeCV=Reservoir&BeneficialUseCV=Evaporative&SearchGeometry=Colorado

Response: 500 currently unable to handle this request.

5.2 Remove Colorado (Search geometry)

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?SiteTypeCV=Reservoir&BeneficialUseCV=Evaporative

Response: Works

5.3 Add priority date

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?SiteTypeCV=Reservoir&BeneficialUseCV=Evaporative&EndPriorityDate=01/01/1990

Response: Works

Conclusion: Search Geometry “Colorado” doesn’t exist? 

Test 6

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?BeneficialUseCV=Irrigation&EndPriorityDate=01/01/1850

Response: Works for all the three states currently in WaDE: UT, CO, OK

Test 6

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?BeneficialUseCV=Mining&EndPriorityDate=01/01/1850

Response: Works gets UT and OK

Test 7

using 'SiteAllocationAmountsDigest' (simplified version of the call) for organization Washington State Department of Ecology

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmountsDigest?EndPriorityDate=01/01/1850&OrganizationUUID=WSDE

OK. Returns 11 rows of data

Test 8

use organization only

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmountsDigest?OrganizationUUID=WSDE

Works. Returns rows of data for Washington.

Test 9 

Try other states

Works for OWRD (OR), OWRB (OK), NDSWC (ND), CODWR (CO)

Test 10

Like Test 7 but Change to recent end priority date

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmountsDigest?EndPriorityDate=01/01/1950&OrganizationUUID=WSDE

wade-api-qa.azure-api.net is currently unable to handle this request.
HTTP ERROR 500

Test 11

Organization CODWR with end priority date 1950

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmountsDigest?EndPriorityDate=01/01/1950&OrganizationUUID=CODWR

wade-api-qa.azure-api.net is currently unable to handle this request.
HTTP ERROR 500

Test 12

Add beneficial use to 'Test 7'

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmountsDigest?BeneficialUseCV=Mining&EndPriorityDate=01/01/1890

OK. Returns samples for UT and OR

Test 13

Recent end priority date with BeneficialUseCV=Mining

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmountsDigest?BeneficialUseCV=Mining&EndPriorityDate=01/01/1950

Works. Returns samples for UT and OR

Test 14

Using state filter with water allocations

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?EndPriorityDate=01/01/1890&State=WA

OK. Works

Test 15

Test other states OR, OK, UT, CO, SD

OK. Works

Test 15 

Recent end priority date 1950 with allocation amounts

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?EndPriorityDate=01/01/1950&State=OK

Works. Gets larger number of rows for OK. 

Test 16 

Like 15 but with different state

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?EndPriorityDate=01/01/1950&State=WA

Works. 

Test 17 

Just the state no other filter

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?State=WA

Works.

Also works for other states OR, OK, CO, UT, ND, AZ

Test 18

Specify index in search

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmounts?EndPriorityDate=01/01/1890&State=WA&StartIndex=1000

Works.

Test 19

Specify record length in search

https://wade-api-qa.azure-api.net/v1/SiteAllocationAmountsDigest?OrganizationUUID=WSDE&StartIndex=0&RecordCount=10

Works.

## B.	Site Variable Amounts 

Test 1. 

https://wade-api-qa.azure-api.net/v1/SiteVariableAmounts?SiteTypeCV=Reservoir&VariableCV=Allocation&VariableSpecificCV=Allocation%20all&BeneficialUseCV=Evaporative&StartDate=10/01/1990

Response: [ ] (Empty string)

1.2 reduce the conditions

https://wade-api-qa.azure-api.net/v1/SiteVariableAmounts?SiteTypeCV=Reservoir&BeneficialUseCV=Evaporative

Response: [ ] (Empty string)

Test 2. 

https://wade-api-qa.azure-api.net/v1/SiteVariableAmounts?SiteTypeCV=Well&BeneficialUseCV=Domestic

Response: [ ] (Empty string)

Test 3.

https://wade-api-qa.azure-api.net/v1/SiteVariableAmounts?SiteTypeCV=Mine

Response: [ ] (Empty string)

Test 4. 

https://wade-api-qa.azure-api.net/v1/SiteVariableAmounts?SiteTypeCV=Ditch

Response: [ ] (Empty string)

Test 4.  Search on the GUI .

gives out “Internal Error” for the following Search.
This search also seems to require the user to know what SiteUUID they are looking for which is difficult to know if you don’t know the data already.
 
Conclusion: All valid searches seem to give out empty string [ ]

Test 5 

https://wade-api-qa.azure-api.net/v1/SiteVariableAmounts?BeneficialUseCV=Agriculture

Response: [ ] (Empty string)


## C.	Aggregated Amounts

Test 1. 

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?VariableCV=Consumptive%20Use&VariableSpecificCV=Withdrawal%2C%20Irrigation&BeneficialUseCV=Irrigation&USGSCategoryNameCV=Irrigation%2C%20Surface%20Water%2C%20Fresh%09

Response: [ ] (empty string)

Test 2.

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?VariableSpecificCV=Irrigation&BeneficialUseCV=Irrigation&USGSCategoryNameCV=Irrigation

Response: [ ] (empty string)

Test 3

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?ReportingUnitUUID=HUC8&BeneficialUseCV=Irrigation&USGSCategoryNameCV=Irrigation

Response: [ ] (empty string)

Test 4.

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?ReportingUnitUUID=SubArea&BeneficialUseCV=Irrigation

Response: [ ] (empty string)

Test 5.

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?ReportingUnitUUID=SubArea&BeneficialUseCV=Export%20From%20Basin

Response: [ ] (empty string)

Test 6

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?ReportingUnitUUID=15010003

Response: Works

Test 7

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?ReportingUnitTypeCV=SubArea

Response: Worlks returns UT aggregated amounts

Test 8

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?BeneficialUseCV=Agriculture

Response: Works retuns UT with agriculture ben uses

Test 9

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?BeneficialUseCV=Agriculture&ReportingUnitTypeCV=SubArea

Response: Works 

Test 10

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?BeneficialUseCV=Agriculture&ReportingUnitTypeCV=County

Response: Works returns UT

Test 11

https://wade-api-qa.azure-api.net/v1/AggregatedAmounts?ReportingUnitTypeCV=County

Response: Works returns UT and NM


## D. Agg Regulatory Overlay

Observation1: RegulatoryOverlayUUID is not in the controlled vocabulary list (http://vocabulary.westernstateswater.org/

Test 1. 

https://wade-api-qa.azure-api.net/v1/AggRegulatoryOverlay?ReportingUnitUUID=HUC8&RegulatoryStatusCV=In%20Effect

Response: [ ]

Test 2. 

https://wade-api-qa.azure-api.net/v1/AggRegulatoryOverlay?ReportingUnitUUID=HUC8

Response: [ ]

Test 3. 

https://wade-api-qa.azure-api.net/v1/AggRegulatoryOverlay?OrganizationUUID=CODWR&ReportingUnitUUID=HUC8

Response: [ ]

Conclusion: There aren’t many options here

Test 4 Specify Search boundary as WKT

https://wade-api-qa.azure-api.net/v1/AggRegulatoryOverlay?OrganizationUUID=CODWR&ReportingUnitUUID=HUC8&SearchBoundary=MultiPolygon%20(((-105.03039889981448596%2040.99811901369133693,%20-104.67152999714699035%2040.93662999701217586,%20-104.54517995921685269%2040.55182993675373382,%20-104.58765863616156366%2040.42510143726843808,%20-104.88326387696423581%2040.72136293266157736,%20-105.03039889981448596%2040.99811901369133693)))

Response: [ ]

Test 5. 

https://wade-api-qa.azure-api.net/v1/AggRegulatoryOverlay?OrganizationUUID=CODWR&ReportingUnitUUID=HUC8&SearchBoundary=10190008

Response: HTTP ERROR 500. wade-api-qa.azure-api.net is currently unable to handle this request.

Test 6. For Utah

https://wade-api-qa.azure-api.net/v1/AggRegulatoryOverlay?OrganizationUUID=UTDWRE

Response: [ ]

https://wade-api-qa.azure-api.net/v1/AggRegulatoryOverlay?OrganizationUUID=UTDWRE&ReportingUnitUUID=49003

Response: [ ]

Conclusion: These entrees exist in the Google doc Aggregated amount. Not clear why they don’t load:
(https://drive.google.com/drive/folders/1PYtPVdV-DfVhOYeefV8haOvsVWrmbBdQ)

Test 6.

https://wade-api-qa.azure-api.net/v1/AggRegulatoryOverlay?OrganizationUUID=UTDWRE&ReportingUnitUUID=HUC8&SearchBoundary=MultiPolygon%20(((-112.11839477105620233%2037.00113964195932681,%20-112.368945999999994%2037.00112500000000182,%20-112.79622000582783414%2037.00047388676497917,%20-112.38868750220444781%2037.50238600396886568,%20-112.39891643412261146%2037.41419705759852832,%20-112.32310287233877943%2037.42090320599390907,%20-112.11839477105620233%2037.00113964195932681)))

Response: [ ]

Test 7. Remove organization id

https://wade-api-qa.azure-api.net/v1/AggRegulatoryOverlay?ReportingUnitUUID=49001

Response:
[{"OrganizationName":null,"OrganizationPurview":null,"OrganizationWebsite":null,"OrganizationPhoneNumber":null,"OrganizationContactName":null,"OrganizationContactEmail":null,"OrganizationState":null,"RegulatoryOverlays":[],"ReportingUnitsRegulatory":[]}]


Test 8. 

https://wade-api-qa.azure-api.net/v1/AggRegulatoryOverlay?ReportingUnitUUID=15010003

Response: [ ]

