// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AzureServicesForUnity.AppService.QueryHelpers.OData
{
    internal class QueryToken
    {
        public int Position { get; set; }
        public string Text { get; set; }
        public QueryTokenKind Kind { get; set; }
    }
}
