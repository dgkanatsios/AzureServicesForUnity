using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace AzureServicesForUnity.QueryHelpers
{
    public class ODataQueryResult
    {
        private Dictionary<string, MethodCallExpression> methodCallExpressions
            = new Dictionary<string, MethodCallExpression>();
        public ODataQueryResult()
        {
            methodCallExpressions.Add("Where", null);
            methodCallExpressions.Add("OrderBy", null);
            methodCallExpressions.Add("OrderByDescending", null);
            methodCallExpressions.Add("Take", null);
            methodCallExpressions.Add("Skip", null);
            methodCallExpressions.Add("Select", null);
        }
        public MethodCallExpression WhereExpression
        {
            get { return methodCallExpressions["Where"]; }
        }
        public MethodCallExpression OrderByExpression
        {
            get { return methodCallExpressions["OrderBy"]; }
        }
        public MethodCallExpression OrderByDescendingExpression
        {
            get { return methodCallExpressions["OrderByDescending"]; }
        }
        public MethodCallExpression TakeExpression
        {
            get { return methodCallExpressions["Take"]; }
        }
        public MethodCallExpression SkipExpression
        {
            get { return methodCallExpressions["Skip"]; }
        }
        public MethodCallExpression SelectExpression
        {
            get { return methodCallExpressions["Select"]; }
        }
        public bool ContainsKey(string key)
        {
            return methodCallExpressions.ContainsKey(key);
        }
        public MethodCallExpression this[string key]
        {
            get
            {
                return methodCallExpressions[key];
            }
            set
            {
                methodCallExpressions[key] = value;
            }
        }
    }
    public class MethodExpressionFinder : ExpressionVisitor
    {
        //private MethodCallExpression methodCallExpression = null;
        private ODataQueryResult odqr = new ODataQueryResult();
        public ODataQueryResult GetMethodExpression(Expression expression)
        {
            Visit(expression);
            return odqr;
        }
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (odqr.ContainsKey(expression.Method.Name))
            {
                if (odqr[expression.Method.Name] != null)//if already have been set
                    throw new Exception("Can only use one " + expression.Method.Name + " method call");
                odqr[expression.Method.Name] = expression;
            }
            Visit(expression.Arguments[0]);
            return expression;
        }
    }
}
