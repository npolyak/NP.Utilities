using System.Linq.Expressions;
using System.Reflection;

namespace NP.Utilities.Expressions
{
    public class ParamValue : IParamValGetter, IParamValSetter
    {
        ParameterExpression _inputArrayParam;

        public ParameterInfo Param { get; }

        public Expression? InputArrayAccessConvertExpr { get; private set; }

        public ParameterExpression? Expr { get; }

        public Expression? AssignInputValueExpression { get; private set; }

        public Expression? AssignOutputValueExpression { get; private set; }

        public ParameterExpression _outputArrayParams;

        public Expression? InputParamExpression => 
                (InputArrayAccessConvertExpr ?? Expr);


        public object? Value { get; set; }

        public object? GetValue() => Value;

        public void SetValue(object? value) => Value = value;

        public string? Name => Param.Name;

        // e.g, Int32& (reference type)
        public Type TheType { get; }

        // for non-reference type it will be the same as TheType
        // for reference type it will be corresponding non-reference type
        // e.g. for TheType=Int32& it will be Int32 (without ampersand at the end)
        public Type RealType { get; }
        
        public Expression? ReturnArrayAccessExpr { get; private set; }

        public ParamValue
        (
            ParameterInfo param, 
            ParameterExpression inputArrayParam, 
            ParameterExpression outputArrayParams,
            ParameterExpression? returnArrayParams = null)
        {
            Param = param;

            _inputArrayParam = inputArrayParam;
            _outputArrayParams = outputArrayParams;

            TheType = param.ParameterType!;

            if (TheType.IsByRef)
            {
                RealType = TheType.GetElementType()!;
            }
            else
            {
                RealType = TheType;
            }

            if (IsReturn)
            {
                ReturnArrayAccessExpr = returnArrayParams!.CreateArrayCellAccessExpression(0);
            }
            else
            {
                if (IsOut)
                {
                    Expr = Expression.Variable(RealType, Name);
                }
                else // In only
                {
                    Expr = Expression.Parameter(RealType, Name);
                }
            }
        }

        public bool IsReturn => Param.Position == -1;

        public bool IsIn => IsReturn ? false: Param.IsIn();

        public bool IsOut => IsReturn ? false : Param.IsOut();

        private int _inputIdx;
        public int InputIdx
        { 
            get => _inputIdx;
            set
            {
                _inputIdx = value;


                var inputArrayAccessConvExpr =
                    Expression.Convert
                    (
                        _inputArrayParam.CreateArrayCellAccessExpression(InputIdx),
                        this.RealType);

                if (IsOut)
                {
                    /// e.g. $referenceInt = (System.Int32)$__Input__[0]
                    AssignInputValueExpression =
                        Expression.Assign(Expr, inputArrayAccessConvExpr);
                }
                else
                {
                    /// e.g (System.Int32)$__Input__[1]
                    InputArrayAccessConvertExpr = inputArrayAccessConvExpr;
                }
            }
        }

        private int _outputIdx;
        public int OutputIdx 
        {
            get => _outputIdx;
            set
            {
                _outputIdx = value;

                var outputArrayAccessExpr = 
                    _outputArrayParams.CreateArrayCellAccessExpression(_outputIdx);

                ///e.g. $__Output__[0] = (System.Object)$referenceInt
                AssignOutputValueExpression = 
                    Expression.Assign
                    (
                        outputArrayAccessExpr, 
                        Expression.Convert(Expr, typeof(object)));
            }
        }

    }
}
