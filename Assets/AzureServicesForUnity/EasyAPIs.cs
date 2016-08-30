using AzureServicesForUnity.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AzureServicesForUnity
{
    public class EasyAPIs : MonoBehaviour
    {


        public void CallAPI<T,P>(string apiname, HttpMethod method, Action<CallbackResponse<T>> onInvokeAPICompleted, P instance)
            where T : class
            where P : class
        {
            Utilities.ValidateForNull(apiname, onInvokeAPICompleted);
            StartCoroutine(CallAPIInternal(apiname, method, onInvokeAPICompleted, instance));
        }

        public void CallAPI<T>(string apiname, HttpMethod method, Action<CallbackResponse<T>> onInvokeAPICompleted)
           where T : class
        {
            Utilities.ValidateForNull(apiname, onInvokeAPICompleted);
            StartCoroutine(CallAPIInternal<T,object>(apiname, method, onInvokeAPICompleted));
        }

        public string Url;
        public static EasyAPIs Instance;

        [HideInInspector]
        public string AuthenticationToken;

        void Awake()
        {
            Instance = this;
            Utilities.ValidateForNull(Url);
        }

        

        private IEnumerator CallAPIInternal<T,P>(string apiname, HttpMethod method, Action<CallbackResponse<T>> onInvokeAPICompleted, P instance = null)
           where T : class
           where P : class
        {
            string json = null;
            if (instance != null) json = JsonUtility.ToJson(instance);

            using (UnityWebRequest www = Utilities.BuildWebRequest(GetApiUrl(apiname),
                method.ToString(), json, AuthenticationToken))
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
                    {
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


        private string GetApiUrl(string apiname)
        {
            return string.Format("{0}/api/{1}", Url, apiname);
        }
    }
}
