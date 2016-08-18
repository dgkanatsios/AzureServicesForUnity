# Azure Services For Unity 
Microsoft Azure has a great service to host apps called [App Service](https://azure.microsoft.com/en-us/services/app-service/) which allows you to connect and expose a database via an awesome feature called [Easy Tables](https://azure.microsoft.com/en-us/blog/azure-app-service-updates-november-2015/)

Unity has a great class to do HTTP requests called [UnityWebRequest](http://docs.unity3d.com/Manual/UnityWebRequest.html). It's relatively new but it can be really helpful. Plus, there is another handy class called [JsonUtility](http://docs.unity3d.com/ScriptReference/JsonUtility.html) that assists in JSON serialization/deserialization.

Combine the above two paragraphs and you have a small cross platform library that allows a Unity game easy access Azure Easy Tables (yes, very easy!).

Sample scene points to a free Azure mobile app, so it can take some time to respond in subsequent requests.

###Current version: 0.0.8 (beta)

For information and usage instructions check https://dgkanatsios.com/2016/04/14/use-azure-services-from-unity/ and the demo scene
I've made some changes to the API compared to the one in the blog post, update imminent!

###Changelog

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
