namespace SFTestStateless.Intergation.Tests
{
    public class Quantity<TValue>
    {
        public Quantity(TValue value, Unit unit)
        {
            Value = value;
            Unit = unit;
        }

        public TValue Value { get; }
        public Unit Unit { get; }
    }
}
