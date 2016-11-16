using AzureServicesForUnity.AppService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace AzureServicesForUnity.Shared
{
    public static class Utilities
    {
        public static void ValidateForNull(params object[] objects)
        {
            foreach (object obj in objects)
            {
                if (obj == null)
                {
                    throw new Exception("Argument null");
                }
            }

        }

        public static bool IsWWWError(UnityWebRequest www)
        {
            return www.isError || (www.responseCode >= 400L && www.responseCode <= 511L);
        }

        public static void BuildResponseObjectOnFailure(CallbackResponse response, UnityWebRequest www)
        {
            if (www.responseCode == 404L)
                response.Status = CallBackResult.NotFound;
            else
                response.Status = CallBackResult.Failure;

            string errorMessage = www.error;
            if (errorMessage == null && www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
                errorMessage = www.downloadHandler.text;
            else
                errorMessage = Constants.ErrorOccurred; 

            Exception ex = new Exception(errorMessage);
            response.Exception = ex;
        }

        public static void BuildResponseObjectOnException(CallbackResponse response, Exception ex)
        {
            response.Status = CallBackResult.LocalException;
            response.Exception = ex;
        }

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

            www.SetRequestHeader(Constants.Accept, Constants.ApplicationJson);
            www.SetRequestHeader(Constants.Content_Type, Constants.ApplicationJson);
            www.SetRequestHeader(Constants.ZumoString, Constants.ZumoVersion);

            if (!string.IsNullOrEmpty(authenticationToken))
                www.SetRequestHeader(Constants.ZumoAuth, authenticationToken.Trim());

            www.downloadHandler = new DownloadHandlerBuffer();

            if (!string.IsNullOrEmpty(json))
            {
                byte[] payload = Encoding.UTF8.GetBytes(json);
                UploadHandler handler = new UploadHandlerRaw(payload);
                handler.contentType = Constants.ApplicationJson;
                www.uploadHandler = handler;
            }
            return www;
        }
    }

    //http://forum.unity3d.com/threads/how-to-load-an-array-with-jsonutility.375735/#post-2585129
    public class JsonHelper
    {
        public static T[] GetJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }

    public enum HttpMethod
    {
        Post,
        Get,
        Patch,
        Delete,
        Put
    }
}

