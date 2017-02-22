# Azure Services For Unity 
Microsoft Azure has a great service to host apps called [App Service](https://azure.microsoft.com/en-us/services/app-service/) which allows you to connect and expose a database via an awesome feature called [Easy Tables](https://azure.microsoft.com/en-us/blog/azure-app-service-updates-november-2015/). App Service also allows you to easily created various API endpoints for your game, via the Easy APIs feature. 
Moreover, Azure Storage has a great NoSQL key-value store called [Table Storage Service](https://azure.microsoft.com/en-us/services/storage/tables/) which can be used to store precious data for your game (such as highscores, save data, chat logs etc.). This library also contains some methods to access Table Storage Service from within a Unity game.

In the project source files you can find two scenes, one that contains demos for App Service and one for Table Storage. 

There are three blog posts that describe the library and its usage
- [Original blog post](https://dgkanatsios.com/2016/04/14/use-azure-services-from-unity/) describing the library and Easy Tables access
- [Updates, fixes and workarounds](https://dgkanatsios.com/2016/09/01/an-update-to-azure-services-for-unity-library/)
- [Accessing Table Storage](https://dgkanatsios.com/2017/02/21/accessing-azure-table-service-from-a-unity-game/)

###Usage
In the root folder you'll find two scenes that contain demos for App Service and Storage interaction, respectively. Their relevant source files exist in the UIScripts folder. Main library resides in the AppServicesForUnity folder. In this folder, the AppService and Storage folders contain isolated code about AppService and Storage, respectively whereas the Shared folder contains code used by both.
So, in order to use this library in your Unity game, copy the AppServicesForUnity folder into your project and delete the folder you do not need. To use the library, copy the relevant code from the UIScripts folder.

###Changelog

###version 0.0.11 (21/2/2017)
- Added methods to access Azure Table Service to use as a game backend

###version 0.0.10
- Changes in Easy APIs API (no, that's correct!) so that there are two methods now, one to return a single and one to return multiple results

###version 0.0.9
- Fix for use on Android, check details here: https://github.com/dgkanatsios/AzureServicesForUnity/issues/4

###version 0.0.8 (18/8/2016)
- Updated for use with Unity 5.4 (removed "Experimental" from UnityWebRequest namespace)

###version 0.0.7 (28/7/2016)
- Fixed a couple of reported issues on OSX and iOS

####version 0.0.6
- Fixed IncludeTotalCount() issue

####version 0.0.5
- Namespace changes, separating Easy Table and Easy APIs
- Target for next version is a *.unitypackage file creation, fingers crossed!

####version 0.0.4
- Fix for issue about [OData not working on iOS] (https://github.com/dgkanatsios/AzureServicesForUnity/issues/1). Many thanks to Matt Warren for pointing me to a PartialEvaluator solution that does not use Reflection.Emit (check [here](https://github.com/mattwar/iqtoolkit/blob/master/src/IQToolkit/ExpressionEvaluator.cs)) and [9 to Friday studio](http://www.9tofriday.co.za/) for precious help in debugging.

####version 0.0.3
- Integrated the (much better) official OData query translator from the [official App Service .NET client](https://github.com/Azure/azure-mobile-apps-net-client/tree/master/src/Microsoft.WindowsAzure.MobileServices/Table)

####version 0.0.2
- Initial commit
