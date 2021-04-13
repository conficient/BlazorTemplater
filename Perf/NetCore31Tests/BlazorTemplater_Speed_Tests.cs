using DataModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetCore31Tests
{
    [TestClass]
    public class BlazorTemplater_Speed_Tests
    {
        // name
        const string name = "BlazorTemplater .NET Core 3.1";

        [TestMethod]
        public void Single_Run()
        {
            var invoice = Invoice.Create();

            // Start timing
            var sw = Stopwatch.StartNew();

            var templater = new BlazorTemplater.Templater();
            var parameters = new Dictionary<string, object>()
            {
                { nameof(RazorComponentLibrary.Invoice.Model), invoice }
            };
            var html = templater.RenderComponent<RazorComponentLibrary.Invoice>(parameters);

            // Stop timing
            sw.Stop();

            Console.WriteLine($"{name}: {sw.ElapsedMilliseconds}ms (single run)");
            Console.WriteLine(html);
        }


        [TestMethod]
        public void Repeated_Runs()
        {
            const int runs = 100;
            var invoice = Invoice.Create();

            // Start timing
            var sw = Stopwatch.StartNew();

            // reuse same templater for each run
            var templater = new BlazorTemplater.Templater();
            var parameters = new Dictionary<string, object>()
                {
                    { nameof(RazorComponentLibrary.Invoice.Model), invoice }
                };

            for (int i = 0; i < runs; i++)
            {
                _ = templater.RenderComponent<RazorComponentLibrary.Invoice>(parameters);
            }

            // Stop timing
            sw.Stop();

            Console.WriteLine($"{name}: {sw.ElapsedMilliseconds}ms ({runs} runs)");
        }
    }
}
