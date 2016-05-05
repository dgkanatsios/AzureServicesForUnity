// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AzureServicesForUnity.QueryHelpers.OData
{
    /// <summary>
    /// Enumeration of unary operators.
    /// </summary>
    public enum UnaryOperatorKind
    {
        /// <summary>
        /// The unary - operator.
        /// </summary>
        Negate,
        /// <summary>
        /// The not operator.
        /// </summary>
        Not
    }
}
