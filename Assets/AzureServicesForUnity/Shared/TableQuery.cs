using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AzureServicesForUnity.Shared
{
    public class TableQuery
    {
        public string filter;
        public string orderBy;
        public uint top;
        public uint skip;
        public string select;
        public bool includeDeleted;
        public bool inlineCount;

        //check here for the options
        //https://msdn.microsoft.com/en-us/library/azure/jj677199.aspx

      
        public override string ToString()
        {
            string queryString = string.Empty;
            string q = "?";
            if (!string.IsNullOrEmpty(this.filter))
            {
                queryString += string.Format("{0}$filter=({1})", q, this.filter);
                q = "&";
            }
            if (!string.IsNullOrEmpty(this.orderBy))
            {
                queryString += string.Format("{0}$orderby={1}", q, this.orderBy);
                q = "&";
            }
            if (this.top > 0)
            {
                queryString += string.Format("{0}$top={1}", q, this.top.ToString());
                q = "&";
            }
            if (this.skip > 0)
            {
                queryString += string.Format("{0}$skip={1}", q, this.skip.ToString());
                q = "&";
            }
            if (!string.IsNullOrEmpty(this.select))
            {
                queryString += string.Format("{0}$select={1}", q, this.select);
                q = "&";
            }
            if (this.includeDeleted)
            {
                queryString += string.Format("{0}__includeDeleted=true", q);
                q = "&";
            }
            if(this.inlineCount)
            {
                queryString += String.Format("{0}$inlinecount=allpages", q);
            }
            return Uri.EscapeUriString(queryString);
        }
    }
}
