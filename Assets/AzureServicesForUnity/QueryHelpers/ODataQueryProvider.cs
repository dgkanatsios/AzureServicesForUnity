using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace AzureServicesForUnity.QueryHelpers
{
    //great stuff here: https://blogs.msdn.microsoft.com/mattwar/page/2/
    public class ODataQueryProvider : QueryProviderBase
    {
        public ODataQueryProvider()
        {
        }
        public override string GetQueryText(Expression expression)
        {
            return this.Translate(expression);
        }
        public override object Execute(Expression expression)
        {
            string command = this.Translate(expression);
            return command;
        }
        private string Translate(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            return new QueryTranslator().Translate(expression);
        }
    }
}
