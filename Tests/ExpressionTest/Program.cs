using ExpressionTest;
using System.Reflection;
using NP.Utilities.Expressions;
using System.Linq.Expressions;
using NP.Utilities;

public class Program
{
    public static int Plus(int number1, int number2)
    {
        return number1 + number2;
    }

    public static void PlusRef(ref int referenceInt, int number2)
    {
        referenceInt += number2;
    }

    public static void PlusOut(int i1, int i2, out int result)
    {
        result = i1 + i2;
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

        ///.Block(System.Int32 $i) {
        ///     $i = 10
        /// }

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
        ///$i1
       
        ParameterExpression variableExpr = Expression.Variable(typeof(int), "referencedInt");
        ///$referencedInt

        Expression assignExpr = Expression.Assign(variableExpr, inputParamExpression1);
        ///$referencedInt = $i1

        ParameterExpression inputParamExpression2 = Expression.Parameter(typeof(int), "i2");
        ///$i2

        var callPlusRefExpr = Expression.Call(plusRefMethodInfo, variableExpr, inputParamExpression2);
        ///.Call Program.PlusRef(
        ///     $referencedInt,
        ///     $i2)

        Expression body =
            Expression.Block
            (
                typeof(int),
                new ParameterExpression[] { variableExpr },
                new Expression[] { assignExpr, callPlusRefExpr, variableExpr });
        ///.Block(System.Int32 $referencedInt) { // this is where the block's variables are defined (ref or not does not matter)
        ///    $referencedInt = $i1; // i1 is the variable coming from the input parameters
        ///    .Call Program.PlusRef(
        ///        $referencedInt,
        ///        $i2);             // i2 is the variagble coming from input parameters
        ///    $referencedInt   // last line specifies what the block returns.
        ///}       

        var resultExpression = Expression.Lambda<Func<int, int, int>>
            (
                body,
                new ParameterExpression[]
                {
                    inputParamExpression1,
                    inputParamExpression2 });
        ///.Lambda #Lambda1<System.Func`3[System.Int32,System.Int32,System.Int32]>(
        ///    System.Int32 $i1,
        ///    System.Int32 $i2) {
        ///    .Block(System.Int32 $referencedInt) {
        ///        $referencedInt = $i1;
        ///        .Call Program.PlusRef(
        ///            $referencedInt,
        ///            $i2);
        ///        $referencedInt
        ///    }
        ///}

        Func<int, int, int> f = resultExpression.Compile();

        int i1 = 3, i2 = 4;
        int i = f(i1, i2); // i = 7

        Console.WriteLine(i);
    }

    public static void OutSampleWithInputArgs()
    {

        Type type = typeof(Program);

        MethodInfo plusOutMethodInfo = type.GetMethod(nameof(Program.PlusOut))!;

        ParameterExpression inputParamExpression1 = Expression.Parameter(typeof(int), "i1");
        ParameterExpression inputParamExpression2 = Expression.Parameter(typeof(int), "i2");
        ParameterExpression resultExpr = Expression.Variable(typeof(int), "result");

        var callPlusOutExpr =
            Expression.Call
            (
                plusOutMethodInfo,
                inputParamExpression1,
                inputParamExpression2,
                resultExpr);
        ///.Call Program.PlusOut
        /// (
        ///      $i1,
        ///      $i2,
        ///      $result)


        Expression body =
            Expression.Block
            (
                typeof(int),
                new ParameterExpression[] { resultExpr },
                new Expression[] { callPlusOutExpr, resultExpr });
        ///.Block(System.Int32 $result) {
        ///    .Call Program.PlusOut
        ///     (
        ///         $i1,
        ///         $i2,
        ///         $result); 
        ///    $result
        ///}


        Expression<Func<int, int, int>> lambdaExpression =
            Expression.Lambda<Func<int, int, int>>
            (
                body,
                new ParameterExpression[] { inputParamExpression1, inputParamExpression2 });
        ///.Lambda #Lambda1<System.Func`3[System.Int32,System.Int32,System.Int32]>(
        ///    System.Int32 $i1,
        ///    System.Int32 $i2) {
        ///    .Block(System.Int32 $result) {
        ///        .Call Program.PlusOut(
        ///            $i1,
        ///            $i2,
        ///            $result);
        ///        $result
        ///    }
        ///}

        Func<int, int, int> f = lambdaExpression.Compile();

        int i1 = 3, i2 = 4;
        int result = f(i1, i2); // i = 7

        Console.WriteLine(result);
    }

    public static void RefSampleWithInputArgsConversion()
    {

        Type type = typeof(Program);

        MethodInfo plusRefMethodInfo = type.GetMethod(nameof(Program.PlusRef))!;



        ParameterExpression inputParamExpression1 = Expression.Parameter(typeof(int), "i1");
        ParameterExpression variableExpr = Expression.Variable(typeof(int), "referencedInt");
        Expression assignExpr = Expression.Assign(variableExpr, inputParamExpression1);

        ParameterExpression inputParamExpression2 = Expression.Parameter(typeof(int), "i2");

        var callPlusRefExpr = Expression.Call(plusRefMethodInfo, variableExpr, inputParamExpression2);
        ///.Call Program.PlusRef(
        ///    $referencedInt,
        ///    $i2)

        Expression body =
            Expression.Block
            (
                typeof(int),
                new ParameterExpression[] { variableExpr },
                new Expression[] { assignExpr, callPlusRefExpr, variableExpr });
        ///.Block(System.Int32 $referencedInt) { // this is where the block's variables are defined (ref or not does not matter)
        ///        $referencedInt = $i1; // i1 is the variable coming from the input parameters
        ///        .Call Program.PlusRef(
        ///            $referencedInt,
        ///            $i2);             // i2 is the variagble coming from input parameters
        ///        $referencedInt   // last line specifies what the block returns.
        /// }       
        /// 

        Expression<Func<object[], object[]>> lambdaExpression =
            Expression.Lambda<Func<object[], object[]>>(body, new ParameterExpression[] { inputParamExpression1, inputParamExpression2 }); 

        Func<object[], object[]> f = lambdaExpression.Compile();

        int i1 = 3, i2 = 4;
        object[] result = f(new object[] { 3, 4}); // i = 7

        Console.WriteLine(result[0]);
    }

    public static void Main(string[] args)
    {
        // VoidMethodTest();

        //MultiArgMethodWithReturnTest();

        //TestBlockExprWithReturn();

        //RefSampleFromStackOverflow();

        //RefSampleWithInputArgs();

        //OutSampleWithInputArgs();

        //RefSampleWithInputArgsConversion();


        MethodInfo plusRefMethodInfo = typeof(Program).GetMethod(nameof(PlusOut))!;

        var parameters = plusRefMethodInfo.GetParameters();

        var param1 = parameters[0];
        Console.WriteLine(param1.GetParamInfoStr());

        var param2 = parameters[1];
        Console.WriteLine(param2.GetParamInfoStr());


        var param3 = parameters[2];
        Console.WriteLine(param3.GetParamInfoStr());

        //object[] Plus(object[] args)
        //{
        //    int refInt = (int) args[0];

        //    PlusRef(ref refInt, (int)args[1]);

        //    return new object [] { refInt };
        //}
    }
}

