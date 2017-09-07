using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.AzureServicesForUnity.Shared
{
    //I wanted this library to be plugin-free, that's why I'm using native JsonUtility
    //however, you could take a look and use this one: https://github.com/SaladLab/Json.Net.Unity3D


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
            public T[] array = null;
        }

        [Serializable]
        private class TableStorageResult<T>
        {
            public T[] value = null;
        }

        public static T[] GetJsonArrayFromTableStorage<T>(string json)
        {
            TableStorageResult<T> result = JsonUtility.FromJson<TableStorageResult<T>>(json);
            return result.value;
        }
    }
}
