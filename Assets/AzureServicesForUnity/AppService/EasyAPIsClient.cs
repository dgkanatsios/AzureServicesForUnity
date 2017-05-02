using Assets.AzureServicesForUnity.Shared;
using AzureServicesForUnity.AppService;
using AzureServicesForUnity.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AzureServicesForUnity.AppService
{
    public class EasyAPIsClient : MonoBehaviour
    {

        /// <summary>
        /// Calls Easy API and returns multiple results in array
        /// </summary>
        /// <typeparam name="T">Typeof return object</typeparam>
        /// <typeparam name="P">Typeof object to send to the server</typeparam>
        /// <param name="apiname">Name of the API</param>
        /// <param name="method">HTTP verb</param>
        /// <param name="onInvokeAPICompleted">Callback</param>
        /// <param name="instance">Instance of object to send to the server</param>
        public void CallAPIMultiple<T, P>(string apiname, HttpMethod method, Action<CallbackResponse<T[]>> onInvokeAPICompleted, P instance)
            where T : class
            where P : class
        {
            Utilities.ValidateForNull(apiname, onInvokeAPICompleted);
            StartCoroutine(CallAPIInternal(apiname, method, onInvokeAPICompleted, EasyAPIReturnType.MultipleResults, instance));
        }

        /// <summary>
        /// Calls Easy API and returns multiple results in array
        /// </summary>
        /// <typeparam name="T">Typeof return object</typeparam>
        /// <param name="apiname">Name of the API</param>
        /// <param name="method">HTTP verb</param>
        /// <param name="onInvokeAPICompleted">Callback</param>
        public void CallAPIMultiple<T>(string apiname, HttpMethod method, Action<CallbackResponse<T[]>> onInvokeAPICompleted)
        where T : class
        {
            Utilities.ValidateForNull(apiname, onInvokeAPICompleted);
            StartCoroutine(CallAPIInternal<T, object>(apiname, method, onInvokeAPICompleted,EasyAPIReturnType.MultipleResults));
        }

        /// <summary>
        /// Calls Easy API and returns single result
        /// </summary>
        /// <typeparam name="T">Typeof return object</typeparam>
        /// <typeparam name="P">Typeof object to send to the server</typeparam>
        /// <param name="apiname">Name of the API</param>
        /// <param name="method">HTTP verb</param>
        /// <param name="onInvokeAPICompleted">Callback</param>
        /// <param name="instance">Instance of object to send to the server</param>
        public void CallAPISingle<T, P>(string apiname, HttpMethod method, Action<CallbackResponse<T[]>> onInvokeAPICompleted, P instance)
        where T : class
        where P : class
        {
            Utilities.ValidateForNull(apiname, onInvokeAPICompleted);
            StartCoroutine(CallAPIInternal(apiname, method, onInvokeAPICompleted,EasyAPIReturnType.SingleResult, instance));
        }

        /// <summary>
        /// Calls Easy API and returns single result
        /// </summary>
        /// <typeparam name="T">Typeof return object</typeparam>
        /// <param name="apiname">Name of the API</param>
        /// <param name="method">HTTP verb</param>
        /// <param name="onInvokeAPICompleted">Callback</param>
        public void CallAPISingle<T>(string apiname, HttpMethod method, Action<CallbackResponse<T[]>> onInvokeAPICompleted)
           where T : class
        {
            Utilities.ValidateForNull(apiname, onInvokeAPICompleted);
            StartCoroutine(CallAPIInternal<T, object>(apiname, method, onInvokeAPICompleted, EasyAPIReturnType.SingleResult));
        }

        public string Url;
        public static EasyAPIsClient Instance;

        [HideInInspector]
        public string AuthenticationToken;

        void Awake()
        {
            Instance = this;
            Utilities.ValidateForNull(Url);
        }

        private IEnumerator CallAPIInternal<T, P>(string apiname,
            HttpMethod method, Action<CallbackResponse<T[]>> onInvokeAPICompleted, EasyAPIReturnType returnType, P instance = null)
           where T : class
           where P : class
        {
            string json = null;
            if (instance != null) json = JsonUtility.ToJson(instance);

            using (UnityWebRequest www = AppServiceUtilities.BuildAppServiceWebRequest(GetApiUrl(apiname),
                method.ToString(), json, AuthenticationToken))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);
                CallbackResponse<T[]> response = new CallbackResponse<T[]>();
                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }
                else if (www.downloadHandler != null)  //all OK
                {
                    try
                    {
                        T[] returnObject = null;
                        if (returnType == EasyAPIReturnType.MultipleResults)
                            returnObject = JsonHelper.GetJsonArray<T>(www.downloadHandler.text);
                        else if(returnType == EasyAPIReturnType.SingleResult)
                        {
                            var returnObjectSingle = JsonUtility.FromJson<T>(www.downloadHandler.text);
                            returnObject = new T[] { returnObjectSingle };
                        }
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



        private string GetApiUrl(string apiname)
        {
            return string.Format("{0}/api/{1}", Url, apiname);
        }
    }

    public enum EasyAPIReturnType
    {
        SingleResult,
        MultipleResults
    }
}
