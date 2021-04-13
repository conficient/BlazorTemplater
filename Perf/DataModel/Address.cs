using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }

        public static Address GetTestAddress()
        {
            return new Address()
            {
                Line1 = "123 The High Street",
                Line2 = "Someplace",
                Town = "Test Town",
                Postcode = "TE5 1NG"
            };
        }
    }
}
