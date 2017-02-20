using AzureServicesForUnity.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AzureServicesForUnity.Storage
{
    public class StorageUtilities
    {
        public static string AcceptMinimalMetadata = "application/json;odata=minimalmetadata";
        public static string XMSVERSIONNAME = "x-ms-version";
        public static string XMSVERSIONVALUE = "2015-12-11";
        public static string XMSDATENAME = "x-ms-date";
        public static string AuthorizationHeaderName = "Authorization";

        
        public static UnityWebRequest BuildStorageWebRequest(string url,  string method, string accountName, string json)
        {
            UnityWebRequest www = new UnityWebRequest(url, method);

            www.SetRequestHeader(Globals.Accept, AcceptMinimalMetadata);
            www.SetRequestHeader(Globals.Content_Type, Globals.ApplicationJson);

            www.SetRequestHeader("MaxDataServiceVersion", "3.0;NetFx");

            www.SetRequestHeader(XMSVERSIONNAME, XMSVERSIONVALUE);

            www.SetRequestHeader(XMSDATENAME, DateTimeOffset.UtcNow.UtcDateTime.ToString("R", CultureInfo.InvariantCulture));

            string message = CanonicalizeHttpRequest(www, accountName);

            if (!string.IsNullOrEmpty(TableStorage.Instance.AuthenticationToken))
            {
                byte[] KeyValue = Convert.FromBase64String(TableStorage.Instance.AuthenticationToken);
                string signature = ComputeHmac256(KeyValue, message);

                www.SetRequestHeader(AuthorizationHeaderName, GetAuthorization(accountName, signature));
            }
            else if(!string.IsNullOrEmpty(TableStorage.Instance.SASToken))
            {
                if (url.Contains("?"))//already set via a query
                {
                    url += "&" + TableStorage.Instance.SASToken.Substring(1); //replace the "?" from SASToken with a "&"
                }
                else
                {
                    url += TableStorage.Instance.SASToken;
                }
                www.url = url; //re-set the URL
                Debug.Log(url);
            }

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

        internal static string ComputeHmac256(byte[] key, string message)
        {
            using (HashAlgorithm hashAlgorithm = new HMACSHA256(key))
            {
                byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
                return Convert.ToBase64String(hashAlgorithm.ComputeHash(messageBuffer));
            }
        }

        internal static string GetAuthorization(string accountName, string signature)
        {
            return
               //string.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}", this.canonicalizer.AuthorizationScheme, this.credentials.AccountName, signature)
               string.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}", "SharedKey", accountName, signature);
        }

        public static string CanonicalizeHttpRequest(UnityWebRequest request, string accountName)
        {
            // Add the method (GET, POST, PUT, or HEAD).
            CanonicalizedString canonicalizedString = new CanonicalizedString(request.method, 200);

            // Add the Content-* HTTP headers. Empty values are allowed.
            canonicalizedString.AppendCanonicalizedElement(request.GetRequestHeader("ContentMd5"));
            canonicalizedString.AppendCanonicalizedElement(request.GetRequestHeader("Content-Type"));

            // Add the Date HTTP header (or the x-ms-date header if it is being used)
            AuthenticationUtility.AppendCanonicalizedDateHeader(canonicalizedString, request, true);

            // Add the canonicalized URI element
            string resourceString = AuthenticationUtility.GetCanonicalizedResourceString(new Uri(request.url), accountName, true);
            canonicalizedString.AppendCanonicalizedElement(resourceString);

            return canonicalizedString.ToString();
        }
    }




}

public class StorageCredentials
{
    public string SASToken { get; private set; }

    public string AccountName { get; private set; }

    public bool IsAnonymous
    {
        get
        {
            return (this.SASToken == null) && (this.AccountName == null);
        }
    }

}

internal class CanonicalizedString
{
    private const int DefaultCapacity = 300;
    private const char ElementDelimiter = '\n';

    /// <summary>
    /// Stores the internal <see cref="StringBuilder"/> that holds the canonicalized string.
    /// </summary>
    private readonly StringBuilder canonicalizedString;

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonicalizedString"/> class.
    /// </summary>
    /// <param name="initialElement">The first canonicalized element to start the string with.</param>
    public CanonicalizedString(string initialElement)
        : this(initialElement, CanonicalizedString.DefaultCapacity)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CanonicalizedString"/> class.
    /// </summary>
    /// <param name="initialElement">The first canonicalized element to start the string with.</param>
    /// <param name="capacity">The starting size of the string.</param>
    public CanonicalizedString(string initialElement, int capacity)
    {
        this.canonicalizedString = new StringBuilder(initialElement, capacity);
    }

    /// <summary>
    /// Append additional canonicalized element to the string.
    /// </summary>
    /// <param name="element">An additional canonicalized element to append to the string.</param>
    public void AppendCanonicalizedElement(string element)
    {
        this.canonicalizedString.Append(CanonicalizedString.ElementDelimiter);
        this.canonicalizedString.Append(element);
    }

    /// <summary>
    /// Converts the value of this instance to a string.
    /// </summary>
    /// <returns>A string whose value is the same as this instance.</returns>
    public override string ToString()
    {
        return this.canonicalizedString.ToString();
    }
}


