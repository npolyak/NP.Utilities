namespace NP.Utilities
{
    public class ValueContainer<T>
    {
        public T Value { get; }

        public ValueContainer(T value)
        {
            Value = value;
        }
    }
}
