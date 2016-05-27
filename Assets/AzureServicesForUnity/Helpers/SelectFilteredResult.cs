using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureServicesForUnity.Helpers
{
    [Serializable]
    public class SelectFilteredResult<T>
    {
        /// <summary>
        /// If IncludeTotalCount() is not called, it has the value -1. If it is called, it has the value of the total number of rows
        /// </summary>
        public int count;
        public T[] results;

    }
}
