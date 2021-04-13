using DataModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NetCore31Tests
{
    [TestClass]
    public class RazorTemplating_Speed_Tests
    {
        // name
        const string name = "RazorTemplating .NET Core 3.1";

        [TestMethod]
        public async Task Single_Run()
        {
            var rand = new Random();
            const int rows = 1000;

            var order = OrderModel.CreateModel(name, rand, rows);

            // Start timing
            var sw = Stopwatch.StartNew();

            var html = await RazorTemplatingLibrary.Renderer.RenderOrderAsync(order);
            // Stop timing
            sw.Stop();

            Console.WriteLine($"{name}: {sw.ElapsedMilliseconds}ms (single run)");
            Console.WriteLine(html);
        }


        [TestMethod]
        public async Task Repeated_Runs()
        {
            const int runs = 100;

            var rand = new Random();
            const int rows = 1000;

            var order = OrderModel.CreateModel(name, rand, rows);

            // Start timing
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < runs; i++)
            {
                _ = await RazorTemplatingLibrary.Renderer.RenderOrderAsync(order);
            }

            // Stop timing
            sw.Stop();

            Console.WriteLine($"{name}: {sw.ElapsedMilliseconds}ms ({runs} runs)");
        }
    }
}
