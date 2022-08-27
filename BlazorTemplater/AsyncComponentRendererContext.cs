#nullable enable

using System.IO;

namespace BlazorTemplater
{
    internal class AsyncComponentRendererContext
    {
        public AsyncComponentRendererContext(TextWriter textWriter)
        {
            TextWriter = textWriter;
        }

        public TextWriter TextWriter { get; }

        public string? ClosestSelectValue { get; set; }
    }
}