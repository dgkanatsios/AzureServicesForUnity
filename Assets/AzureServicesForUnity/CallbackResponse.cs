using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureServicesForUnity
{
    public class CallbackResponse
    {
        public CallBackResult Status { get; set; }
        public Exception Exception { get; set; }
    }

    public class CallbackResponse<V> : CallbackResponse
    {
        public V Result { get; set; }
    }

    public enum CallBackResult
    {
        Success,
        Failure,
        DeserializationFailure
    }
}
