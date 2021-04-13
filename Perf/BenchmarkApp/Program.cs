using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BlazorTemplater;
using DataModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BenchmarkApp
{
    class Program
    {
        static void Main()
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
            Console.WriteLine(summary);
        }
    }

    [MemoryDiagnoser, MinColumn, MaxColumn]
    public class RenderTests
    {
        private readonly Invoice invoice;
        private readonly Templater templater;
        private readonly Dictionary<string, object> parms;

        public RenderTests()
        {
            // create models to use
            invoice = Invoice.Create();
            templater = new Templater();
            parms = new Dictionary<string, object>()
            {
                { nameof(RazorComponentLibrary.Invoice.Model), invoice }
            };
        }


        [Benchmark]
        public string Test_BlazorTemplater()
        {
            return templater.RenderComponent<RazorComponentLibrary.Invoice>(parms);
        }
        
        [Benchmark]
        public async Task<string> Test_RazorTemplating()
        {
            return await RazorTemplatingLibrary.Renderer.RenderOrderAsync(invoice);
        }


    }
}
