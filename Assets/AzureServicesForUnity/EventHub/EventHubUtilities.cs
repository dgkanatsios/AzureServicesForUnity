using AzureServicesForUnity.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AzureServicesForUnity.EventHub
{
    public class EventHubUtilities
    {
        public static UnityWebRequest BuildEventHubWebRequest(string url, string method, string json, string key, string keyName)
        {
            UnityWebRequest www = new UnityWebRequest(url, method);

            www.SetRequestHeader(Globals.Accept, Globals.ApplicationJson);
            www.SetRequestHeader(Globals.Content_Type, Globals.ApplicationJson);

            string authorization = createToken(url, keyName, key);

            www.SetRequestHeader("Authorization", authorization.Trim());

            if (Globals.DebugFlag) Debug.Log("Authorization=" + authorization);
            if (Globals.DebugFlag) Debug.Log("URL=" + www.url);

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

        //https://docs.microsoft.com/en-us/rest/api/eventhub/generate-sas-token
        //BAD PRACTICE - THIS SHOULD BE GENERATED SERVER SIDE!!!!
        //this is included here just for the sake of the demo
        //generating the token on the client side induces several security risks
        //the proper way would be to run createToken function on the server side, get it on the client and use it
        private static string createToken(string resourceUri, string keyName, string key)
        {
            TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var week = 60 * 60 * 24 * 7;
            var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + week);
            string stringToSign = Uri.EscapeDataString(resourceUri) + "\n" + expiry;

            string signature = Utilities.ComputeHmac256(Encoding.UTF8.GetBytes(key), stringToSign);

            var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", Uri.EscapeDataString(resourceUri), Uri.EscapeDataString(signature), expiry, keyName);
            return sasToken;
        }
    }
}
