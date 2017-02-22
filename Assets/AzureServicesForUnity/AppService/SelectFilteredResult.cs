using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureServicesForUnity.AppService
{
    [Serializable]
    public class SelectFilteredResult<T>
    {
        /// <summary>
        /// If IncludeTotalCount() is called, it contains the total number of rows. If it is not called, it is -1
        /// </summary>
        public int count;
        public T[] results;

    }
}
