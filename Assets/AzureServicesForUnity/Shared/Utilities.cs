using AzureServicesForUnity.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

#if NETFX_CORE
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

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
            else if (www.responseCode == 409L)
                response.Status = CallBackResult.ResourceExists;
            else
                response.Status = CallBackResult.Failure;

            string errorMessage = www.error;
            if (errorMessage == null && www.downloadHandler != null && !string.IsNullOrEmpty(www.downloadHandler.text))
                errorMessage = www.downloadHandler.text;
            else
                errorMessage = Globals.ErrorOccurred;

            Exception ex = new Exception(errorMessage);
            response.Exception = ex;
        }

        public static void BuildResponseObjectOnException(CallbackResponse response, Exception ex)
        {
            response.Status = CallBackResult.LocalException;
            response.Exception = ex;
        }

        public static string ComputeHmac256(byte[] key, string message)
        {
#if NETFX_CORE

			MacAlgorithmProvider provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
			CryptographicHash hash = provider.CreateHash(key.AsBuffer());
			hash.Append(CryptographicBuffer.ConvertStringToBinary(message, BinaryStringEncoding.Utf8));
			return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
#else
            using (HashAlgorithm hashAlgorithm = new HMACSHA256(key))
            {
                byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
                return Convert.ToBase64String(hashAlgorithm.ComputeHash(messageBuffer));
            }
#endif
        }

        public static string GetAuthorization(string accountName, string signature)
        {
            return
               string.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}", "SharedKey", accountName, signature);
        }

        public static string CanonicalizeHttpRequest(UnityWebRequest request, string accountName)
        {
            CanonicalizedString canonicalizedString = new CanonicalizedString(request.method, 200);

            canonicalizedString.AppendCanonicalizedElement(request.GetRequestHeader("ContentMd5"));
            canonicalizedString.AppendCanonicalizedElement(request.GetRequestHeader("Content-Type"));

            AuthenticationUtility.AppendCanonicalizedDateHeader(canonicalizedString, request, true);

            string resourceString = AuthenticationUtility.GetCanonicalizedResourceString(new Uri(request.url), accountName, true);
            canonicalizedString.AppendCanonicalizedElement(resourceString);

            return canonicalizedString.ToString();
        }

    }

  

    public enum HttpMethod
    {
        Post,
        Get,
        Patch,
        Delete,
        Put,
        Merge
    }
}

