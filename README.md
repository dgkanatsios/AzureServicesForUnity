# Azure Services For Unity 
Microsoft Azure has a great service to host apps called [App Service](https://azure.microsoft.com/en-us/services/app-service/) which allows you to connect and expose a database via an awesome feature called [Easy Tables](https://azure.microsoft.com/en-us/blog/azure-app-service-updates-november-2015/)

Unity has a great class to do HTTP requests called [UnityWebRequest](http://docs.unity3d.com/Manual/UnityWebRequest.html). It's relatively new but it can be really helpful. Plus, there is another handy class called [JsonUtility](http://docs.unity3d.com/ScriptReference/JsonUtility.html) that assists in JSON serialization/deserialization.

Combine the above two paragraphs and you have a small cross platform library that allows a Unity game easy access Azure Easy Tables (yes, very easy!).

###Current version: 0.0.3 (beta)

For information and usage instructions check https://dgkanatsios.com/2016/04/14/use-azure-services-from-unity/

###Changelog

####version 0.0.3
- Integrated the (much better) official OData query translator from the [official App Service .NET client](https://github.com/Azure/azure-mobile-apps-net-client/tree/master/src/Microsoft.WindowsAzure.MobileServices/Table)

####version 0.0.2
- Initial commit
