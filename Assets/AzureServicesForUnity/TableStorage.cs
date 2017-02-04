using UnityEngine;
using System.Collections;
using AzureServicesForUnity.Shared;
using UnityEngine.Networking;
using AzureServicesForUnity.Storage;
using System;
using AzureServicesForUnity.Shared;

namespace AzureServicesForUnity
{
    public class TableStorage : MonoBehaviour
    {
        //public string Url;
        public static TableStorage Instance;

        [HideInInspector]
        public string AuthenticationToken;

        void Awake()
        {
            Instance = this;
            Url = string.Format("https://{0}.table.core.windows.net/", AccountName);
        }

        private string Url;
        public string AccountName = "unitystorage2";

        public void QueryTable<T>(string query, string tableName, Action<CallbackResponse<T[]>> onQueryTableCompleted)
            where T : TableEntity
        {
            Utilities.ValidateForNull(query, tableName, onQueryTableCompleted);
            StartCoroutine(QueryTableInternal(query, tableName, onQueryTableCompleted));
        }

    

        public void CreateTableIfNotExists(string tableName, Action<CallbackResponse> onCreateTableCompleted)
        {
            StartCoroutine(CreateTableIfNotExistsInternal(tableName, onCreateTableCompleted));
        }

        public void DeleteTable(string tableName, Action<CallbackResponse> onDeleteTableCompleted)
        {
            StartCoroutine(DeleteTableInternal(tableName, onDeleteTableCompleted));
        }

        public void InsertEntity<T>(T instance, string tableName, Action<CallbackResponse> onInsertCompleted)
            where T : TableEntity
        {
            Utilities.ValidateForNull(instance, tableName, onInsertCompleted);
            StartCoroutine(InsertInternal(instance, tableName, onInsertCompleted));
        }

        private IEnumerator InsertInternal<T>(T instance, string tableName, Action<CallbackResponse> onInsertCompleted)
        where T : TableEntity
        {
            string url = string.Format("{0}{1}()", Url, tableName);

            string json = JsonUtility.ToJson(instance);
            using (UnityWebRequest www =
                StorageUtilities.BuildStorageWebRequest(url, HttpMethod.Post.ToString(), AccountName, json))
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
                    //let's get the header for the new object that was created

                    string dataServiceId = www.GetResponseHeader("Location");
                    if (Globals.DebugFlag) Debug.Log("new object ID is " + dataServiceId);
                    response.Status = CallBackResult.Success;
                }
                onInsertCompleted(response);
            }
        }

        public IEnumerator DeleteTableInternal(string tableName, Action<CallbackResponse> onCreateTableCompleted)
        {
            string url = string.Format("{0}Tables('{1}')", Url, tableName);

            using (UnityWebRequest www = StorageUtilities.BuildStorageWebRequest(url, HttpMethod.Delete.ToString(), AccountName, string.Empty))

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
                onCreateTableCompleted(response);
            }
        }

        public IEnumerator CreateTableIfNotExistsInternal(string tableName, Action<CallbackResponse> onCreateTableCompleted)
        {
            string url = Url + "Tables()";

            string json = string.Format("{{\"TableName\":\"{0}\"}}", tableName);
            using (UnityWebRequest www = StorageUtilities.BuildStorageWebRequest(url, HttpMethod.Post.ToString(), AccountName, json))

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
                onCreateTableCompleted(response);
            }
        }

        private IEnumerator QueryTableInternal<T>(string query, string tableName, Action<CallbackResponse<T[]>> onQueryTableCompleted)
        where T : TableEntity
        {
            string url = string.Format("{0}{1}()?{2}", Url, tableName, query);


            using (UnityWebRequest www =
                StorageUtilities.BuildStorageWebRequest(url, HttpMethod.Get.ToString(), AccountName, string.Empty))
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
                    //let's get the header for the new object that was created

                    T[] data = JsonHelper.GetJsonArrayFromTableStorage<T>(www.downloadHandler.text);
                    if (Globals.DebugFlag) Debug.Log("Received " + data.Length + " objects");

                    foreach (var item in data)
                    {
                        Debug.Log(string.Format("Item with PartitionKey {0} and RowKey {1}", item.PartitionKey, item.RowKey));
                    }
                    response.Status = CallBackResult.Success;
                }
                onQueryTableCompleted(response);
            }
        }
    }

}
