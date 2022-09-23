using ExpressionTest;
using System.Reflection;
using NP.Utilities.Expressions;
using System.Linq.Expressions;

public class Program
{
    public static int Plus(int number1, int number2)
    {
        return number1 + number2;
    }

    public static void PlusRef(ref int number1, int number2)
    {
        number1 += number2;
    }

    public static void PlusOut(int n1, int n2, out int result)
    {
        result = n1 + n2;
    }

    internal static int Square(int i)
    {
        return i * i;
    }

    private static void VoidMethodTest()
    {
        Counter counter = new Counter();

        MethodInfo methodInfo =
            typeof(Counter).GetMethod(nameof(Counter.UpCount));

        Action upCountAction = (Action)methodInfo.GetCompiledMethodCallLambda(counter);


        upCountAction();
        upCountAction();
        upCountAction();

        Console.WriteLine(counter.Count); //3
    }

    public static void MultiArgMethodWithReturnTest()
    {
        MethodInfo plusMethodInfo = typeof(Program).GetMethod(nameof(Program.Plus))!;

        Func<object[], object> func = plusMethodInfo.GetParamArrayLambdaForReturningMethod();

        int result = (int)func.Invoke(new object[] { 10, 2 });

        Console.WriteLine(result);
    }

    public static void TestBlockExprWithReturn()
    {
        ParameterExpression variableExpr = Expression.Variable(typeof(int), "i");
        Expression assignExpr = Expression.Assign(variableExpr, Expression.Constant(10));
        Expression blockExpr = Expression.Block(
            typeof(int), // return type
            new ParameterExpression[] { variableExpr },
            assignExpr); // last expression is returned (apparently if there is a return type)

        Func<int> f = Expression.Lambda<Func<int>>(blockExpr).Compile();

        int result = f.Invoke();

        Console.WriteLine(result);
    }

    public static void TestCallReturningMethodExpr()
    {
        Type type = typeof(Program);

        MethodInfo methodInfo = type.GetMethod(nameof(Program.Square))!;

        ParameterExpression inputParamExpression = Expression.Parameter(typeof(int), "i");

        var callSquareExpr = Expression.Call(methodInfo, new[] { inputParamExpression });

        Func<int, int> squared = Expression.Lambda<Func<int, int>>(callSquareExpr, inputParamExpression).Compile();

        Console.Write(squared(10)); // prints 100 (10 * 10)
    }

    public static void TwiceTheInputByRef(ref int x)
    {
        x = x * 2;
    }

    public static void RefSampleFromStackOverflow()
    {
        Type type = typeof(Program);

        MethodInfo methodInfo =
            type.GetMethod("TwiceTheInputByRef")!;

        var inputVar = Expression.Variable(typeof(int), "input");
        var blockExp =
            Expression.Block(
                    new[] { inputVar }
                    , Expression.Assign(inputVar, Expression.Constant(10))
                    , Expression.Call(methodInfo, inputVar)
                    , inputVar
                    );

        // returns 20
        var result = Expression.Lambda<Func<int>>(blockExp).Compile()();
    }

    public static void RefSampleWithInputArgs()
    {

        Type type = typeof(Program);

        MethodInfo plusRefMethodInfo = type.GetMethod(nameof(Program.PlusRef))!;

        ParameterExpression inputParamExpression1 = Expression.Parameter(typeof(int), "i1");
        ParameterExpression variableExpr = Expression.Variable(typeof(int), "referencedInt");
        Expression assignExpr = Expression.Assign(variableExpr, inputParamExpression1);

        ParameterExpression inputParamExpression2 = Expression.Parameter(typeof(int), "i2");

        var callPlusRefExpr = Expression.Call(plusRefMethodInfo, variableExpr, inputParamExpression2);

        ///.Block(System.Int32 $referencedInt) { // this is where the block's variables are defined (ref or not does not matter)
        ///        $referencedInt = $i1; // i1 is the variable coming from the input parameters
        ///        .Call Program.PlusRef(
        ///            $referencedInt,
        ///            $i2);             // i2 is the variagble coming from input parameters
        ///        $referencedInt   // last line specifies what the block returns.
        /// }       
        /// 

        Expression body =
            Expression.Block
            (
                typeof(int),
                new ParameterExpression[] { variableExpr },
                new Expression[] { assignExpr, callPlusRefExpr, variableExpr });

        Func<int, int, int> f =
            Expression.Lambda<Func<int, int, int>>
            (
                body, 
                new ParameterExpression[]
                {
                    inputParamExpression1, 
                    inputParamExpression2 })
            .Compile();

        int i1 = 3, i2 = 4;
        int i = f(i1, i2); // i = 7

        Console.WriteLine(i);
    }

    public static void RefSampleWithInputArgsConvertion()
    {

        Type type = typeof(Program);

        MethodInfo plusRefMethodInfo = type.GetMethod(nameof(Program.PlusRef))!;

        ParameterExpression inputParamExpression1 = Expression.Parameter(typeof(int), "i1");
        ParameterExpression variableExpr = Expression.Variable(typeof(int), "referencedInt");
        Expression assignExpr = Expression.Assign(variableExpr, inputParamExpression1);

        ParameterExpression inputParamExpression2 = Expression.Parameter(typeof(int), "i2");

        var callPlusRefExpr = Expression.Call(plusRefMethodInfo, variableExpr, inputParamExpression2);

        ///.Block(System.Int32 $referencedInt) { // this is where the block's variables are defined (ref or not does not matter)
        ///        $referencedInt = $i1; // i1 is the variable coming from the input parameters
        ///        .Call Program.PlusRef(
        ///            $referencedInt,
        ///            $i2);             // i2 is the variagble coming from input parameters
        ///        $referencedInt   // last line specifies what the block returns.
        /// }       
        /// 

        Expression body =
            Expression.Block
            (
                typeof(int),
                new ParameterExpression[] { variableExpr },
                new Expression[] { assignExpr, callPlusRefExpr, variableExpr });

        Func<object[], object[]> f =
            Expression.Lambda<Func<object[], object[]>>(body, new ParameterExpression[] { inputParamExpression1, inputParamExpression2 }).Compile();

        int i1 = 3, i2 = 4;
        object[] result = f(new object[] { 3, 4}); // i = 7

        Console.WriteLine(result[0]);
    }

    public static void OutSampleWithInputArgs()
    {

        Type type = typeof(Program);

        MethodInfo plusOutMethodInfo = type.GetMethod(nameof(Program.PlusOut))!;

        ParameterExpression inputParamExpression1 = Expression.Parameter(typeof(int), "i1");
        ParameterExpression variableExpr = Expression.Variable(typeof(int), "outInt");

        ParameterExpression inputParamExpression2 = Expression.Parameter(typeof(int), "i2");

        var callPlusOutExpr = 
            Expression.Call
            (
                plusOutMethodInfo, 
                inputParamExpression1, 
                inputParamExpression2,
                variableExpr);

        //.Block(System.Int32 $outInt) { // defined outInt
        //    .Call Program.PlusOut(
        //        $i1, // from method params
        //        $i2, // from method params
        //        $outInt); // passing outInt
        //    $outInt // returning outInt
        //}

        Expression body =
            Expression.Block
            (
                typeof(int),
                new ParameterExpression[] { variableExpr },
                new Expression[] { callPlusOutExpr, variableExpr });

        Func<int, int, int> f =
            Expression.Lambda<Func<int, int, int>>
            (
                body, 
                new ParameterExpression[] { inputParamExpression1, inputParamExpression2 }).Compile();

        int i1 = 3, i2 = 4;
        int i = f(i1, i2); // i = 7

        Console.WriteLine(i);
    }

    public static void Main(string[] args)
    {
        // VoidMethodTest();

        // MultiArgMethodWithReturnTest();

        // TestBlockExprWithReturn();

        // RefSampleFromStackOverflow();

        // RefSampleWithInputArgs();

        // OutSampleWithInputArgs();

        RefSampleWithInputArgsConvertion();

        object[] Plus(object[] args)
        {
            int refInt = (int) args[0];

            PlusRef(ref refInt, (int)args[1]);

            return new object [] { refInt };
        }


    }
}

