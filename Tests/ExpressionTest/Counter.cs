namespace ExpressionTest
{
    public class Counter
    {
        public int Count { get; private set; } = 0;

        public void UpCount() => Count++;
    }
}
