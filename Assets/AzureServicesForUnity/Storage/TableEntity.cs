using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureServicesForUnity.Shared
{
    [Serializable()]
    public class TableEntity
    {
        public TableEntity()
        {
        }

        public TableEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public string PartitionKey;

        public string RowKey;

    }


}
