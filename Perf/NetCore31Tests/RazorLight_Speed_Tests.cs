using DataModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace NetCore31Tests
{
    [TestClass]
    public class RazorLight_Speed_Tests
    {
        // name
        const string name = "RazorLight .NET Core 3.1";

        [TestMethod]
        public async Task Single_Run()
        {
          
            var rand = new Random();
            const int rows = 1000;

            var order = OrderModel.CreateModel(name, rand, rows);

            // Start timing
            var sw = Stopwatch.StartNew();

            var renderer = new RazorLightLibrary.Renderer();
            var html = await renderer.RenderTemplateAsync<OrderModel>(order);

            // Stop timing
            sw.Stop();

            Console.WriteLine($"{name}: {sw.ElapsedMilliseconds}ms (single run)");
            Console.WriteLine(html);
        }


        [TestMethod]
        public async Task Repeated_RunsAsync()
        {
            const int runs = 100;

            var rand = new Random();
            const int rows = 1000;

            var order = OrderModel.CreateModel(name, rand, rows);

            // Start timing
            var sw = Stopwatch.StartNew();

            // reuse same templater for each run
            var renderer = new RazorLightLibrary.Renderer();

            for (int i = 0; i < runs; i++)
            {
                _ = await renderer.RenderTemplateAsync<OrderModel>(order);
            }

            // Stop timing
            sw.Stop();

            Console.WriteLine($"{name}: {sw.ElapsedMilliseconds}ms ({runs} runs)");
        }
    }
}
