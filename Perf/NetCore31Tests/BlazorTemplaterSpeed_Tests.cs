using DataModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetCore31Tests
{
    [TestClass]
    public class BlazorTemplaterSpeed_Tests
    {
        // name
        const string name = "BlazorTemplater .NET Core 3.1";

        [TestMethod]
        public void Single_Run()
        {
            var rand = new Random();
            const int rows = 1000;

            var order = OrderModel.CreateModel(name, rand, rows);

            // Start timing
            var sw = Stopwatch.StartNew();

            var templater = new BlazorTemplater.Templater();
            var parameters = new Dictionary<string, object>()
            {
                { nameof(RazorComponentLibrary.OrderView.Order), order }
            };
            var html = templater.RenderComponent<RazorComponentLibrary.OrderView>(parameters);

            // Stop timing
            sw.Stop();

            Console.WriteLine($"{name}: {sw.ElapsedMilliseconds}ms (single run)");
            Console.WriteLine(html);
        }


        [TestMethod]
        public void Repeated_Runs()
        {
            const int runs = 100;

            var rand = new Random();
            const int rows = 1000;

            var order = OrderModel.CreateModel(name, rand, rows);

            // Start timing
            var sw = Stopwatch.StartNew();

            // reuse same templater for each run
            var templater = new BlazorTemplater.Templater();

            for (int i = 0; i < runs; i++)
            {
                var parameters = new Dictionary<string, object>()
                {
                    { nameof(RazorComponentLibrary.OrderView.Order), order }
                };  
                _ = templater.RenderComponent<RazorComponentLibrary.OrderView>(parameters);
            }

            // Stop timing
            sw.Stop();

            Console.WriteLine($"{name}: {sw.ElapsedMilliseconds}ms ({runs} runs)");
        }
    }
}
