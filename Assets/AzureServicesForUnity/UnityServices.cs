using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Experimental.Networking;
using System.Linq;

namespace AzureServicesForUnity
{
    public class UnityServices : MonoBehaviour
    {
        public string Url = "https://unityservicesdemo.azurewebsites.net/tables/";
        public bool DebugFlag = true;

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

        public void Insert<T>(T instance, Action<T> onSuccess = null, Action<string> onError = null)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(instance);
            StartCoroutine(InsertInternal(instance, onSuccess, onError));
        }

        public void Select<T>(IQueryable<T> query, Action<T[]> onSuccess, Action<string> onError = null)
            where T : AzureObjectBase
        {
            StartCoroutine(SelectInternal(query, onSuccess, onError));
        }

        public void SelectSingle<T>(string id, Action<T> onSuccess, Action<string> onError = null)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(id);
            StartCoroutine(SelectSingleInternal<T>(id, onSuccess, onError));
        }

        public void UpdateObject<T>(T instance, Action<T> onSuccess = null, Action<string> onError = null)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(instance);
            StartCoroutine(UpdateInternal(instance, onSuccess, onError));
        }

        public void Delete<T>(string id, Action onSuccess = null, Action<string> onError = null)
            where T : AzureObjectBase
        {
            Utilities.ValidateForNull(id);
            StartCoroutine(DeleteInternal<Highscore>(id, onSuccess, onError));
        }

        private IEnumerator DeleteInternal<T>(string id, Action onSuccess = null, Action<string> onError = null)
            where T : AzureObjectBase
        {
            using (UnityWebRequest www = BuildWebRequest<T>(GetRemoteUrl<T>() + "/" + WWW.EscapeURL(id), "DELETE", null))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);
                if (Utilities.IsWWWError(www))
                {
                    if (DebugFlag) Debug.Log(www.error);
                    if (onError != null) onError(www.error ?? Constants.ErrorOccurred);
                }
                else
                {
                    if (onSuccess != null)
                        onSuccess();
                }
            }
        }

        private IEnumerator InsertInternal<T>(T instance, Action<T> onSuccess = null, Action<string> onError = null)
          where T : AzureObjectBase
        {
            string json = JsonUtility.ToJson(instance);

            using (UnityWebRequest www = BuildWebRequest<T>(GetRemoteUrl<T>(), "POST", json))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);
                if (Utilities.IsWWWError(www))
                {
                    if (DebugFlag) Debug.Log(www.error);
                    if (onError != null)
                        onError(www.error ?? Constants.ErrorOccurred);
                }

                else if (www.downloadHandler != null)  //all OK
                {
                    //let's get the new object that was created
                    T newObject = JsonUtility.FromJson<T>(www.downloadHandler.text);
                    if(DebugFlag) Debug.Log("new object ID is " + newObject.id);
                }
            }

        }

        private IEnumerator SelectSingleInternal<T>(string id, Action<T> onSuccess, Action<string> onError = null)
            where T : AzureObjectBase
        {
            using (UnityWebRequest www = BuildWebRequest<T>(GetRemoteUrl<T>() + "/" + WWW.EscapeURL(id), "GET", null))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);
                if (Utilities.IsWWWError(www))
                {
                    if (DebugFlag) Debug.Log(www.error);
                    if (onError != null) onError(www.error ?? Constants.ErrorOccurred);
                }
                else
                {
                    T data = JsonUtility.FromJson<T>(www.downloadHandler.text);
                    if (data != null)
                        onSuccess(data);
                }
            }
        }

        private IEnumerator SelectInternal<T>(IQueryable<T> query, Action<T[]> onSuccess, Action<string> onError = null)
           where T : AzureObjectBase
        {
            string url = GetRemoteUrl<T>();
            if (query != null)
            {
                url += query.ToString();
            }
            if (DebugFlag) Debug.Log(url);
            using (UnityWebRequest www = BuildWebRequest<T>(url, "GET", null))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);
                if (www.isError || www.downloadHandler == null)
                {
                    if (DebugFlag) Debug.Log(www.error);
                    if (onError != null)
                        onError(www.error ?? Constants.ErrorOccurred);
                }
                else
                {
                    T[] data = Utilities.DeserializeJsonArray<T>(www.downloadHandler.text);
                    if (data != null)
                        onSuccess(data);
                }
            }
        }

        private IEnumerator UpdateInternal<T>(T instance, Action<T> onSuccess = null, Action<string> onError = null)
           where T : AzureObjectBase
        {
            string json = JsonUtility.ToJson(instance);
            using (UnityWebRequest www = BuildWebRequest<T>(GetRemoteUrl<T>(), "PATCH", json))
            {
                yield return www.Send();
                if (DebugFlag) Debug.Log(www.responseCode);
                if (www.isError || www.downloadHandler == null)
                {
                    if (DebugFlag) Debug.Log(www.error);
                    if (onError != null) onError(www.error ?? Constants.ErrorOccurred);
                }
                else if (www.downloadHandler != null)  //all OK
                {
                    //let's get the new object that was created
                    T updatedObject = JsonUtility.FromJson<T>(www.downloadHandler.text);
                    if (DebugFlag) Debug.Log("updated object ID is " + updatedObject.id);
                    if (onSuccess != null)
                    {
                        onSuccess(updatedObject);
                    }
                }
            }

        }

        private UnityWebRequest BuildWebRequest<T>(string url, string method, string json)
            where T : AzureObjectBase
        {
            UnityWebRequest www = new UnityWebRequest(url, method);

            www.SetRequestHeader("Accept", "application/json");
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader(Constants.ZumoString, Constants.ZumoVersion);
            www.downloadHandler = new DownloadHandlerBuffer();
            if (json != null)
            {
                byte[] payload = System.Text.Encoding.UTF8.GetBytes(json);
                UploadHandler handler = new UploadHandlerRaw(payload);
                handler.contentType = "application/json";
                www.uploadHandler = handler;
            }
            return www;

        }

        private string GetRemoteUrl<T>()
        {
            return Url + typeof(T).Name;
        }

    }
}

