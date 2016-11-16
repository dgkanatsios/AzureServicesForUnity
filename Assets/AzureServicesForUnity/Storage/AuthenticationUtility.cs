using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace AzureServicesForUnity.Storage
{
    internal static class AuthenticationUtility
    {
        private const int ExpectedResourceStringLength = 100;
        private const int ExpectedHeaderNameAndValueLength = 50;
        private const char HeaderNameValueSeparator = ':';
        private const char HeaderValueDelimiter = ',';


        //public static string GetPreferredDateHeaderValue(HttpWebRequest request)
        //{
        //    string microsoftDateHeaderValue = request.Headers[Constants.HeaderConstants.Date];
        //    if (!string.IsNullOrEmpty(microsoftDateHeaderValue))
        //    {
        //        return microsoftDateHeaderValue;
        //    }

        //    return request.Headers[HttpRequestHeader.Date];
        //}


        //public static void AppendCanonicalizedContentLengthHeader(CanonicalizedString canonicalizedString, HttpWebRequest request)
        //{
        //    if (request.ContentLength != -1L && request.ContentLength != 0)
        //    {
        //        canonicalizedString.AppendCanonicalizedElement(request.ContentLength.ToString(CultureInfo.InvariantCulture));
        //    }
        //    else
        //    {
        //        canonicalizedString.AppendCanonicalizedElement(null);
        //    }
        //}


        public static void AppendCanonicalizedDateHeader(CanonicalizedString canonicalizedString, UnityWebRequest request, bool allowMicrosoftDateHeader = false)
        {
            string microsoftDateHeaderValue = request.GetRequestHeader("x-ms-date");
            if (string.IsNullOrEmpty(microsoftDateHeaderValue))
            {
                canonicalizedString.AppendCanonicalizedElement(request.GetRequestHeader("Date"));
            }
            else if (allowMicrosoftDateHeader)
            {
                canonicalizedString.AppendCanonicalizedElement(microsoftDateHeaderValue);
            }
            else
            {
                canonicalizedString.AppendCanonicalizedElement(null);
            }
        }

        //public static void AppendCanonicalizedCustomHeaders(CanonicalizedString canonicalizedString, HttpWebRequest request)
        //{
        //    List<string> headerNames = new List<string>(request.Headers.AllKeys.Length);
        //    foreach (string headerName in request.Headers.AllKeys)
        //    {
        //        if (headerName.StartsWith(Constants.HeaderConstants.PrefixForStorageHeader, StringComparison.OrdinalIgnoreCase))
        //        {
        //            headerNames.Add(headerName.ToLowerInvariant());
        //        }
        //    }

        //    CultureInfo sortingCulture = new CultureInfo("en-US");
        //    StringComparer sortingComparer = StringComparer.Create(sortingCulture, false);
        //    headerNames.Sort(sortingComparer);

        //    StringBuilder canonicalizedElement = new StringBuilder(ExpectedHeaderNameAndValueLength);
        //    foreach (string headerName in headerNames)
        //    {
        //        string value = request.Headers[headerName];
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            canonicalizedElement.Length = 0;
        //            canonicalizedElement.Append(headerName);
        //            canonicalizedElement.Append(HeaderNameValueSeparator);
        //            canonicalizedElement.Append(value.TrimStart().Replace("\r\n", string.Empty));

        //            canonicalizedString.AppendCanonicalizedElement(canonicalizedElement.ToString());
        //        }
        //    }
        //}

        //public static string GetCanonicalizedHeaderValue(DateTimeOffset? value)
        //{
        //    if (value.HasValue)
        //    {
        //        return HttpWebUtility.ConvertDateTimeToHttpString(value.Value);
        //    }

        //    return null;
        //}

        private static string GetAbsolutePathWithoutSecondarySuffix(Uri uri, string accountName)
        {
            string absolutePath = uri.AbsolutePath;
            string secondaryAccountName = string.Concat(accountName, "-secondary"/*CloudStorageAccount.SecondaryLocationAccountSuffix*/);

            int startIndex = absolutePath.IndexOf(secondaryAccountName, StringComparison.OrdinalIgnoreCase);
            if (startIndex == 1)
            {
                startIndex += accountName.Length;
                absolutePath = absolutePath.Remove(startIndex, 10/*CloudStorageAccount.SecondaryLocationAccountSuffix.Length*/);
            }

            return absolutePath;
        }

        public static string GetCanonicalizedResourceString(Uri uri, string accountName, bool isSharedKeyLiteOrTableService = false)
        {
            StringBuilder canonicalizedResource = new StringBuilder(ExpectedResourceStringLength);
            canonicalizedResource.Append('/');
            canonicalizedResource.Append(accountName);
            canonicalizedResource.Append(GetAbsolutePathWithoutSecondarySuffix(uri, accountName));

            IDictionary<string, string> queryParameters = new Dictionary<string,string>();// HttpWebUtility.ParseQueryString(uri.Query);
            if (!isSharedKeyLiteOrTableService)
            {
                List<string> queryParameterNames = new List<string>(queryParameters.Keys);
                queryParameterNames.Sort(StringComparer.OrdinalIgnoreCase);

                foreach (string queryParameterName in queryParameterNames)
                {
                    canonicalizedResource.Append('\n');
                    canonicalizedResource.Append(queryParameterName.ToLowerInvariant());
                    canonicalizedResource.Append(':');
                    canonicalizedResource.Append(queryParameters[queryParameterName]);
                }
            }
            else
            {
                // Add only the comp parameter
                string compQueryParameterValue;
                if (queryParameters.TryGetValue("comp", out compQueryParameterValue))
                {
                    canonicalizedResource.Append("?comp=");
                    canonicalizedResource.Append(compQueryParameterValue);
                }
            }

            return canonicalizedResource.ToString();
        }
    }
}


