using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace AzureServicesForUnity.Shared
{
    internal static class AuthenticationUtility
    {
        private const int ExpectedResourceStringLength = 100;
        private const int ExpectedHeaderNameAndValueLength = 50;
        private const char HeaderNameValueSeparator = ':';
        private const char HeaderValueDelimiter = ',';

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

        private static string GetAbsolutePathWithoutSecondarySuffix(Uri uri, string accountName)
        {
            string absolutePath = uri.AbsolutePath;
            string secondaryAccountName = string.Concat(accountName, "-secondary");

            int startIndex = absolutePath.IndexOf(secondaryAccountName, StringComparison.OrdinalIgnoreCase);
            if (startIndex == 1)
            {
                startIndex += accountName.Length;
                absolutePath = absolutePath.Remove(startIndex, 10);
            }

            return absolutePath;
        }

        public static string GetCanonicalizedResourceString(Uri uri, string accountName, bool isSharedKeyLiteOrTableService = false)
        {
            StringBuilder canonicalizedResource = new StringBuilder(ExpectedResourceStringLength);
            canonicalizedResource.Append('/');
            canonicalizedResource.Append(accountName);
            canonicalizedResource.Append(GetAbsolutePathWithoutSecondarySuffix(uri, accountName));

            IDictionary<string, string> queryParameters = new Dictionary<string, string>();
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


