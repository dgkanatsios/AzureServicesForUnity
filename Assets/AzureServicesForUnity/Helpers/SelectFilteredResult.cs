using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureServicesForUnity.Helpers
{
    [Serializable]
    public class SelectFilteredResult<T>
    {
        public int? count;
        public T[] results;

    }
}
