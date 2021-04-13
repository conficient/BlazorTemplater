using RazorLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RazorLightLibrary
{

    public class Renderer
    {
        public Renderer()
        {
            assembly = System.Reflection.Assembly.GetExecutingAssembly();
            engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(assembly)
                .SetOperatingAssembly(assembly)
                .UseMemoryCachingProvider()
                .Build();
        }

        readonly Assembly assembly;
        readonly RazorLightEngine engine;
        readonly Dictionary<string, string> templateCache = new Dictionary<string, string>();

        public async Task<string> RenderTemplateAsync<TModel>(TModel model)
        {
            // template name
            const string resourceName = "RazorLightLibrary.OrderView.cshtml";

            string template;
            if (!templateCache.ContainsKey(resourceName))
            {
                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                    throw new ArgumentException(resourceName + " not found");
                var reader = new StreamReader(stream);
                template = reader.ReadToEnd();
            }
            else
                template = templateCache[resourceName];

            return await engine.CompileRenderStringAsync<TModel>(resourceName, template, model);
        }

    }
}
