// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
using System.Linq.Expressions;

namespace NP.Utilities
{
    public static class ExpressionUtils
    {
        public static string GetMemberName(this LambdaExpression expression)
        {
            MemberExpression memberExpression =
                expression.Body as MemberExpression;

            if (memberExpression != null)
                return memberExpression?.Member?.Name;

            MethodCallExpression methodCallExpression =
                expression.Body as MethodCallExpression;

            return methodCallExpression?.Method.Name;
        }
    }
}
