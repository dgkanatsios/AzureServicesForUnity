using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Experimental.Networking;
using System.Linq;

namespace AzureServicesForUnity
{
    public class UnityServices : MonoBehaviour
    {
        public string Url = "https://unityservicesdemo.azurewebsites.net";
        public bool DebugFlag = true;

        [HideInInspector]
        public string AuthenticationToken;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                DestroyImmediate(this);
            }
            Utilities.ValidateForNull(Url);
        }

        private static UnityServices instance;
        public static UnityServices Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UnityServices();
                }
                return instance;
            }
        }

        public void CallAPI<T>(string apiname, Action<CallbackResponse<T>> onInvokeAPICompleted)
            where T : class
        {
            Utilities.ValidateForNull(apiname, onInvokeAPICompleted);
            StartCoroutine(CallAPIInternal(apiname, onInvokeAPICompleted));
        }



        public void Insert<T>(T instance, Action<CallbackResponse<T>> onInsertCompleted)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(instance, onInsertCompleted);
            StartCoroutine(InsertInternal(instance, onInsertCompleted));
        }

        public void SelectFiltered<T>(IQueryable<T> query, Action<CallbackResponse<T[]>> onSelectCompleted)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(onSelectCompleted); //query can be null
            StartCoroutine(SelectFilteredInternal(query, onSelectCompleted));
        }

        public void SelectByID<T>(string id, Action<CallbackResponse<T>> onSelectSingleCompleted)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(id, onSelectSingleCompleted);
            StartCoroutine(SelectByIDInternal<T>(id, onSelectSingleCompleted));
        }

        public void UpdateObject<T>(T instance, Action<CallbackResponse<T>> onUpdateCompleted)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(instance);
            StartCoroutine(UpdateInternal(instance, onUpdateCompleted));
        }

        public void DeleteByID<T>(string id, Action<CallbackResponse> onDeleteCompleted)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(id, onDeleteCompleted);
            StartCoroutine(DeleteByIDInternal<Highscore>(id, onDeleteCompleted));
        }

        private IEnumerator DeleteByIDInternal<T>(string id, Action<CallbackResponse> onDeleteCompleted)
            where T : AzureObjectBase
        {
            using (UnityWebRequest www = BuildWebRequest<T>(GetTablesUrl<T>() + "/" + WWW.EscapeURL(id), "DELETE", null))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse response = new CallbackResponse();

                if (Utilities.IsWWWError(www))
                {
                    if (DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }
                else
                {
                    response.Status = CallBackResult.Success;
                }
                onDeleteCompleted(response);
            }
        }

        private IEnumerator InsertInternal<T>(T instance, Action<CallbackResponse<T>> onInsertCompleted)
          where T : AzureObjectBase
        {
            string json = JsonUtility.ToJson(instance);

            using (UnityWebRequest www = BuildWebRequest<T>(GetTablesUrl<T>(), "POST", json))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse<T> response = new CallbackResponse<T>();

                if (Utilities.IsWWWError(www))
                {
                    if (DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }

                else if (www.downloadHandler != null)  //all OK
                {
                    //let's get the new object that was created
                    try
                    {
                        T newObject = JsonUtility.FromJson<T>(www.downloadHandler.text);
                        if (DebugFlag) Debug.Log("new object ID is " + newObject.id);
                        response.Status = CallBackResult.Success;
                        response.Result = newObject;
                    }
                    catch (Exception ex)
                    {
                        response.Status = CallBackResult.DeserializationFailure;
                        response.Exception = ex;
                    }
                }
                onInsertCompleted(response);
            }
        }

        private IEnumerator SelectByIDInternal<T>(string id, Action<CallbackResponse<T>> onSelectSingleCompleted)
            where T : AzureObjectBase
        {
            using (UnityWebRequest www = BuildWebRequest<T>(GetTablesUrl<T>() + "/" + WWW.EscapeURL(id), "GET", null))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);
                CallbackResponse<T> response = new CallbackResponse<T>();
                if (Utilities.IsWWWError(www))
                {
                    if (DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }
                else
                {
                    try
                    {
                        T data = JsonUtility.FromJson<T>(www.downloadHandler.text);
                        response.Result = data;
                        response.Status = CallBackResult.Success;
                    }
                    catch (Exception ex)
                    {
                        response.Status = CallBackResult.Failure;
                        response.Exception = ex;
                    }
                }
                onSelectSingleCompleted(response);
            }
        }

        private IEnumerator SelectFilteredInternal<T>(IQueryable<T> query, Action<CallbackResponse<T[]>> onSelectCompleted)
           where T : AzureObjectBase
        {
            string url = GetTablesUrl<T>();
            if (query != null)
            {
                url += query.ToString();
            }
            if (DebugFlag) Debug.Log(url);
            using (UnityWebRequest www = BuildWebRequest<T>(url, "GET", null))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse<T[]> response = new CallbackResponse<T[]>();

                if (Utilities.IsWWWError(www))
                {
                    if (DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }
                else
                {
                    response.Status = CallBackResult.Success;
                    T[] data = Utilities.DeserializeJsonArray<T>(www.downloadHandler.text);
                    response.Result = data;
                }
                onSelectCompleted(response);
            }
        }

        private IEnumerator UpdateInternal<T>(T instance, Action<CallbackResponse<T>> onUpdateCompleted)
           where T : AzureObjectBase
        {
            string json = JsonUtility.ToJson(instance);
            using (UnityWebRequest www = BuildWebRequest<T>(GetTablesUrl<T>(), "PATCH", json))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);
                CallbackResponse<T> response = new CallbackResponse<T>();
                if (Utilities.IsWWWError(www))
                {
                    if (DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }
                else if (www.downloadHandler != null)  //all OK
                {
                    //let's get the new object that was created
                    try
                    {
                        T updatedObject = JsonUtility.FromJson<T>(www.downloadHandler.text);
                        if (DebugFlag) Debug.Log("updated object ID is " + updatedObject.id);
                        response.Status = CallBackResult.Success;
                        response.Result = updatedObject;
                    }
                    catch (Exception ex)
                    {
                        response.Status = CallBackResult.DeserializationFailure;
                        response.Exception = ex;
                    }
                }
                onUpdateCompleted(response);
            }

        }

        private IEnumerator CallAPIInternal<T>(string apiname,Action<CallbackResponse<T>> onInvokeAPICompleted)
            where T : class
        {
            using (UnityWebRequest www = BuildWebRequest<T>(GetApiUrl(apiname), "POST", null))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);
                CallbackResponse<T> response = new CallbackResponse<T>();
                if (Utilities.IsWWWError(www))
                {
                    if (DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }
                else if (www.downloadHandler != null)  //all OK
                {
                    try
                    {
                        //FromJson method does not do well with single quotes...
                        T returnObject = JsonUtility.FromJson<T>(www.downloadHandler.text);
                        response.Status = CallBackResult.Success;
                        response.Result = returnObject;
                    }
                    catch (Exception ex)
                    {
                        response.Status = CallBackResult.DeserializationFailure;
                        response.Exception = ex;
                    }
                }
                onInvokeAPICompleted(response);

            }
        }


        private UnityWebRequest BuildWebRequest<T>(string url, string method, string json)
            where T : class
        {
            UnityWebRequest www = new UnityWebRequest(url, method);

            www.SetRequestHeader(Constants.Accept, Constants.ApplicationJson);
            www.SetRequestHeader(Constants.Content_Type, Constants.ApplicationJson);
            www.SetRequestHeader(Constants.ZumoString, Constants.ZumoVersion);

            if (!string.IsNullOrEmpty(AuthenticationToken))
                www.SetRequestHeader(Constants.ZumoAuth, AuthenticationToken.Trim());

            www.downloadHandler = new DownloadHandlerBuffer();

            if (json != null)
            {
                byte[] payload = System.Text.Encoding.UTF8.GetBytes(json);
                UploadHandler handler = new UploadHandlerRaw(payload);
                handler.contentType = Constants.ApplicationJson;
                www.uploadHandler = handler;
            }
            return www;

        }

        private string GetTablesUrl<T>()
        {
            return string.Format("{0}/tables/{1}", Url, typeof(T).Name);
        }

        private string GetApiUrl(string apiname)
        {
            return string.Format("{0}/api/{1}", Url, apiname);
        }

    }
}


///Some helpful links, included here for reference
///Table operations on Azure: https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-node-backend-how-to-use-server-sdk/#howto-dynamicschema
///How to use the managed client: https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-dotnet-how-to-use-client-library/#inserting
///Basic OData tutorial: http://www.odata.org/getting-started/basic-tutorial/
///OData URI conventions: http://www.odata.org/documentation/odata-version-2-0/uri-conventions/
///OData operations: http://www.odata.org/documentation/odata-version-2-0/operations/
///Walkthough: creating an IQueryable LINQ provider: https://msdn.microsoft.com/en-us/library/bb546158(v=vs.110).aspx
///Building an IQueryable provider blog post series: https://blogs.msdn.microsoft.com/mattwar/page/3/
///Add authentication to your Windows app: https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-windows-store-dotnet-get-started-users/
///How to configure Facebook authentication: https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-how-to-configure-facebook-authentication/

