using System;

namespace BlazorTemplater.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Rendering Sample.razor to HTML..");

            var templater = new Templater();
            var html = templater.RenderComponent<Sample>();

            Console.WriteLine(html);
        }
    }
}
