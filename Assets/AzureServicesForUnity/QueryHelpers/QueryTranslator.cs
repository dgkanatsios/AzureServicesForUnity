using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace AzureServicesForUnity.QueryHelpers
{
    class QueryTranslator : ExpressionVisitor
    {
        StringBuilder sb;
        internal string Translate(Expression expression)
        {
            ODataQueryResult odqr = new MethodExpressionFinder().GetMethodExpression(expression);
            this.sb = new StringBuilder();
            if (odqr.WhereExpression != null)
            {
                sb.Append("&$filter=");
                this.Visit(StripQuotes(odqr.WhereExpression.Arguments[1]));
            }
            if(odqr.SkipExpression !=null)
            {
                sb.Append("&$skip=");
                this.Visit(StripQuotes(odqr.SkipExpression.Arguments[1]));
            }
            if (odqr.TakeExpression != null)
            {
                sb.Append("&$take=");
                this.Visit(StripQuotes(odqr.TakeExpression.Arguments[1]));
            }
            if (odqr.OrderByExpression != null || odqr.OrderByDescendingExpression != null)
            {
                sb.Append("&$orderby=");
                if (odqr.OrderByExpression != null)
                    this.Visit(StripQuotes(odqr.OrderByExpression.Arguments[1]));
                else if (odqr.OrderByDescendingExpression != null)
                {
                    this.Visit(StripQuotes(odqr.OrderByDescendingExpression.Arguments[1]));
                    sb.Append(" desc");
                }
            }
            if(odqr.SelectExpression!=null)
            {
                sb.Append("&$select=");
                var selectExpr = StripQuotes(odqr.SelectExpression.Arguments[1]);
                var propArgs = (NewExpression)((LambdaExpression)selectExpr).Body;
                for (int i = 0; i < propArgs.Arguments.Count; i++)
                {
                    var prop = propArgs.Arguments[i];
                    this.Visit(prop);
                    if (i != propArgs.Arguments.Count - 1)
                        sb.Append(",");
                }
            }
            sb = sb.Replace('&', '?', 0, 1);
            return this.sb.ToString();
        }
        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                sb.Append("&$filter=");
                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "OrderBy")
            {
                sb.Append("&$orderby=");
                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "OrderByDescending")
            {
                sb.Append("&$orderby=");
                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                sb.Append(" desc");
                return m;
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Select")
            {
                //do nothing
                return m;
            }
            else
            {
                throw new NotSupportedException(string.Format("The method ‘{ 0 }’ is not supported", m.Method.Name));
            }
        }
        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" not ");
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator ‘{ 0 }’ is not supported", u.NodeType));
            }
            return u;
        }
        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");
            this.Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    sb.Append(" and ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    sb.Append(" or ");
                    break;
                case ExpressionType.Equal:
                    sb.Append(" eq ");
                    break;
                case ExpressionType.NotEqual:
                    sb.Append(" ne ");
                    break;
                case ExpressionType.LessThan:
                    sb.Append(" lt ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(" le ");
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(" gt ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" ge ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator ‘{ 0 }’ is not supported", b.NodeType));
            }
            this.Visit(b.Right);
            sb.Append(")");
            return b;
        }
        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;
            if (q != null)
            {
            }
            else if (c.Value == null)
            {
                sb.Append("NULL");
            }
            else {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)c.Value) ? "true" : "false");
                        break;
                    case TypeCode.String:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for ‘{ 0}’ is not supported", c.Value));
                    default:
                        sb.Append(c.Value);
                        break;
                }
            }
            return c;
        }
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                sb.Append(m.Member.Name);
                return m;
            }
            throw new NotSupportedException(string.Format("The member ‘{ 0 }’ is not supported", m.Member.Name));
        }
    }
}
