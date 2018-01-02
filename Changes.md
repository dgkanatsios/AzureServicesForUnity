## Changelog

#### version 0.0.13 (2/1/2018)
- Minor fix for CosmosDB Table API support

#### version 0.0.12 (2/5/2017)
- Added method to post data to Azure Event Hub

#### version 0.0.11 (21/2/2017)
- Added methods to access Azure Table Service to use as a game backend

#### version 0.0.10
- Changes in Easy APIs API (no, that's correct!) so that there are two methods now, one to return a single and one to return multiple results

#### version 0.0.9
- Fix for use on Android, check details here: https://github.com/dgkanatsios/AzureServicesForUnity/issues/4

#### version 0.0.8 (18/8/2016)
- Updated for use with Unity 5.4 (removed "Experimental" from UnityWebRequest namespace)

#### version 0.0.7 (28/7/2016)
- Fixed a couple of reported issues on OSX and iOS

#### version 0.0.6
- Fixed IncludeTotalCount() issue

#### version 0.0.5
- Namespace changes, separating Easy Table and Easy APIs
- Target for next version is a *.unitypackage file creation, fingers crossed!

#### version 0.0.4
- Fix for issue about [OData not working on iOS] (https://github.com/dgkanatsios/AzureServicesForUnity/issues/1). Many thanks to Matt Warren for pointing me to a PartialEvaluator solution that does not use Reflection.Emit (check [here](https://github.com/mattwar/iqtoolkit/blob/master/src/IQToolkit/ExpressionEvaluator.cs)) and [9 to Friday studio](http://www.9tofriday.co.za/) for precious help in debugging.

#### version 0.0.3
- Integrated the (much better) official OData query translator from the [official App Service .NET client](https://github.com/Azure/azure-mobile-apps-net-client/tree/master/src/Microsoft.WindowsAzure.MobileServices/Table)

#### version 0.0.2
- Initial commit
