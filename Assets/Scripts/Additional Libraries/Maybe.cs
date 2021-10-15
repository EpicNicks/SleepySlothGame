namespace Utils
{
    public abstract class Maybe<T>
    {
        private Maybe() { }

        public sealed class Some : Maybe<T>
        {
            public T Value { get; }
            public Some(T value)
            {
                Value = value;
            }
        }

        public sealed class None : Maybe<T> { }
    }
}
