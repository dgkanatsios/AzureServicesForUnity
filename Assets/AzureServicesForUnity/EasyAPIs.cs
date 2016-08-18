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


        public void CallAPI<T>(string apiname, HttpMethod method, Action<CallbackResponse<T>> onInvokeAPICompleted)
            where T : class
        {
            Utilities.ValidateForNull(apiname, onInvokeAPICompleted);
            StartCoroutine(CallAPIInternal(apiname, method, onInvokeAPICompleted));
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

        

        private IEnumerator CallAPIInternal<T>(string apiname, HttpMethod method, Action<CallbackResponse<T>> onInvokeAPICompleted)
           where T : class
        {
            using (UnityWebRequest www = Utilities.BuildWebRequest(GetApiUrl(apiname),
                method.ToString(), null, AuthenticationToken))
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


        private string GetApiUrl(string apiname)
        {
            return string.Format("{0}/api/{1}", Url, apiname);
        }
    }
}
