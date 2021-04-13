using System;

namespace DataModel
{
    public class Row
    {
        public string Item { get; set; }

        public string Code { get; set; }

        /// <summary>
        /// Can be free
        /// </summary>
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal Total => UnitPrice * Quantity;

        internal static Row CreateRandom(Random r, int x)
        {
            return new Row()
            {
                Item = $"Item {x:0000}",
                Code = $"SKU{r.Next(0, 1000000):00000000}",
                Quantity = r.Next(0, 50),
                UnitPrice = Convert.ToDecimal(r.NextDouble() * 100 + 1)
            };
        }
    }
}