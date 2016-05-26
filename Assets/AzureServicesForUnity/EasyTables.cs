using AzureServicesForUnity.Helpers;
using AzureServicesForUnity.QueryHelpers.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.Networking;

namespace AzureServicesForUnity
{
    public class EasyTables : MonoBehaviour
    {
        public string Url;
        public static EasyTables Instance;

        [HideInInspector]
        public string AuthenticationToken;

        void Awake()
        {
            Instance = this;
            Utilities.ValidateForNull(Url);
        }



        #region Public methods



        public void Insert<T>(T instance, Action<CallbackResponse<T>> onInsertCompleted)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(instance, onInsertCompleted);
            StartCoroutine(InsertInternal(instance, onInsertCompleted));
        }

        public void SelectFiltered<T>(EasyTableQuery<T> query, Action<CallbackResponse<SelectFilteredResult<T>>> onSelectCompleted)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(onSelectCompleted); //query can be null
            StartCoroutine(SelectFilteredInternal(query, onSelectCompleted));
        }

        public void SelectByID<T>(string id, Action<CallbackResponse<T>> onSelectByIDCompleted)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(id, onSelectByIDCompleted);
            StartCoroutine(SelectByIDInternal<T>(id, onSelectByIDCompleted));
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

        #endregion

        private IEnumerator DeleteByIDInternal<T>(string id, Action<CallbackResponse> onDeleteCompleted)
            where T : AzureObjectBase
        {
            using (UnityWebRequest www = Utilities.BuildWebRequest
                (GetTablesUrl<T>() + "/" + WWW.EscapeURL(id), HttpMethod.Delete.ToString(), null, AuthenticationToken))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse response = new CallbackResponse();

                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error);
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

            using (UnityWebRequest www = Utilities.BuildWebRequest(GetTablesUrl<T>(),
                HttpMethod.Post.ToString(), json, AuthenticationToken))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse<T> response = new CallbackResponse<T>();

                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }

                else if (www.downloadHandler != null)  //all OK
                {
                    //let's get the new object that was created
                    try
                    {
                        T newObject = JsonUtility.FromJson<T>(www.downloadHandler.text);
                        if (Globals.DebugFlag) Debug.Log("new object ID is " + newObject.id);
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

        private IEnumerator SelectByIDInternal<T>(string id, Action<CallbackResponse<T>> onSelectByIDCompleted)
            where T : AzureObjectBase
        {
            using (UnityWebRequest www = Utilities.BuildWebRequest
                (GetTablesUrl<T>() + "/" + WWW.EscapeURL(id), HttpMethod.Get.ToString(), null, AuthenticationToken))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);
                CallbackResponse<T> response = new CallbackResponse<T>();
                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error);
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
                onSelectByIDCompleted(response);
            }
        }

        private IEnumerator SelectFilteredInternal<T>(EasyTableQuery<T> query,
            Action<CallbackResponse<SelectFilteredResult<T>>> onSelectCompleted)
           where T : AzureObjectBase
        {
            string url = GetTablesUrl<T>();
            if (query != null)
            {
                url += "?" + query.ToODataString();
            }
            if (Globals.DebugFlag) Debug.Log(url);
            using (UnityWebRequest www = Utilities.BuildWebRequest(url,
                HttpMethod.Get.ToString(), null, AuthenticationToken))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse<SelectFilteredResult<T>> response = new CallbackResponse<SelectFilteredResult<T>>();

                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }
                else
                {
                    response.Status = CallBackResult.Success;

                    SelectFilteredResult<T> selectFilteredResult = null;
                    if (query.RequestTotalCount)
                        selectFilteredResult = JsonUtility.FromJson<SelectFilteredResult<T>>(www.downloadHandler.text);
                    else
                    {
                        selectFilteredResult = new SelectFilteredResult<T>();
                        T[] data = Utilities.DeserializeJsonArray<T>(www.downloadHandler.text);
                        selectFilteredResult.results = data;
                        selectFilteredResult.count = null;
                    }
                    response.Result = selectFilteredResult;
                }
                onSelectCompleted(response);
            }
        }

        private IEnumerator UpdateInternal<T>(T instance, Action<CallbackResponse<T>> onUpdateCompleted)
           where T : AzureObjectBase
        {
            string json = JsonUtility.ToJson(instance);
            using (UnityWebRequest www = Utilities.BuildWebRequest(GetTablesUrl<T>(),
                HttpMethod.Patch.ToString(), json, AuthenticationToken))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);
                CallbackResponse<T> response = new CallbackResponse<T>();
                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }
                else if (www.downloadHandler != null)  //all OK
                {
                    try
                    { //let's get the new object that was created
                        T updatedObject = JsonUtility.FromJson<T>(www.downloadHandler.text);
                        if (Globals.DebugFlag) Debug.Log("updated object ID is " + updatedObject.id);
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

       

        private string GetTablesUrl<T>()
        {
            return string.Format("{0}/tables/{1}", Url, typeof(T).Name);
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


