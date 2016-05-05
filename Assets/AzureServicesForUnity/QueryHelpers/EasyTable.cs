using AzureServicesForUnity;
using AzureServicesForUnity.QueryHelpers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AzureServicesForUnity.QueryHelpers
{
    public class EasyTable<T>
    {
        public string TableName { get; private set; }

        public EasyTable()
        {
            TableName = typeof(T).Name;
            QueryProvider = new EasyTableQueryProvider();
        }

        public EasyTableQueryProvider QueryProvider { get; private set; }

        public EasyTableQuery<T> CreateQuery()
        {
            return this.QueryProvider.Create(new T[0].AsQueryable(), new Dictionary<string, string>(),  false);
        }


        public EasyTableQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            return CreateQuery().Where(predicate);
        }

        public EasyTableQuery<U> Select<U>(Expression<Func<T, U>> selector)
        {
            return CreateQuery().Select(selector);
        }

        public EasyTableQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return CreateQuery().OrderBy(keySelector);
        }


        public EasyTableQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return CreateQuery().OrderByDescending(keySelector);
        }

        public EasyTableQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return CreateQuery().ThenBy(keySelector);
        }

        public EasyTableQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return CreateQuery().ThenByDescending(keySelector);
        }


        public EasyTableQuery<T> Skip(int count)
        {
            return CreateQuery().Skip(count);
        }


        public EasyTableQuery<T> Take(int count)
        {
            return CreateQuery().Take(count);
        }


        public EasyTableQuery<T> IncludeTotalCount()
        {
            return this.CreateQuery().IncludeTotalCount();
        }

        public EasyTableQuery<T> IncludeDeleted()
        {
            return this.CreateQuery().IncludeDeleted();
        }


        public EasyTableQuery<T> WithParameters(IDictionary<string, string> parameters)
        {
            return this.CreateQuery().WithParameters(parameters);
        }
    }
}
