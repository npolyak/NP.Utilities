using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities.Expressions
{

    public class ReplaceArgsExprStringBuilder : ExprStringBuilder
    {
        Dictionary<string, string> ParamMap { get; } = new Dictionary<string, string>();

        string[] ParamNames { get; }

        public ReplaceArgsExprStringBuilder(params string[] paramNames)
        {
            ParamNames = paramNames;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.Parameters.Count > ParamNames.Length)
                throw new Exception("Roxy Usage Error: the number of expression parameter cannot be greater than the number of parameters within the wrapper plus 1");

            for(int i = 0; i < node.Parameters.Count; i++)
            {
                ParamMap[node.Parameters[i].Name] = ParamNames[i];
            }

            return base.Visit(node.Body);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            string paramName = node.Name;

            string mappedName = ParamMap[paramName];

            Out(mappedName);

            return node;
        }
    }
}
