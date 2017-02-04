// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using AzureServicesForUnity.Shared.QueryHelpers.Other;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;



namespace AzureServicesForUnity.Shared.QueryHelpers.Linq
{
    public class EasyTableQueryProvider
    {

        internal EasyTableQuery<T> Create<T>(
                                                        IQueryable<T> query,
                                                        IDictionary<string, string> parameters,
                                                        bool includeTotalCount)
        {

            Debug.Assert(query != null, "query cannot be null!");
            Debug.Assert(parameters != null, "parameters cannot be null!");

            // NOTE: Make sure any changes to this logic are reflected in the
            // Select method below which has its own version of this code to
            // work around type changes for its projection.
            return new EasyTableQuery<T>(
                
                this,
                query,
                parameters,
                includeTotalCount);
        }

     

        /// <summary>
        /// Compile the query into a EasyTableQueryDescription.
        /// </summary>
        /// <returns>
        /// The compiled OData query.
        /// </returns>
        internal EasyTableQueryDescription Compile<T>(EasyTableQuery<T> query)
        {
            // Compile the query from the underlying IQueryable's expression
            // tree
            EasyTableQueryTranslator<T> translator = new EasyTableQueryTranslator<T>(query);
            EasyTableQueryDescription compiledQuery = translator.Translate();

            return compiledQuery;
        }

        public string ToODataString<T>(EasyTableQuery<T> query)
        {
            EasyTableQueryDescription description = this.Compile(query);
            return description.ToODataString();
        }
    }
}
