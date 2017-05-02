using AzureServicesForUnity.EventHub;
using AzureServicesForUnity.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AzureServicesForUnity.EventHub
{
    public class EventHubClient : MonoBehaviour
    {
        //documentation: https://docs.microsoft.com/en-us/rest/api/eventhub/send-event

        public static EventHubClient Instance;

        //Event Hubs authentication overview

        [HideInInspector]
        //type is https://{servicebusNamespace}.servicebus.windows.net/{eventHubPath}/messages
        public string Url = "ENTER YOUR URL HERE";

        [HideInInspector]
        public string Key = "ENTER YOUR KEY HERE";

        [HideInInspector]
        public string KeyName = "Send"; //depends on the key

        void Awake()
        {
            Instance = this;
        }

        public void PostData<T>(T data, Action<CallbackResponse> onPostDataCompleted)
            where T:class
        {
            Utilities.ValidateForNull(data, onPostDataCompleted);
            StartCoroutine(PostDataInternal(data, onPostDataCompleted));
        }

        private IEnumerator PostDataInternal<T>(T data, Action<CallbackResponse> onPostDataCompleted)
            where T : class
        {

            string json = JsonUtility.ToJson(data);
            
            using (UnityWebRequest www = EventHubUtilities.BuildEventHubWebRequest(Url, HttpMethod.Post.ToString(), json, Key, KeyName))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse response = new CallbackResponse();

                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error ?? "Error " + www.responseCode);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }
                else
                {
                    response.Status = CallBackResult.Success;
                }
                onPostDataCompleted(response);
            }
        }

        
    }
}