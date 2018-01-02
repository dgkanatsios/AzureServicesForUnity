using AzureServicesForUnity.Shared;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

#if NETFX_CORE
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace AzureServicesForUnity.Shared
{
    public class StorageUtilities
    {
        public static string AcceptMinimalMetadata = "application/json;odata=minimalmetadata";
        public static string XMSVERSIONNAME = "x-ms-version";
        public static string XMSVERSIONVALUE = "2015-12-11";
        public static string XMSDATENAME = "x-ms-date";
        public static string AuthorizationHeaderName = "Authorization";


        public static UnityWebRequest BuildStorageWebRequest(string url, string method, string accountName, string json)
        {
            UnityWebRequest www = new UnityWebRequest(url, method);

            www.SetRequestHeader(Globals.Accept, AcceptMinimalMetadata);
            www.SetRequestHeader(Globals.Content_Type, Globals.ApplicationJson);

            www.SetRequestHeader("MaxDataServiceVersion", "3.0;NetFx");

            www.SetRequestHeader(XMSVERSIONNAME, XMSVERSIONVALUE);

            www.SetRequestHeader(XMSDATENAME, DateTimeOffset.UtcNow.UtcDateTime.ToString("R", CultureInfo.InvariantCulture));

            string message = Utilities.CanonicalizeHttpRequest(www, accountName);


            if (!string.IsNullOrEmpty(TableStorageClient.Instance.SASToken))
            {
                if (TableStorageClient.Instance.EndpointStorageType == EndpointStorageType.CosmosDBTableAPI)
                {
                    //CosmosDB Table API works only with AuthenticationToken, listed as Key on Azure Portal
                    //you can use either the Primary key or the Secondary key
                    //https://docs.microsoft.com/en-us/azure/cosmos-db/create-table-dotnet#update-your-connection-string
                    string error = "CosmosDB Table API does not support SAS token, it works only with AuthenticationToken/Key, please fix this in the code and try again";
                    Debug.Log(error);
                    throw new Exception(error);
                }

                if (url.Contains("?"))//already set via query string
                {
                    url += "&" + TableStorageClient.Instance.SASToken.Substring(1); //replace the "?" from the existing SASToken with a "&"
                }
                else
                {
                    url += TableStorageClient.Instance.SASToken;
                }
                www.url = url; //re-set the URL
            }
            else if (!string.IsNullOrEmpty(TableStorageClient.Instance.AuthenticationToken))
            {
                //display a warning that this is a not recommended method
                //only displayed using Table Storage, since SAS is not supported in CosmosDB Table API
                if (Globals.DebugFlag && TableStorageClient.Instance.EndpointStorageType == EndpointStorageType.TableStorage)
                    Debug.Log("You're using Shared Key authentication. You're highly encouraged to switch to SAS authentication");
                byte[] KeyValue = Convert.FromBase64String(TableStorageClient.Instance.AuthenticationToken);
                string signature = Utilities.ComputeHmac256(KeyValue, message);

                www.SetRequestHeader(AuthorizationHeaderName, Utilities.GetAuthorization(accountName, signature));
            }
            else //if both SAS and authenticationtoken are empty, warn the user
            {
                if (Globals.DebugFlag) Debug.Log("__BEWARE__! Both SAS and authentication tokens are not set, this request will be anonymous.");
            }
            if (Globals.DebugFlag) Debug.Log("URL= " + www.url);
            www.downloadHandler = new DownloadHandlerBuffer();

            if (!string.IsNullOrEmpty(json))
            {
                byte[] payload = Encoding.UTF8.GetBytes(json);
                UploadHandler handler = new UploadHandlerRaw(payload)
                {
                    contentType = Globals.ApplicationJson
                };
                www.uploadHandler = handler;
            }
            return www;
        }


    }

}

public enum EndpointStorageType
{
    TableStorage,
    CosmosDBTableAPI
}

internal class CanonicalizedString
{
    private const int DefaultCapacity = 300;
    private const char ElementDelimiter = '\n';

    private readonly StringBuilder canonicalizedString;

    public CanonicalizedString(string initialElement)
        : this(initialElement, CanonicalizedString.DefaultCapacity)
    {
    }

    public CanonicalizedString(string initialElement, int capacity)
    {
        this.canonicalizedString = new StringBuilder(initialElement, capacity);
    }

    public void AppendCanonicalizedElement(string element)
    {
        this.canonicalizedString.Append(CanonicalizedString.ElementDelimiter);
        this.canonicalizedString.Append(element);
    }

    public override string ToString()
    {
        return this.canonicalizedString.ToString();
    }
}


