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
