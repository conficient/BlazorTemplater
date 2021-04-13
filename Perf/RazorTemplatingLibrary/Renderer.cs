using DataModel;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorTemplatingLibrary
{
    public static class Renderer
    {
        public static async Task<string> RenderOrderAsync(Invoice model)
        {
            return await RazorTemplateEngine.RenderAsync("~/Invoice.cshtml", model);
        }

    }
}
