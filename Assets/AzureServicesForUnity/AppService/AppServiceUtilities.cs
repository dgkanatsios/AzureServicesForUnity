using AzureServicesForUnity.AppService;
using AzureServicesForUnity.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace AzureServicesForUnity.AppService
{
    public static class AppServiceUtilities
    {
        /// <summary>
        /// Builds and returns a UnityWebRequest object
        /// </summary>
        /// <param name="url">Url to hit</param>
        /// <param name="method">POST,GET, etc.</param>
        /// <param name="json">Any JSON to send</param>
        /// <param name="authenticationToken">Authentication token for the headers</param>
        /// <returns>A UnityWebRequest object</returns>
        public static UnityWebRequest BuildAppServiceWebRequest(string url, string method, string json, string authenticationToken)
        {
            UnityWebRequest www = new UnityWebRequest(url, method);

            www.SetRequestHeader(Globals.Accept, Globals.ApplicationJson);
            www.SetRequestHeader(Globals.Content_Type, Globals.ApplicationJson);
            www.SetRequestHeader(Constants.ZumoString, Constants.ZumoVersion);

            if (!string.IsNullOrEmpty(authenticationToken))
                www.SetRequestHeader(Constants.ZumoAuth, authenticationToken.Trim());

            www.downloadHandler = new DownloadHandlerBuffer();

            if (!string.IsNullOrEmpty(json))
            {
                byte[] payload = Encoding.UTF8.GetBytes(json);
                UploadHandler handler = new UploadHandlerRaw(payload);
                handler.contentType = Globals.ApplicationJson;
                www.uploadHandler = handler;
            }
            return www;
        }
    }
}
