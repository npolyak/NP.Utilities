// (c) Nick Polyak 2013 - http://awebpros.com/
//
// License: MIT License https://opensource.org/licenses/MIT//
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author(s) of this software if something goes wrong. 
// 
// Also as a courtesy, please, mention this software in any documentation for the 
// products that use it.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NP.Utilities.Expressions
{
    public static class CompiledExpressionUtils
    {
        static DoubleParamMap<Type, string, Func<object, object>> _untypedGettersCache =
            new DoubleParamMap<Type, string, Func<object, object>>();


        public static Func<object, object> GetUntypedCSPropertyGetterByObjType
        (
            this Type objType,
            string propertyName
        )
        {
            Func<object, object> result;

            if (_untypedGettersCache.TryGetValue(objType, propertyName, out result))
            {
                return result;
            }

            ParameterExpression paramExpression = Expression.Parameter(typeof(object));
            UnaryExpression typedObjectExpression = Expression.Convert(paramExpression, objType);

            Expression propertyGetterExpression =
                Expression.Property(typedObjectExpression, propertyName);

            UnaryExpression valueCastExpression = Expression.Convert(propertyGetterExpression, typeof(object));

            result = Expression.Lambda<Func<object, object>>(valueCastExpression, paramExpression).Compile();

            _untypedGettersCache.AddKeyValue(objType, propertyName, result);

            return result;
        }

        public static Func<object, object> GetUntypedCSPropertyGetter
        (
            object obj,
            string propertyName
        )
        {
            return GetUntypedCSPropertyGetterByObjType(obj?.GetType(), propertyName);
        }

        //typed getters cache
        static DoubleParamMap<Type, string, object> _typedGettersCache =
            new DoubleParamMap<Type, string, object>();

        // returns property getter
        public static Func<TObject, TProperty> GetTypedCSPropertyGetter<TObject, TProperty>(string propertyName)
        {
            Type objType = typeof(TObject);
            object resultObj;

            if (_typedGettersCache.TryGetValue(objType, propertyName, out resultObj))
            {
                return resultObj as Func<TObject, TProperty>;
            }

            ParameterExpression paramExpression = Expression.Parameter(objType, "value");

            Expression propertyGetterExpression = Expression.Property(paramExpression, propertyName);

            Func<TObject, TProperty> result =
                Expression.Lambda<Func<TObject, TProperty>>
                (
                    propertyGetterExpression,
                    paramExpression
                ).Compile();

            _typedGettersCache.AddKeyValue(objType, propertyName, result);

            return result;
        }

        private static
            (
                ParameterExpression objParamExpression,
                ParameterExpression valueParamExpression,
                MemberExpression propertyExpression,
                UnaryExpression valueCastExpression) GetExpressions
            (
                this Type objType, 
                string propertyName, 
                Type propItemType)
        {

            ParameterExpression objParamExpression = Expression.Parameter(typeof(object));

            UnaryExpression objCastExpression = Expression.Convert(objParamExpression, objType);

            ParameterExpression valueParamExpression = Expression.Parameter(typeof(object));

            UnaryExpression valueCastExpression = Expression.Convert(valueParamExpression, propItemType);

            MemberExpression propertyExpression = Expression.Property(objCastExpression, propertyName);

            return (objParamExpression, valueParamExpression, propertyExpression, valueCastExpression);
        }

        static DoubleParamMap<Type, string, Action<object, object>> _untypedSettersCache =
            new DoubleParamMap<Type, string, Action<object, object>>();

        public static Action<object, object> GetUntypedCSPropertySetterByObjType
        (
            this Type objType,
            string propertyName
        )
        {
            Action<object, object> result;

            if (_untypedSettersCache.TryGetValue(objType, propertyName, out result))
            {
                return result;
            }
            Type propertyType = objType.GetPropType(propertyName);

            (ParameterExpression objParamExpression,
                ParameterExpression valueParamExpression,
                MemberExpression propertyExpression,
                UnaryExpression valueCastExpression) = objType.GetExpressions(propertyName, propertyType);

            BinaryExpression assignExpression = Expression.Assign(propertyExpression, valueCastExpression);

            result = Expression.Lambda<Action<object, object>>
            (
                 assignExpression,
                 objParamExpression,
                 valueParamExpression
            ).Compile();

            _untypedSettersCache.AddKeyValue(objType, propertyName, result);

            return result;
        }

        private static Action<object, object> CreateCollectionProcessor
        (
            this Type objType, 
            string collectionPropName,
            Type collectionProcessingClass, 
            string collectionProcessingMethod,
            DoubleParamMap<Type, string, Action<object, object>> cache)
        {
            Action<object, object> result;

            if (cache.TryGetValue(objType, collectionPropName, out result))
            {
                return result;
            }

            Type cellType = objType.GetPropType(collectionPropName).GetCellTypeFromCollectionType();

            (ParameterExpression objParamExpression,
                ParameterExpression valueParamExpression,
                MemberExpression propertyExpression,
                UnaryExpression valueCastExpression) = objType.GetExpressions(collectionPropName, cellType);

            MethodInfo genericMethodInfo = collectionProcessingClass.GetMethod(collectionProcessingMethod);

            MethodInfo methodInfo = genericMethodInfo.MakeGenericMethod(cellType);

            MethodCallExpression callExpr = Expression.Call(methodInfo, propertyExpression, valueCastExpression);

            result = Expression.Lambda<Action<object, object>>(callExpr, objParamExpression, valueParamExpression).Compile();

            cache.AddKeyValue(objType, collectionPropName, result);

            return result;
        }

        static DoubleParamMap<Type, string, Action<object, object>> _untypedCollectionAddersCacher =
                new DoubleParamMap<Type, string, Action<object, object>>();

        public static Action<object, object> GetUntypedCSObjCollectionAdder
        (
            this Type objType,
            string collectionPropName)
        {
            return objType
                    .CreateCollectionProcessor
                    (
                        collectionPropName, 
                        typeof(CollectionUtils), 
                        nameof(CollectionUtils.AddIfNotThereSimple), 
                        _untypedCollectionAddersCacher);
        }

        static DoubleParamMap<Type, string, Action<object, object>> _untypedCollectionRemoversCacher =
            new DoubleParamMap<Type, string, Action<object, object>>();

        public static Action<object, object> GetUntypedCSObjCollectionRemover
        (
            this Type objType,
            string collectionPropName)
        {
            return objType
                    .CreateCollectionProcessor
                    (
                        collectionPropName,
                        typeof(CollectionUtils),
                        nameof(CollectionUtils.RemoveItem),
                        _untypedCollectionRemoversCacher);
        }


        public static Action<object, object> GetUntypedCSPropertySetter
        (
            object obj,
            string propertyName
        )
        {
            if (obj == null)
                return null;

            return GetUntypedCSPropertySetterByObjType(obj.GetType(), propertyName);
        }


        public static Action<object, object> GetUntypedVoidSingleArgMethodByObjType
        (
            this Type objType, 
            string methodName
        )
        {
            Action<object, object> result;

            if (_untypedSettersCache.TryGetValue(objType, methodName, out result))
            {
                return result;
            }

            Type methodArgType = objType.GetMethodArgType(methodName);

            ParameterExpression objParamExpression = Expression.Parameter(typeof(object));

            UnaryExpression objCastExpression = Expression.Convert(objParamExpression, objType);

            ParameterExpression valueParamExpression = Expression.Parameter(typeof(object));
            UnaryExpression valueCastExpression = Expression.Convert(valueParamExpression, methodArgType);

            MethodCallExpression methodExpression = Expression.Call(objCastExpression, methodName, null, valueCastExpression);

            result = Expression.Lambda<Action<object, object>>
            (
                 methodExpression,
                 objParamExpression,
                 valueParamExpression
            ).Compile();

            _untypedSettersCache.AddKeyValue(objType, methodName, result);

            return result;
        }


        static DoubleParamMap<Type, string, Action<object>> _untypedNoArgsCache =
            new DoubleParamMap<Type, string, Action<object>>();

        public static Action<object> GetUntypedVoidNoArgsMethodByObjType
        (
            this Type objType, 
            string methodName
        )
        {
            Action<object> result;
            if (_untypedNoArgsCache.TryGetValue(objType, methodName, out result))
            {
                return result;
            }

            MethodInfo methodInfo = objType.GetMethod(methodName);

            ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "obj");

            Expression convertExpression =
                Expression.Convert(parameterExpression, objType);

            Expression callExpression = Expression.Call(convertExpression, methodInfo);

            result = Expression.Lambda<Action<object>>(callExpression, parameterExpression).Compile();

            _untypedNoArgsCache.AddKeyValue(objType, methodName, result);

            return result;
        }

        static DoubleParamMap<Type, string, object> _typedSettersCache =
            new DoubleParamMap<Type, string, object>();

        // in this function we figure out the property type from the object itself
        public static Action<TObject, object> GetTypedCSPropertySetter<TObject>
        (
            this TObject obj, 
            string propertyName
        )
        {

            Type objType = obj.GetType();

            object resultObj;
            if (_typedSettersCache.TryGetValue(objType, propertyName, out resultObj))
            {
                return resultObj as Action<TObject, object>;
            }

            Type propertyType = objType.GetPropType(propertyName);

            ParameterExpression objParamExpression = 
                Expression.Parameter(objType);

            ParameterExpression propertyParamExpression = 
                Expression.Parameter(typeof(object), propertyName);

            UnaryExpression propertyCastExpression = 
                Expression.Convert(propertyParamExpression, propertyType);

            MemberExpression propertyExpression = 
                Expression.Property(objParamExpression, propertyName);

            BinaryExpression assignExpression = 
                Expression.Assign(propertyExpression, propertyCastExpression);

            Action<TObject, object> result = 
                Expression.Lambda<Action<TObject, object>>
            (
                assignExpression, objParamExpression, propertyParamExpression
            ).Compile();

            _typedSettersCache.AddKeyValue(objType, propertyName, result);

            return result;
        }


        static DoubleParamMap<Type, string, object> _fullyTypedSettersCache =
            new DoubleParamMap<Type, string, object>();

        // returns property setter:
        public static Action<TObject, TProperty> 
            GetFullyTypedCSPropertySetter<TObject, TProperty>(string propertyName)
        {
            Type objType = typeof(TObject);

            object resultObj;
            if(_fullyTypedSettersCache.TryGetValue(objType, propertyName, out resultObj))
            {
                return resultObj as Action<TObject, TProperty>;
            }

            ParameterExpression objParamExpression = Expression.Parameter(objType);

            ParameterExpression propertyParamExpression = Expression.Parameter(typeof(TProperty), propertyName);

            MemberExpression propertyGetterExpression = Expression.Property(objParamExpression, propertyName);

            BinaryExpression assignExpression = Expression.Assign(propertyGetterExpression, propertyParamExpression);

            Action<TObject, TProperty> result = Expression.Lambda<Action<TObject, TProperty>>
            (
                assignExpression, objParamExpression, propertyParamExpression
            ).Compile();

            _fullyTypedSettersCache.AddKeyValue(objType, propertyName, result);

            return result;
        }


        public static (ParameterExpression[] methodObjParams, UnaryExpression[] methodCastParams) GetMethodParams(this MethodInfo methodInfo)
        {
            var paramPairs = methodInfo.GetParameters().Select(param =>
            {
                ParameterExpression objParam = Expression.Parameter(typeof(object), param.Name);
                UnaryExpression castParam = Expression.Convert(objParam, param.ParameterType);

                return new { ObjParam = objParam, CastParam = castParam };
            }).ToList();

            return (paramPairs.Select(pp => pp.ObjParam).ToArray(), paramPairs.Select(pp => pp.CastParam).ToArray());
        }

        public static Expression CreateArrayCellAccessExpression
        (
            this ParameterExpression arrayParameterExpression, // e.g.created by Expression.Parameter(typeof(object[]), "inputParams");
            int cellIdx)
        {
            var cellToSet = Expression.Constant(cellIdx, typeof(int));

            var arrayAccess = Expression.ArrayAccess(arrayParameterExpression, cellToSet);

            return arrayAccess;
        }

        public static Expression CreateAssignArrayCellExpression
        (
            this ParameterExpression arrayParameterExpression, // e.g. created by Expression.Parameter(typeof(object[]), "inputParams"); 
            int cellIdx, 
            Expression exprToAssign)
        {
            var assignArrayCell =
                Expression.Assign
                (
                    arrayParameterExpression.CreateArrayCellAccessExpression(cellIdx),
                    exprToAssign);

            return assignArrayCell;
        }

        public static Expression[] GetMethodParamSettersFromArray(this MethodInfo methodInfo, ParameterExpression arrayParams)
        {
            Expression[] paramSetters =
                methodInfo.GetParameters().Select((param, idx) => Expression.Convert(arrayParams.CreateArrayCellAccessExpression(idx), param.ParameterType)).ToArray();

            return paramSetters;
        }

        // basically for method MyMethod(int i, string str) return a body of the method that would pass
        // the same params as an object array
        // MyMethodCall(object[] inputParams)
        // { 
        //     MyMethod((int)inputParams[0], (string) inputParams[1]);
        // }
        public static MethodCallExpression GetMethodCallFromParamArray
        (
            this MethodInfo methodInfo, 
            ParameterExpression inputParams,
            object obj = null)
        {
            Expression[] paramSetters =
                methodInfo.GetParameters()
                            .Select
                            (
                                (param, idx) => 
                                        Expression.Convert
                                        (
                                            inputParams.CreateArrayCellAccessExpression(idx), 
                                            param.ParameterType)).ToArray();
            Expression instanceExpression = null;

            if (obj != null)
            {
                instanceExpression = Expression.Constant(obj);
            }

            MethodCallExpression methodCallExpr = Expression.Call(instanceExpression, methodInfo, paramSetters);

            return methodCallExpr;
        }

        /// <summary>
        /// returns an Action&lt;object[]&gt; to call a void method specified by 
        /// MethodInfo. If this method is not static, you should also provice 
        /// the object info as obj argument
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Action<object[]> GetParamArrayLambdaForVoidMethod
        (
            this MethodInfo methodInfo,
            object obj = null)
        {
            var array = Expression.Parameter(typeof(object[]), "inputParams");

            MethodCallExpression methodCallExpr = methodInfo.GetMethodCallFromParamArray(array, obj);

            var resultExpr = Expression.Lambda<Action<object[]>>(methodCallExpr, array);

            var result = resultExpr.Compile();

            return result;
        }

        /// <summary>
        /// returns an Func&lt;object[], object&gt; to call a NON-VOID method specified by 
        /// MethodInfo. If this method is not static, you should also provice 
        /// the object info as obj argument
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Func<object[], object> GetParamArrayLambdaForReturningMethod
        (
            this MethodInfo methodInfo,
            object obj = null)
        {
            var array = Expression.Parameter(typeof(object[]), "inputParams");
            
            MethodCallExpression methodCallExpr = methodInfo.GetMethodCallFromParamArray(array, obj);

            var finalExpr = Expression.Convert(methodCallExpr, typeof(object));

            var resultExpr = Expression.Lambda<Func<object[], object>>(finalExpr, array);

            var result = resultExpr.Compile();

            return result;
        }


        /// <summary>
        /// this method is exact opposite of GetMethodCallFromParamArray. It creates a MethodCallExpression
        /// on a method that takes an array of objects and whose parameters are passed as paramInfos 
        /// collection. This method is used in the two methods below to create a void or a non-void Delegate
        /// with the properly typed input arguments, that wrap a call to a void or object returning method
        /// taking an array of objects as input arguments. 
        /// </summary>
        /// <param name="methodInfoForMethodAcceptingObjArray"></param>
        /// <param name="paramInfos"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static (MethodCallExpression methodCallExpr, ParameterExpression[] paramExprs) 
        GetMethodCallForOriginalMethodFromTakingObjArgs
        (            
            this MethodInfo methodInfoForMethodAcceptingObjArray,
            IEnumerable<ParamInfo> paramInfos,
            object obj = null,
            params IParamValGetterExpressionInfo[] paramValGetterExpressionInfos)
        {

            Expression GetParamExpression(string paramName, Type paramType)
            {
                IParamValGetterExpressionInfo paramValGetterExpressionInfo =
                    paramValGetterExpressionInfos
                        .FirstOrDefault
                        (
                            paramValExpressionInfo =>
                                paramValExpressionInfo.ParamName == paramName);

                if (paramValGetterExpressionInfo == null)
                {
                    return Expression.Parameter(paramType, paramName);
                }
                else
                {
                    return paramValGetterExpressionInfo.ValueGetterExpression;
                }
            }
            Expression[] parameterExpressions =
                paramInfos
                    ?.Select(paramInfo => GetParamExpression(paramInfo.ParamName, paramInfo.ParamType))
                    ?.ToArray();

            Expression[] objExpressions =
                parameterExpressions
                    ?.Select(paramExpr => Expression.Convert(paramExpr, typeof(object)))
                    ?.ToArray();

            MethodCallExpression methodCallExpression = null;

            if (obj == null)
            {

                methodCallExpression = objExpressions != null ?     
                    Expression.Call
                    (
                        methodInfoForMethodAcceptingObjArray,
                        Expression.NewArrayInit(typeof(object), objExpressions) ) 
                    : 
                    Expression.Call(methodInfoForMethodAcceptingObjArray);
            }
            else
            {
                methodCallExpression = objExpressions != null ? 
                    Expression.Call
                    (
                        Expression.Constant(obj),
                        methodInfoForMethodAcceptingObjArray,
                        Expression.NewArrayInit(typeof(object), objExpressions))
                    :
                    Expression.Call
                    (
                        Expression.Constant(obj),
                        methodInfoForMethodAcceptingObjArray);
            }

            return (methodCallExpression, parameterExpressions?.OfType<ParameterExpression>()?.ToArray());
        }

        /// <summary>
        /// this method wraps a void method that takes an array of objects into 
        /// a void method with properly typed arguments (types are specified by 
        /// paramInfos array)
        /// </summary>
        /// <param name="methodInfoForMethodAcceptingObjArray"></param>
        /// <param name="paramInfos"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Delegate GetLambdaForVoidMethodAcceptingObjArray
        (
            this MethodInfo methodInfoForMethodAcceptingObjArray, 
            IEnumerable<ParamInfo> paramInfos,
            object obj = null,
            params IParamValGetterExpressionInfo[] paramValGetterExpressionInfos)
        {

            (MethodCallExpression methodCallerExpr, ParameterExpression[] paramExprs)  =
                methodInfoForMethodAcceptingObjArray
                    .GetMethodCallForOriginalMethodFromTakingObjArgs(paramInfos, obj, paramValGetterExpressionInfos);

            return methodCallerExpr.GetDelegateFromExpr(paramExprs);
        }

        /// <summary>
        /// this method wraps a method that takes an array of objects and returns an object, into 
        /// a method with properly typed arguments (types are specified by 
        /// paramInfos array) that returns a value of types specified by returingType input arg. 
        /// If returning type is null, it will return a value of type 'object'. 
        /// </summary>
        /// <param name="methodInfoForMethodAcceptingObjArray"></param>
        /// <param name="paramInfos"></param>
        /// <param name="returningType"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Delegate GetLambdaForObjReturningMethodAcceptingObjArray(
            this MethodInfo methodInfoForMethodAcceptingObjArray,
            IEnumerable<ParamInfo> paramInfos,
            Type returningType = null,
            object obj = null,
            params IParamValGetterExpressionInfo[] paramValGetterExpressionInfos)
        {
            (MethodCallExpression methodCallerExpr, ParameterExpression[] paramExprs) =
                methodInfoForMethodAcceptingObjArray
                    .GetMethodCallForOriginalMethodFromTakingObjArgs(paramInfos, obj, paramValGetterExpressionInfos);

            if (returningType == null)
            {
                returningType = typeof(object);
            }

            Expression methodCallerConversionExpr = 
                Expression.Convert(methodCallerExpr, returningType);

            return methodCallerConversionExpr.GetDelegateFromExpr(paramExprs);
        }

        public static Delegate GetDelegateFromExpr
        (
            this Expression expr, 
            IEnumerable<ParameterExpression> exprParams)
        {
            var resultExpr =
                exprParams != null ? 
                    Expression.Lambda(expr, exprParams) : Expression.Lambda(expr);

            var compiledResult = resultExpr.Compile();

            return compiledResult;
        }



        /// <summary>
        /// returns the method call expression to call the method and the ParameterExpressions 
        /// for calling the wrapped method. Parameters corresponding to the passed 
        /// paramValGetterExpressionInfos argument, are not returned, since they are obtained
        /// via IParamValGetterExpressionInfo.ValueGetterExpression and not via the arguments
        /// of the wrapper method
        /// </summary>
        public static (MethodCallExpression methodCallExpression, ParameterExpression[] paramExpressions)
            GetMethodCallExpression
        (
            this MethodInfo methodInfo,
            object obj = null,
            params IParamValGetterExpressionInfo[] paramValGetterExpressionInfos
        )
        {
            if (methodInfo == null)
                return (null, null);

            Expression GetParamExpression(string paramName, Type paramType)
            {
                IParamValGetterExpressionInfo paramValGetterExpressionInfo =
                    paramValGetterExpressionInfos
                        .FirstOrDefault
                        (
                            paramValExpressionInfo => 
                                paramValExpressionInfo.ParamName == paramName);

                if (paramValGetterExpressionInfo == null)
                {
                    return Expression.Parameter(paramType, paramName);
                }
                else
                {
                    Expression resultExpression = 
                        Expression.Convert(paramValGetterExpressionInfo.ValueGetterExpression, paramType);

                    return resultExpression;
                }
            }

            // obtains the expressions for the MethodInfo parameters including the expressions
            // to be passed from the Wrapper Method - they are of type ParameterExpression and 
            // the expressions to obtain the value via IParamValGetterExpressionInfo.ValueGetterExpression
            // and conversion (of type UnaryExpression)
            Expression[] paramExpressions =
                methodInfo.GetParameters()
                          .Select(paramInfo => GetParamExpression(paramInfo.Name, paramInfo.ParameterType))
                          .ToArray();

            Expression instanceExpr =
                obj != null ? Expression.Constant(obj) : null;

            MethodCallExpression methodCallExpression =
                Expression.Call(instanceExpr, methodInfo, paramExpressions);

            return (methodCallExpression, paramExpressions.OfType<ParameterExpression>().ToArray());
        }

        public static Delegate GetCompiledMethodCallLambda
        (
            this MethodInfo methodInfo,
            object obj = null,

            // these are parameters to the original method whose values are not received
            // from the parameters of the generated method, but obtained via 
            // IParamValGetterExpressionInfo.ValueGetterExpression
            params IParamValGetterExpressionInfo[] paramValGetterExpressionInfos 
        )
        {
            (MethodCallExpression methodCallExpression, ParameterExpression[] paramExpressions) =
                methodInfo.GetMethodCallExpression(obj, paramValGetterExpressionInfos);

            Delegate lambda = 
                Expression.Lambda
                (
                    methodCallExpression, 
                    paramExpressions)
                .Compile();

            return lambda;
        }


        public static Delegate GetCompiledMethodCallLambdaWithMethodToExecuteBeforeMain
        (
            this MethodInfo methodInfo,
            object obj,
            MethodCallExpression voidNoArgMethodCallToBeExecutedBeforeMainMethod,
            params IParamValGetterExpressionInfo[] paramValGetterExpressionInfos
        )
        {
            (MethodCallExpression methodCallExpression, ParameterExpression[] paramExpressions) =
                methodInfo.GetMethodCallExpression(obj, paramValGetterExpressionInfos);

            Expression combinedExpression =
                Expression.Block
                (
                    voidNoArgMethodCallToBeExecutedBeforeMainMethod, 
                    methodCallExpression);

            Delegate lambda =
                Expression.Lambda
                (
                    combinedExpression,
                    paramExpressions)
                .Compile();

            return lambda;

        }
    }
}
