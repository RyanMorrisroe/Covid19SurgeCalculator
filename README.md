# COVID-19 Surge Calculator
**This tool is provided as open source software, free of charge, and with no warranty. The author makes no claim to the accuracy of results - if you choose to employ this tool you should verify the output with other modeling tools.**

A web-based COVID-19 Surge Calculator based off the Sg2 beta v2 COVID-19 Surge Calculator.

**Important Note**: Although this codebase is licensed under the MIT license, which allows for free commercial use, it includes EPPlus 5.0.4 to generate Excel Exports. That software requires a paid license for commercial use. However, public safety or health organizations appear to be exempt from this requirement - please review https://epplussoftware.com/LicenseOverview/LicenseFAQ if you plan on using this code commercially. I believe you could downgrade to EPPlus 4 (does not require commercial licensing) with little effort, but I have not tested doing so. You could also swap to another Excel file generator.

## Building
1.) Clone the repo locally

2.) Open using Visual Studio 2019 (16.4 or later, was developed using 16.5.2) or VSCode

3.) Build the project

4.) Start CovidSurgeCalculator.Site to view locally

## Deployment
Follow standard deployment practices for an ASP.NET Core 3.1 website. Works on Windows or Linux. Uses SSL so be sure to supply a valid certificate. Can see a working deployment at https://covidsurgecalculator.azurewebsites.net/

## Configuration
Use the "UseSession" boolean in appsettings.json to determine the behavior of the site.

If UseSession is true:

* The site will first load the model inputs found in DefaultInputs.bin in App_Data\Binaries\
* Any modifications to the model inputs through the site are stored in the session - this is ideal for a simultaneous user scenario
* Changes from the default inputs are not persisted outside of a user session and are not shared between users

If UseSession is false:

* The site will load the model inputs found in Inputs.bin in App_Data\Binaries\
* Any modifications to the model inputs through the site will be written back to disk - there will be potential issues with this model in a simultaneous user scenario
* Changes from the default inputs are persisted across sessions, users, and shutdown

## Editing the Model
All forecasting logic is found in CovidSurgeCalculator.ModelData, unit tests are in CovidSurgeCalculator.ModelData.Tests. 

Raw infection model curve and age cohort data is found in the CSVs folder in CovidSurgeCalculator.ModelData.SerializationHelper. 

Run SerializationHelper to regenerate the default model inputs binary and the Reference Infection Model binary if alterations are made to the raw csv files. 

If new binaries are generated, they should be placed in App_Data\Binaries\ in CovidSurgeCalculator.Site.
