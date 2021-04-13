using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    /// <summary>
    /// This is the common model used for all tests
    /// </summary>
    public class OrderModel
    {
        public string Name { get; set; }

        public Address DeliveryAddress { get; set; }

        public List<Row> Rows { get; set; }

        public decimal SubTotal => Rows.Sum(r => r.Total);

        /// <summary>
        /// Tax rate as %, e.g. 10% => 0.1
        /// </summary>
        public decimal? TaxRate { get; set; }

        public decimal Total => SubTotal * (1 + TaxRate.GetValueOrDefault(0));

        public static OrderModel CreateModel(string name, Random r, int rows)
        {
            const decimal tax = 0.2m;
            return new OrderModel()
            {
                Name = name,
                TaxRate = tax,
                DeliveryAddress = Address.GetTestAddress(),
                Rows = CreateRows(rows, r)
            };
        }

        private static List<Row> CreateRows(int rows, Random r)
        {
            return Enumerable.Range(1, rows).Select(x => Row.CreateRandom(r, x)).ToList();
        }
    }
}
