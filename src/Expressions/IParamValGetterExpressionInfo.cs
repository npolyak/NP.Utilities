// (c) Nick Polyak 2018 - http://awebpros.com/
// License: MIT License https://opensource.org/licenses/MIT
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author(s) of this software if something goes wrong. 
// 
// Also as a courtesy, please, mention this software in any documentation for the 
// products that use it.

using System.Linq.Expressions;

namespace NP.Utilities.Expressions
{
    /// <summary>
    /// used in order to plugin parameter value getter
    /// into a MethodCall expression, instead of ParamExpresion
    /// </summary>
    public interface IParamValGetterExpressionInfo
    {
        string ParamName { get; }

        Expression ValueGetterExpression { get; }
    }

    public class ParamValGetterExpressionInfo : IParamValGetterExpressionInfo
    {
        public string ParamName { get; }

        public Expression ValueGetterExpression { get; }

        public ParamValGetterExpressionInfo(string paramName, Expression valueGetterExpression)
        {
            ParamName = paramName;
            ValueGetterExpression = valueGetterExpression;
        }
    }

    public static class ParamValGetterExpressionHelper
    {
        public static Expression GetCallExpression<T>(T methodContainer, string methodName)
        {
            Expression callExpression = Expression.Call
            (
                Expression.Constant(methodContainer),
                typeof(T).GetMethod(methodName)
            );

            return callExpression;
        }
    }
}
