#nullable enable

using BlazorTemplater.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlazorTemplater.Tests
{
    /// <summary>
    ///
    /// </summary>
    [TestClass]
    public class AsyncComponentRenderer_Tests
    {
        [TestMethod]
        public void Factory_Test()
        {
            var renderer = CreateRenderer<Simple>();

            Assert.IsNotNull(renderer);
        }

        #region Simple render

        /// <summary>
        /// Render a component (no service injection or parameters)
        /// </summary>
        [TestMethod]
        public async Task Simple_Test()
        {
            const string expected = @"<b>Jan 1st is 2021-01-01</b>";
            var actual = await CreateRenderer<Simple>()
                .RenderAsync(new Dictionary<string, object?>());

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion Simple render

        #region Parameters

        /// <summary>
        /// Test a component with a parameter
        /// </summary>
        [TestMethod]
        public async Task ComponentBuilder_Parameters_Test()
        {
            // expected output
            const string expected = "<p>Steve Sanderson is awesome!</p>";

            var model = new TestModel()
            {
                Name = "Steve Sanderson",
                Description = "is awesome"
            };

            var parameters = new Dictionary<string, object?>
            {
                { nameof(Parameters.Model), model },
            };

            var html = await CreateRenderer<Parameters>()
                .RenderAsync(parameters);

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test a component with a parameter
        /// </summary>
        [TestMethod]
        public async Task ComponentBuilder_Parameters_TestHtmlEncoding()
        {
            // expected output
            const string expected = "<p>Safia &amp; Pranav are awesome too!</p>";

            var model = new TestModel()
            {
                Name = "Safia & Pranav",    // the text here is HTML encoded
                Description = "are awesome too"
            };

            var parameters = new Dictionary<string, object?>
            {
                { nameof(Parameters.Model), model },
            };

            var html = await CreateRenderer<Parameters>()
                .RenderAsync(parameters);

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test a component with a parameter which isn't set
        /// </summary>
        [TestMethod]
        public async Task ComponentBuilder_Parameters_TestIfModelNotSet()
        {
            // expected output
            const string expected = "<p>No model!</p>";

            var html = await CreateRenderer<Parameters>()
                .RenderAsync(new Dictionary<string, object?>());

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion Parameters

        #region Errors

        /// <summary>
        /// Test rendering model with error (null reference is expected)
        /// </summary>
        [TestMethod]
        public async Task ComponentRenderer_Error_Test()
        {
            var templater = new Templater();

            // we should get a NullReferenceException thrown as Model parameter is not set
            await Assert.ThrowsExceptionAsync<NullReferenceException>(async () =>
            {
                _ = await CreateRenderer<ErrorTest>().RenderAsync(new Dictionary<string, object?>());
            });
        }

        #endregion Errors

        #region Dependency Injection

        [TestMethod]
        public async Task AddService_Test()
        {
            // set up
            const int a = 2;
            const int b = 3;
            const int c = a + b;
            string expected = $"<p>If you add {a} and {b} you get {c}</p>";

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestService, TestService>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = NullLoggerFactory.Instance;

            var factory = new AsyncComponentRendererFactory<ServiceInjection>(serviceProvider, loggerFactory);
            var renderer = factory.CreateRenderer();

            var parameters = new Dictionary<string, object?>()
            {
                { nameof(ServiceInjection.A), a },
                { nameof(ServiceInjection.B), b },
            };

            var actual = await renderer.RenderAsync(parameters);

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task AddServiceProvider_Test()
        {
            // set up
            const int a = 2;
            const int b = 3;
            const int c = a + b;
            string expected = $"<p>If you add {a} and {b} you get {c}</p>";

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestService, TestService>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = NullLoggerFactory.Instance;

            var factory = new AsyncComponentRendererFactory<ServiceInjection>(serviceProvider, loggerFactory);
            var renderer = factory.CreateRenderer();

            var parameters = new Dictionary<string, object?>()
            {
                { nameof(ServiceInjection.A), a },
                { nameof(ServiceInjection.B), b },
            };

            var actual = await renderer.RenderAsync(parameters);

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Nesting

        /// <summary>
        /// Test that a component containing other components render correctly
        /// </summary>
        [TestMethod]
        public async Task ComponentRenderer_Nested_Test()
        {
            // expected output
            // the spaces before the <p> come from the Parameters.razor component
            // on Windows the string contains \r\n and on unix it's just \n
            string expected = $"<b>Jan 1st is 2021-01-01</b>{Environment.NewLine}    <p>Dan Roth is cool!</p>";

            var model = new TestModel()
            {
                Name = "Dan Roth",
                Description = "is cool"
            };

            var parameters = new Dictionary<string, object?>()
            {
                { nameof(NestedComponents.Model), model },
            };

            var html = await CreateRenderer<NestedComponents>()
                .RenderAsync(parameters);

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Cascading Values

        [TestMethod]
        public async Task ComponentRenderer_CascadingValues_Test()
        {
            const string expected = "<p>The name is Bill</p>";
            var info = new CascadeInfo() { Name = "Bill" };

            var parameters = new Dictionary<string, object?>
            {
                { nameof(CascadeParent.Info), info },
            };

            var html = await CreateRenderer<CascadeParent>()
                .RenderAsync(parameters);

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Assert.AreEqual(expected, actual);

        }

        #endregion

        #region Layouts

        /// <summary>
        /// Renders a component with a LayoutAttribute correctly
        /// </summary>
        [TestMethod]
        public async Task LayoutAttribute_Test()
        {
            // this text appears in the layout:
            const string expectedTemplate = "<title>LayoutFile</title>";
            // this text appears in the content:
            const string expectedContent = "<p>Name = Test!</p>";

            var parameters = new Dictionary<string, object?>()
            {
                { nameof(LayoutComponent.Name), "Test!" },
            };

            var html = await CreateRenderer<LayoutComponent>()
                .RenderAsync(parameters);

            Console.WriteLine(html);

            StringAssert.Contains(html, expectedTemplate, "Expected template was not found");
            StringAssert.Contains(html, expectedContent, "Expected content was not found");
        }

        /// <summary>
        /// test that .UseLayout&lt;TLayout&gt; works
        /// </summary>
        [TestMethod]
        public async Task UseLayoutT_Test()
        {
            // this text appears in the layout:
            const string expectedTemplate = "<title>Layout2</title>";
            // this text appears in the content:
            const string expectedContent = @"<b>Jan 1st is 2021-01-01</b>";

            // render a component that has no layout
            var html = await CreateRenderer<Simple, Layout2>()
                .RenderAsync(new Dictionary<string, object?>());

            Console.WriteLine(html);

            StringAssert.Contains(html, expectedTemplate, "Expected template was not found");
            StringAssert.Contains(html, expectedContent, "Expected content was not found");
        }

        /// <summary>
        /// test that .UseLayout&lt;TLayout&gt; works
        /// </summary>
        [TestMethod]
        public async Task UseLayout_Test()
        {
            // this text appears in the layout:
            const string expectedTemplate = "<title>LayoutFile</title>";
            // this text appears in the content:
            const string expectedContent = @"<b>Jan 1st is 2021-01-01</b>";

            // get layout type:
            var layout = typeof(LayoutFile);

            // render a component that has no layout
            var html = await CreateRenderer<Simple>(layout)
                .RenderAsync(new Dictionary<string, object?>());

            Console.WriteLine(html);

            StringAssert.Contains(html, expectedTemplate, "Expected template was not found");
            StringAssert.Contains(html, expectedContent, "Expected content was not found");
        }


        /// <summary>
        /// test that .UseLayout&lt;TLayout&gt; will not accept a type that does not inherit from LayoutComponentBase
        /// </summary>
        /// <remarks>
        /// The Type-based `UseLayout` can only validate at runtime, whereas the generic version
        /// can validate at compile time.
        /// </remarks>
        [TestMethod]
        public async Task UseLayout_ThrowsIfTypeInvalid()
        {
            // NOT a layout type:
            var layout = typeof(Simple);

            Assert.ThrowsException<ArgumentException>(() =>
            {
                // try to set the layout
                var renderer = CreateRenderer<Simple>(layout);
            });
        }

        [TestMethod]
        public async Task UseLayout_TestMultipleRenders()
        {
            // this text appears in the layout:
            const string expectedTemplate = "<title>LayoutFile</title>";
            // this text appears in the content:
            const string expectedContent1 = "<p>Steve Sanderson is awesome!</p>";
            const string expectedContent2 = "<p>Safia &amp; Pranav are awesome too!</p>";

            var model1 = new TestModel()
            {
                Name = "Steve Sanderson",
                Description = "is awesome"
            };

            var parameters1 = new Dictionary<string, object?>
            {
                { nameof(ParametersSet.Model), model1 },
            };

            var model2 = new TestModel()
            {
                Name = "Safia & Pranav",    // the text here is HTML encoded
                Description = "are awesome too"
            };

            var parameters2 = new Dictionary<string, object?>
            {
                { nameof(ParametersSet.Model), model2 },
            };

            var renderer = CreateRenderer<ParametersSet, LayoutFile>();

            var html1 = await renderer.RenderAsync(parameters1);
            var html2 = await renderer.RenderAsync(parameters2);

            Console.WriteLine(html1);
            Console.WriteLine(html2);

            StringAssert.Contains(html1, expectedTemplate, "Expected template was not found");
            StringAssert.Matches(html1, new Regex(expectedContent1), "Expected content was not found");
            StringAssert.DoesNotMatch(html1, new Regex(expectedContent2), "Expected content was not found");

            StringAssert.Contains(html2, expectedTemplate, "Expected template was not found");
            StringAssert.Matches(html2, new Regex(expectedContent1), "Expected content was not found");
            StringAssert.Matches(html2, new Regex(expectedContent2), "Expected content was not found");
        }

        #endregion

        private static AsyncComponentRenderer CreateRenderer<TComponent>(Type? layoutType = null)
            where TComponent : IComponent, new()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = NullLoggerFactory.Instance;

            var factory = layoutType is null 
                ? (IAsyncComponentRendererFactory)new AsyncComponentRendererFactory<TComponent>(serviceProvider, loggerFactory)
                : new AsyncComponentRendererFactory(typeof(TComponent), layoutType, serviceProvider, loggerFactory);

            return factory.CreateRenderer();
        }

        private static AsyncComponentRenderer CreateRenderer<TComponent, TLayout>()
            where TComponent : IComponent, new()
            where TLayout : LayoutComponentBase
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerFactory = NullLoggerFactory.Instance;

            var factory = new AsyncComponentRendererFactory<TComponent, TLayout>(serviceProvider, loggerFactory);
            return factory.CreateRenderer();
        }

    }

}