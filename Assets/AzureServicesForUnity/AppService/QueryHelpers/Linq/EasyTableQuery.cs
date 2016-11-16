using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;


namespace AzureServicesForUnity.AppService.QueryHelpers.Linq
{
    public class EasyTableQuery<T> 
    {
        internal EasyTableQuery(EasyTableQueryProvider queryProvider,
                                         IQueryable<T> query,
                                         IDictionary<string, string> parameters,
                                         bool includeTotalCount)
        {
            this.RequestTotalCount = includeTotalCount;
            this.Parameters = parameters;
            this.Query = query;
            this.QueryProvider = queryProvider;
        }

        public override string ToString()
        {
            return ToODataString();
        }

        public string ToODataString()
        {
            return this.QueryProvider.ToODataString(this);
        }

        public bool RequestTotalCount { get; private set; }

        public IDictionary<string, string> Parameters { get; private set; }

        public IQueryable<T> Query { get; set; }

        public EasyTableQueryProvider QueryProvider { get; set; }

        public EasyTableQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return this.QueryProvider.Create<T>(Queryable.Where(this.Query, predicate), this.Parameters, this.RequestTotalCount);
        }

       public EasyTableQuery<U> Select<U>(Expression<Func<T, U>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            // Create a new table with the same name/client but
            // pretending to be of a different type since the query needs to
            // have the same type as the table.  This won't cause any issues
            // since we're only going to use it to evaluate the query and it'll
            // never leak to users.

            return this.QueryProvider.Create(Queryable.Select(this.Query, selector),
                                             this.Parameters,
                                             this.RequestTotalCount);
        }

       public EasyTableQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            return this.QueryProvider.Create(Queryable.OrderBy(this.Query, keySelector), this.Parameters, this.RequestTotalCount);
        }

        public EasyTableQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            return this.QueryProvider.Create(Queryable.OrderByDescending(this.Query, keySelector), this.Parameters, this.RequestTotalCount);
        }

        public EasyTableQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            return this.QueryProvider.Create( Queryable.ThenBy((IOrderedQueryable<T>)this.Query, keySelector), this.Parameters, this.RequestTotalCount);
        }

        public EasyTableQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            return this.QueryProvider.Create(Queryable.ThenByDescending((IOrderedQueryable<T>)this.Query, keySelector), this.Parameters, this.RequestTotalCount);
        }

        /// <summary>
        /// Applies the specified skip clause to the source query.
        /// </summary>
        /// <param name="count">
        /// The number to skip.
        /// </param>
        /// <returns>
        /// The composed query.
        /// </returns>
        public EasyTableQuery<T> Skip(int count)
        {
            return this.QueryProvider.Create(Queryable.Skip(this.Query, count), this.Parameters, this.RequestTotalCount);
        }

        /// <summary>
        /// Applies the specified take clause to the source query.
        /// </summary>
        /// <param name="count">
        /// The number to take.
        /// </param>
        /// <returns>
        /// The composed query.
        /// </returns>
        public EasyTableQuery<T> Take(int count)
        {
            return this.QueryProvider.Create(Queryable.Take(this.Query, count), this.Parameters, this.RequestTotalCount);
        }

        /// <summary>
        /// Applies to the source query the specified string key-value 
        /// pairs to be used as user-defined parameters with the request URI 
        /// query string.
        /// </summary>
        /// <param name="parameters">
        /// The parameters to apply.
        /// </param>
        /// <returns>
        /// The composed query.
        /// </returns>
        public EasyTableQuery<T> WithParameters(IDictionary<string, string> parameters)
        {
            if (parameters != null)
            {
                // Make sure to replace any existing value for the key
                foreach (KeyValuePair<string, string> pair in parameters)
                {
                    this.Parameters[pair.Key] = pair.Value;
                }
            }

            return this.QueryProvider.Create(this.Query, this.Parameters, this.RequestTotalCount);
        }

        /// <summary>
        /// Ensure the query will get the total count for all the records that
        /// would have been returned ignoring any take paging/limit clause
        /// specified by client or server.
        /// </summary>
        /// <returns>
        /// The query object.
        /// </returns>
        public EasyTableQuery<T> IncludeTotalCount()
        {
            return this.QueryProvider.Create(this.Query, this.Parameters, includeTotalCount: true);
        }

        /// <summary>
        /// Ensure the query will get the deleted records. This requires the soft delete feature to be enabled on the Mobile Service. Visit <see href="http://go.microsoft.com/fwlink/?LinkId=507647">the link</see> for details.
        /// </summary>
        /// <returns>
        /// The query object.
        /// </returns>
        public EasyTableQuery<T> IncludeDeleted()
        {
            return this.QueryProvider.Create(this.Query, this.Parameters, includeTotalCount: true);
        }



    }
}
