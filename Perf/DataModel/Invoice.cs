using System;
using System.Collections.Generic;
using System.Linq;

namespace DataModel
{
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string CompanyLogoUrl { get; set; }
        public Address CompanyAddress { get; set; }
        public Address BillingAddress { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<LineItem> LineItems { get; set; }
        public decimal TotalPrice => LineItems.Sum(x => x.TotalPrice);

        public static Invoice Create()
        {
            return new Invoice()
            {
                InvoiceNumber = "3232",
                CreatedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                CompanyAddress = new Address()
                {
                    Name = "XY Technologies",
                    AddressLine1 = "XY Street, Park Road",
                    City = "Chennai",
                    Country = "India",
                    Email = "xy-email@gmail.com",
                    PinCode = "600001"
                },
                BillingAddress = new Address()
                {
                    Name = "XY Customer",
                    AddressLine1 = "ZY Street, Loyal Road",
                    City = "Bangalore",
                    Country = "India",
                    Email = "xy-customer@gmail.com",
                    PinCode = "343099"
                },
                PaymentMethod = new PaymentMethod()
                {
                    Name = "Cheque",
                    ReferenceNumber = "94759849374"
                },
                LineItems = new List<LineItem>
        {
            new LineItem
            {
            Id = 1,
            ItemName = "USB Type-C Cable",
            Quantity = 3,
            PricePerItem = 10.33M
            },
               new LineItem
            {
            Id = 1,
            ItemName = "SSD-512G",
            Quantity = 10,
            PricePerItem = 90.54M
            }
        },
                CompanyLogoUrl = "https://raw.githubusercontent.com/soundaranbu/RazorTemplating/master/src/Razor.Templating.Core/assets/icon.png"
            };
        }
    }
}
