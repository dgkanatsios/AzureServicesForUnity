// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AzureServicesForUnity.Shared.QueryHelpers.Other
{
    /// <summary>
    /// Exception type representing exceptions in parsing of OData queries.
    /// </summary>
    public class EasyTableODataException : InvalidOperationException
    {
        /// <summary>
        /// The position in string where error exists
        /// </summary>
        public int ErrorPosition { get; private set; }

        
        public EasyTableODataException() : base()
        {
        }
       
        public EasyTableODataException(string message, int errorPos)
            : this(message, errorPos, null)
        {
        }
        
     
        public EasyTableODataException(string message, int errorPos, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorPosition = errorPos;
        }
    }
}
