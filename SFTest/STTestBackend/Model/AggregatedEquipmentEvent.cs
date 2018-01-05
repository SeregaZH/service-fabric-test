using System;

namespace STTestBackend.Model
{
    public class AggregatedEquipmentEvent
    {
        public RangeQuantity<int> Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
