using System.Collections.Generic;
using SFTestBackend.Models;

namespace STTestBackend.Model
{
    public class RangeQuantity<TValue>
    {
        public RangeQuantity(List<TValue> value, Unit unit)
        {
            Value = value;
            Unit = unit;
        }

        public List<TValue> Value { get; }
        public Unit Unit { get; }
    }
}
