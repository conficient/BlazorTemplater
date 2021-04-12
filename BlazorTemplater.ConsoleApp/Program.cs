using System;

namespace BlazorTemplater.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Rendering Sample.razor to HTML..");

            var renderer = new BlazorTemplater();
            var html = renderer.RenderComponent<Sample>();

            Console.WriteLine(html);
        }
    }
}
