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
    public class RazorLightSpeed_Tests
    {
        // name
        const string name = "RazorLight .NET Core 3.1";

        [TestMethod]
        public async Task Single_Run()
        {
            // template name
            const string resource = "RazorLightLibrary.OrderView.cshtml";

            var rand = new Random();
            const int rows = 1000;

            var order = OrderModel.CreateModel(name, rand, rows);

            // Start timing
            var sw = Stopwatch.StartNew();

            var renderer = new RazorLightLibrary.Renderer();
            var html = await renderer.RenderTemplateAsync<OrderModel>(resource, order);

            // Stop timing
            sw.Stop();

            Console.WriteLine($"{name}: {sw.ElapsedMilliseconds}ms (single run)");
            Console.WriteLine(html);
        }


        //[TestMethod]
        //public void Repeated_Runs()
        //{
        //    const int runs = 100;

        //    var rand = new Random();
        //    const int rows = 1000;

        //    var order = OrderModel.CreateModel(name, rand, rows);

        //    // Start timing
        //    var sw = Stopwatch.StartNew();

        //    // reuse same templater for each run
        //    var templater = new BlazorTemplater.Templater();

        //    for (int i = 0; i < runs; i++)
        //    {
        //        var parameters = new Dictionary<string, object>()
        //        {
        //            { nameof(RazorClassLibrary1.OrderView.Order), order }
        //        };  
        //        _ = templater.RenderComponent<RazorClassLibrary1.OrderView>(parameters);
        //    }

        //    // Stop timing
        //    sw.Stop();

        //    Console.WriteLine($"{name}: {sw.ElapsedMilliseconds}ms ({runs} runs)");
        //}
    }
}
