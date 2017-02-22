using UnityEngine;
using System.Collections;
using AzureServicesForUnity.Shared;
using UnityEngine.Networking;
using AzureServicesForUnity.Storage;
using System;


namespace AzureServicesForUnity.Storage
{
    public class TableStorage : MonoBehaviour
    {
        //public string Url;
        public static TableStorage Instance;

        private string accountName;
        public void SetAccountName(string accountName)
        {
            this.accountName = accountName;
            Url = string.Format("https://{0}.table.core.windows.net/", accountName);
        }

        [HideInInspector]
        public string AuthenticationToken;

        [HideInInspector]
        public string SASToken;

        void Awake()
        {
            Instance = this;
        }

        private string Url;
       

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

        public void UpdateEntity<T>(T instance, string tableName, Action<CallbackResponse> onUpdateCompleted)
            where T : TableEntity
        {
            Utilities.ValidateForNull(instance, tableName, onUpdateCompleted, instance.PartitionKey, instance.RowKey);
            StartCoroutine(UpdateInternal(instance, tableName, onUpdateCompleted));
        }

        public void InsertOrMergeEntity<T>(T instance, string tableName, Action<CallbackResponse> onInsertOrMergeCompleted)
            where T : TableEntity
        {
            Utilities.ValidateForNull(instance, tableName, onInsertOrMergeCompleted, instance.PartitionKey, instance.RowKey);
            StartCoroutine(InsertOrMergeInternal(instance, tableName, onInsertOrMergeCompleted));
        }

        private IEnumerator InsertInternal<T>(T instance, string tableName, Action<CallbackResponse> onInsertCompleted)
        where T : TableEntity
        {
            string url = string.Format("{0}{1}()", Url, tableName);

            string json = JsonUtility.ToJson(instance);
            using (UnityWebRequest www =
                StorageUtilities.BuildStorageWebRequest(url, HttpMethod.Post.ToString(), accountName, json))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse<T> response = new CallbackResponse<T>();

                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error ?? "Error " + www.responseCode);
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

        private IEnumerator UpdateInternal<T>(T instance, string tableName, Action<CallbackResponse> onUpdateCompleted)
        where T : TableEntity
        {
            string url = string.Format("{0}{1}(PartitionKey='{2}', RowKey='{3}')", Url, tableName, instance.PartitionKey, instance.RowKey);

            string json = JsonUtility.ToJson(instance);
            using (UnityWebRequest www =
                StorageUtilities.BuildStorageWebRequest(url, HttpMethod.Put.ToString(), accountName, json))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse<T> response = new CallbackResponse<T>();

                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error ?? "Error " + www.responseCode);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }

                else if (www.downloadHandler != null)  //all OK
                {
                    if (Globals.DebugFlag) Debug.Log("successfully updated object");
                    response.Status = CallBackResult.Success;
                }
                onUpdateCompleted(response);
            }
        }

        private IEnumerator InsertOrMergeInternal<T>(T instance, string tableName, Action<CallbackResponse> onInsertOrMergeCompleted)
        where T : TableEntity
        {
            string url = string.Format("{0}{1}(PartitionKey='{2}', RowKey='{3}')", Url, tableName, instance.PartitionKey, instance.RowKey);

            string json = JsonUtility.ToJson(instance);
            using (UnityWebRequest www =
                StorageUtilities.BuildStorageWebRequest(url, HttpMethod.Merge.ToString(), accountName, json))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse<T> response = new CallbackResponse<T>();

                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error ?? "Error " + www.responseCode);
                    Utilities.BuildResponseObjectOnFailure(response, www);
                }

                else if (www.downloadHandler != null)  //all OK
                {
                    if (Globals.DebugFlag) Debug.Log("successfully inserted or merged object");
                    response.Status = CallBackResult.Success;
                }
                onInsertOrMergeCompleted(response);
            }
        }

        public IEnumerator DeleteTableInternal(string tableName, Action<CallbackResponse> onCreateTableCompleted)
        {
            string url = string.Format("{0}Tables('{1}')", Url, tableName);

            using (UnityWebRequest www = StorageUtilities.BuildStorageWebRequest(url, HttpMethod.Delete.ToString(), accountName, string.Empty))

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
                onCreateTableCompleted(response);
            }
        }

        public IEnumerator CreateTableIfNotExistsInternal(string tableName, Action<CallbackResponse> onCreateTableCompleted)
        {
            string url = Url + "Tables()";

            string json = string.Format("{{\"TableName\":\"{0}\"}}", tableName);
            using (UnityWebRequest www = StorageUtilities.BuildStorageWebRequest(url, HttpMethod.Post.ToString(), accountName, json))

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
                onCreateTableCompleted(response);
            }
        }

        private IEnumerator QueryTableInternal<T>(string query, string tableName, Action<CallbackResponse<T[]>> onQueryTableCompleted)
        where T : TableEntity
        {
            string url = string.Format("{0}{1}()?{2}", Url, tableName, query);


            using (UnityWebRequest www =
                StorageUtilities.BuildStorageWebRequest(url, HttpMethod.Get.ToString(), accountName, string.Empty))
            {
                yield return www.Send();
                if (Globals.DebugFlag) Debug.Log(www.responseCode);

                CallbackResponse<T[]> response = new CallbackResponse<T[]>();

                if (Utilities.IsWWWError(www))
                {
                    if (Globals.DebugFlag) Debug.Log(www.error ?? "Error " + www.responseCode);
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
