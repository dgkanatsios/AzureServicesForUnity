using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Experimental.Networking;

namespace AzureServicesForUnity
{
    public static class Utilities
    {
        public static T[] DeserializeJsonArray<T>(string json)
        {
            json = json.Trim();
            if (json == "[]")
                return new T[0]; //just return an empty array
            else
            {
                //string will be of the format "[{...},{...},{...},{...}]"

                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                //http://stackoverflow.com/questions/20243621/regular-expression-for-a-json-type-string
                var match = Regex.Match(json, "({.*?})");

                List<T> objectInstances = new List<T>();

                for (int i = 0; i < match.Groups.Count; i++)
                {
                    try
                    {
                        objectInstances.Add(JsonUtility.FromJson<T>(match.Groups[i].Value));
                    }
                    catch
                    {
                        //if you cannot deserialize a single object contained in the array, suppress
                    }
                }
                return objectInstances.ToArray();
            }
        }

        public static void ValidateForNull(object obj)
        {
            if (obj == null)
            {
                throw new Exception("Argument null");
            }

        }

        public static bool IsWWWError(UnityWebRequest www)
        {
            return www.isError || (www.responseCode >= 400L && www.responseCode <= 500L);
        }

    }
}

