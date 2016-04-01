using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace AzureServicesForUnity.QueryHelpers
{
    public abstract class QueryProviderBase : IQueryProvider
    {
        protected QueryProviderBase()
        {
        }
        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new ODataQuery<S>(this, expression);
        }
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(ODataQuery<>).MakeGenericType(elementType), 
                    new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }
        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)this.Execute(expression);
        }
        object IQueryProvider.Execute(Expression expression)
        {
            return this.Execute(expression);
        }
        public abstract string GetQueryText(Expression expression);
        public abstract object Execute(Expression expression);
    }
}
