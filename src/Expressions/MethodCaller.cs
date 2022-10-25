using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;

namespace NP.Utilities.Expressions
{
    public class MethodCaller
    {
        MethodInfo _methodInfo;

        public const string INPUT_PARAM_NAME = "__Input__";
        public const string OUTPUT_PARAM_NAME = "__Output__";
        public const string RETURN_PARAM_NAME = "__RETURN__";

        public Action<object?[], object?[], object?[]> Method { get; private set; }

        public Type DeclaringType => _methodInfo.DeclaringType!;

        public ParamValue[] ParamValues { get; }

        public ParamValue[] InputParams { get; }

        public ParamValue[] OutputParams { get; }

        public ParamValue[] ReturnParams { get; }

        public int NumberInputs => InputParams.Count();

        public int NumberOutputs => OutputParams.Count();

        public int NumberReturns => ReturnParams.Count();

        public bool HasReturn => !_methodInfo.ReturnType.IsVoid();

        public bool IsVoid => !HasReturn;

        public ParamValue ReturnParamValue { get; }

        public object? Instance { get; }

        public Expression InstanceExpression { get; }

        public ParameterExpression InputArrayParamsExpression { get; }

        public ParameterExpression OutputArrayParamsExpression { get; }

        public ParameterExpression ReturnArrayParamsExpression { get; }

        public MethodCaller(MethodInfo methodInfo, object? instance = null)
        {
            _methodInfo = methodInfo;

            InputArrayParamsExpression = Expression.Parameter(typeof(object[]), INPUT_PARAM_NAME);
            OutputArrayParamsExpression = Expression.Parameter(typeof(object[]), OUTPUT_PARAM_NAME);

            Instance = instance;

            if (Instance != null)
            {
                InstanceExpression = Expression.Constant(Instance, DeclaringType);
            }

            List<ParamValue> paramValues = new List<ParamValue>();

            foreach(var parameter in methodInfo.GetParameters())
            {
                ParamValue paramValue = new ParamValue(parameter, InputArrayParamsExpression, OutputArrayParamsExpression);

                paramValues.Add(paramValue);    
            }

            ParamValues = paramValues.ToArray();

            InputParams = ParamValues.Where(v => v.Param.IsIn()).ToArray();

            OutputParams = ParamValues.Where(v => v.Param.IsOut()).ToArray();

            int inputIdx = 0;
            int outputIdx = 0;

            foreach (var paramValue in ParamValues)
            {
                if (paramValue.IsIn)
                {
                    paramValue.InputIdx = inputIdx;
                    inputIdx++;
                }

                if (paramValue.IsOut)
                {
                    paramValue.OutputIdx = outputIdx;
                    outputIdx++;
                }
            }

            ReturnArrayParamsExpression = Expression.Parameter(typeof(object[]), RETURN_PARAM_NAME);

            if (HasReturn)
            {

                ReturnParamValue = new ParamValue(_methodInfo.ReturnParameter, InputArrayParamsExpression, OutputArrayParamsExpression, ReturnArrayParamsExpression);

                ReturnParams = new ParamValue[] { ReturnParamValue };
            }
            else
            {
                ReturnParams = new ParamValue[] { };
            }


            CreateExpressionAndMethod();
        }

        private void CreateExpressionAndMethod()
        {
            IEnumerable<ParamValue> outputParams = ParamValues.Where(p => p.IsOut);

            Expression callExpression = 
                Expression.Call
                (
                    _methodInfo, 
                    ParamValues
                        .Where(p => p.InputParamExpression != null)!
                        .Select(p => p.InputParamExpression)!);

            if (HasReturn)
            {
                callExpression =
                    Expression.Assign
                    (
                        ReturnParamValue.ReturnArrayAccessExpr!,
                        Expression.Convert(callExpression, typeof(object))
                    );
            }

            IEnumerable<Expression> assignInputExpressions =
                ParamValues
                    .Where(p => p.AssignInputValueExpression != null)
                    .Select(p => p.AssignInputValueExpression)!;

            IEnumerable<Expression> assignOutputExpressions =
                outputParams
                    .Select(p => p.AssignOutputValueExpression)!;


            var blockVars = outputParams.Select(p => p.Expr).ToArray(); // variables

            List<Expression> blockExpressions = assignInputExpressions.ToList();

            IEnumerable<Expression> returnExpressions =
                ReturnParams.Select(p => p.ReturnArrayAccessExpr)!;

            blockExpressions.Add(callExpression);
            blockExpressions.AddRange(assignOutputExpressions);
            blockExpressions.AddRange(returnExpressions);

            Expression body;

            if (HasReturn)
            {
                body = Expression.Block(blockVars!, blockExpressions);
            }
            else
            {
                body = Expression.Block(typeof(object), blockVars!, blockExpressions);
            }

            Expression<Action<object[], object?[], object?[]>> lambdaExpression =
                Expression.Lambda<Action<object[], object?[], object?[]>>
                (
                    body,
                    new ParameterExpression[] { InputArrayParamsExpression, OutputArrayParamsExpression, ReturnArrayParamsExpression }
                );
            Method = lambdaExpression.Compile()!;
        }

        public string MethodIdentifierStr
        {
            get
            {
                return $"{DeclaringType.Name}.{_methodInfo.Name}(...)";
            }
        }

        private ParamValue? FindParamVal(string paramName)
        {
            return ParamValues.FirstOrDefault(paramVal => paramVal.Param.Name == paramName);
        }


        public void SetInputValue(string paramName, object inputValue)
        {
            ParamValue paramValue = FindParamVal(paramName)!;

            paramValue.Value = inputValue;
        }

        public object? GetOutputValue(string paramName)
        {
            ParamValue paramValue = FindParamVal(paramName)!;

            return paramValue.Value;
        }

        public object? GetReturnValue()
        {
            ParamValue? paramValue = ReturnParamValue;

            paramValue?.ThrowIfNull($"Method {MethodIdentifierStr} does not have return value.");

            return paramValue!.Value;
        }


        public void Call()
        {
            object[] outputParams = new object[NumberOutputs];

            object[] returnParams = new object[NumberReturns];

            Method(InputParams.Select(p => p.GetValue()).ToArray(), outputParams, returnParams);

            for (int i = 0; i < NumberOutputs; i++)
            {
                OutputParams[i].SetValue(outputParams[i]);
            }

            for(int i = 0; i < NumberReturns; i++)
            {
                ReturnParams[i].SetValue(returnParams[i]);
            }
        }
    }
}
