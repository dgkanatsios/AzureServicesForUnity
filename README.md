[![Software License](https://img.shields.io/badge/license-MIT-brightgreen.svg?style=flat-square)](LICENSE.md)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)
!(https://gaforgithub.azurewebsites.net/api?repo=AzureServicesForUnity)

# Azure Services For Unity (Mobile Apps/App Service - Table Storage/CosmosDB Table API - Event Hubs)

A library to access and use various Azure services useful for games written in Unity game engine. Includes (a) an SDK and (b) sample code that uses the SDK.

## Azure services that can be used with this library

### Azure App Service / Mobile Apps
Microsoft Azure has a great service to host apps called [App Service](https://azure.microsoft.com/en-us/services/app-service/) which allows you to connect and expose a database via an awesome feature called [Easy Tables](https://azure.microsoft.com/en-us/blog/azure-app-service-updates-november-2015/). App Service also allows you to easily created various API endpoints for your game, via the Easy APIs feature. Easy Tables are a features of [Azure Mobile Apps](https://azure.microsoft.com/en-us/services/app-service/mobile/).

### Azure Table Storage

Azure Storage has a NoSQL key-value store called [Table Storage Service](https://azure.microsoft.com/en-us/services/storage/tables/) which can be used to store precious data for your game (such as highscores, save data, chat logs etc.). This library also contains some methods to access Table Storage Service from within a Unity game. 

### Azure CosmosDB with Table API

The same code that is used for Table Storage can be used to access the globally distributed database called [CosmosDB](https://docs.microsoft.com/en-us/azure/cosmos-db/introduction) using [Table API](https://docs.microsoft.com/en-us/azure/cosmos-db/create-table-dotnet#update-your-connection-string).

### Azure Event Hubs

You can also send data to Azure Event Hubs, a hyper-scale telemetry ingestion service that collects, transforms, and stores millions of events.

### Contents

In the project source files you can find three scenes, one that contains demos for App Service, one for Table Storage and one for Event Hubs. 

There are three blog posts that describe the library and its usage
- [Original blog post](https://dgkanatsios.com/2016/04/14/use-azure-services-from-unity/) describing the library and Easy Tables access
- [Updates, fixes and workarounds](https://dgkanatsios.com/2016/09/01/an-update-to-azure-services-for-unity-library/)
- [Accessing Table Storage and CosmosDB via Table Storage API](https://dgkanatsios.com/2017/02/21/accessing-azure-table-service-from-a-unity-game/)

### Usage

- Fork or clone this repository
- In the Assets/AzureServicesForUnity folder you'll find a folder called `_Demo Scenes`
- This folder contains three scenes that contain demos for App Service, EventHub and Storage interaction, respectively. Their relevant source files exist in the same folder. These files call the core files of the library.
- Core files of the library reside in three different folders in the Assets/AzureServicesForUnity. Feel free to use the folders you need (e.g. if you need only AppService interaction, copy the files inside the AppService folder). Do not forget to include the Shared folder.

### FAQ

#### I want to add Facebook authentication to my game. How can I do that?
On the server side you would want to do [this](https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-how-to-configure-facebook-authentication). On the client side, check the [Facebook Unity SDK](https://developers.facebook.com/docs/unity/). For other providers, chec [here](https://docs.microsoft.com/en-us/azure/app-service/app-service-authentication-overview).

#### Are you using any external plugins?
Nope, on purpose. One of the original goals of this library was to be plugin-free.

#### Which platforms does this work?
Hopefully on every platform supported by Unity. If you found a platform that this does not work, [ping me](https://github.com/dgkanatsios/AzureServicesForUnity/issues)!

#### Can I use this SDK to access the [CosmosDB Table API](https://docs.microsoft.com/en-us/azure/cosmos-db/table-introduction)?
Yup, raise an issue if you have problems accessing it.

#### I have a problem with the library, how can I report it?
Use the [Issues](https://github.com/dgkanatsios/AzureServicesForUnity/issues) tab.

#### Do you accept Pull Requests?
Sure, go ahead!
