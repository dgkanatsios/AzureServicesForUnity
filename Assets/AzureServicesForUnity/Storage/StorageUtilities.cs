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

namespace AzureServicesForUnity.Storage
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

            string message = CanonicalizeHttpRequest(www, accountName);


            if (!string.IsNullOrEmpty(TableStorage.Instance.SASToken))
            {
                if (url.Contains("?"))//already set via query string
                {
                    url += "&" + TableStorage.Instance.SASToken.Substring(1); //replace the "?" from the existing SASToken with a "&"
                }
                else
                {
                    url += TableStorage.Instance.SASToken;
                }
                www.url = url; //re-set the URL
            }
            else if (!string.IsNullOrEmpty(TableStorage.Instance.AuthenticationToken))
            {
                //display a warning that this is a not recommended method
                Debug.Log("You're using Shared Key authentication. You're highly encouraged to switch to SAS authentication");
                byte[] KeyValue = Convert.FromBase64String(TableStorage.Instance.AuthenticationToken);
                string signature = ComputeHmac256(KeyValue, message);

                www.SetRequestHeader(AuthorizationHeaderName, GetAuthorization(accountName, signature));
            }
            else //if both SAS and authenticationtoken are empty, warn the user
            {
                Debug.Log("__BEWARE__! Both SAS and authentication tokens are not set, this request will be anonymous.");
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

        private static string ComputeHmac256(byte[] key, string message)
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

        private static string GetAuthorization(string accountName, string signature)
        {
            return
               string.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}", "SharedKey", accountName, signature);
        }

        private static string CanonicalizeHttpRequest(UnityWebRequest request, string accountName)
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


