using BlazorTemplater.Library;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace BlazorTemplater.Tests
{
    /// <summary>
    ///
    /// </summary>
    [TestClass]
    public class ComponentRenderer_Tests
    {
        [TestMethod]
        public void Ctor_Test()
        {
            var builder = new ComponentRenderer<Simple>();

            Assert.IsNotNull(builder);
        }

        #region Simple render

        /// <summary>
        /// Render a component (no service injection or parameters)
        /// </summary>
        [TestMethod]
        public void Simple_Test()
        {
            const string expected = @"<b>Jan 1st is 2021-01-01</b>";
            var actual = new ComponentRenderer<Simple>()
                .Render();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion Simple render

        #region Parameters

        /// <summary>
        /// Test a component with a parameter
        /// </summary>
        [TestMethod]
        public void ComponentBuilder_Parameters_Test()
        {
            // expected output
            const string expected = "<p>Steve Sanderson is awesome!</p>";

            var model = new TestModel()
            {
                Name = "Steve Sanderson",
                Description = "is awesome"
            };

            var html = new ComponentRenderer<Parameters>()
                .Set(c => c.Model, model)
                .Render();

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test a component with a parameter
        /// </summary>
        [TestMethod]
        public void ComponentBuilder_Parameters_TestHtmlEncoding()
        {
            // expected output
            const string expected = "<p>Safia &amp; Pranav are awesome too!</p>";

            var templater = new Templater();
            var model = new TestModel()
            {
                Name = "Safia & Pranav",    // the text here is HTML encoded
                Description = "are awesome too"
            };
            var html = new ComponentRenderer<Parameters>()
                .Set(c => c.Model, model)
                .Render();

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test a component with a parameter which isn't set
        /// </summary>
        [TestMethod]
        public void ComponentBuilder_Parameters_TestIfModelNotSet()
        {
            // expected output
            const string expected = "<p>No model!</p>";

            var html = new ComponentRenderer<Parameters>()                
                .Render();

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion Parameters

        #region Async render

        /// <summary>
        /// Test a async render, so the html is generated only after the async lifecycle is completed
        /// </summary>
        [TestMethod]
        public async Task Async_Test()
        {
            const string expected = @"<h3>AsyncRender Text: Async value</h3>";
            
            var actual = await new ComponentRenderer<AsyncRender>().RenderAsync();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion Async render

        #region Errors

        /// <summary>
        /// Test rendering model with error (null reference is expected)
        /// </summary>
        [TestMethod]
        public void ComponentRenderer_Error_Test()
        {
            var templater = new Templater();

            // we should get a NullReferenceException thrown as Model parameter is not set
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                _ = new ComponentRenderer<ErrorTest>().Render();
            });
        }

        #endregion Errors

        #region Dependency Injection

        [TestMethod]
        public void AddService_Test()
        {
            // set up
            const int a = 2;
            const int b = 3;
            const int c = a + b;
            string expected = $"<p>If you add {a} and {b} you get {c}</p>";

            // fluent ComponentBuilder approach
            var actual = new ComponentRenderer<ServiceInjection>()
                .AddService<ITestService>(new TestService())
                .Set(p => p.A, a)
                .Set(p => p.B, b)
                .Render();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AddServiceProvider_Test()
        {
            // set up
            const int a = 2;
            const int b = 3;
            const int c = a + b;
            string expected = $"<p>If you add {a} and {b} you get {c}</p>";

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestService>(new TestService());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // fluent ComponentBuilder approach
            var actual = new ComponentRenderer<ServiceInjection>()
                .AddServiceProvider(serviceProvider)
                .Set(p => p.A, a)
                .Set(p => p.B, b)
                .Render();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Nesting

        /// <summary>
        /// Test that a component containing other components render correctly
        /// </summary>
        [TestMethod]
        public void ComponentRenderer_Nested_Test()
        {
            // expected output
            // the spaces before the <p> come from the Parameters.razor component
            // on Windows the string contains \r\n and on unix it's just \n
            string expected = $"<b>Jan 1st is 2021-01-01</b>{Environment.NewLine}    <p>Dan Roth is cool!</p>";

            var templater = new Templater();
            var model = new TestModel()
            {
                Name = "Dan Roth",
                Description = "is cool"
            };
            var html = new ComponentRenderer<NestedComponents>()
                .Set(c => c.Model, model)
                .Render();

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Cascading Values

        [TestMethod]
        public void ComponentRenderer_CascadingValues_Test()
        {
            const string expected = "<p>The name is Bill</p>";
            var info = new CascadeInfo() { Name = "Bill" };

            var html = new ComponentRenderer<CascadeParent>()
                .Set(c => c.Info, info)
                .Render();

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
        public void LayoutAttribute_Test()
        {
            // this text appears in the layout:
            const string expectedTemplate = "<title>LayoutFile</title>";
            // this text appears in the content:
            const string expectedContent = "<p>Name = Test!</p>";

            var html = new ComponentRenderer<LayoutComponent>()
                .Set(x => x.Name, "Test!")
                .Render();

            Console.WriteLine(html);

            StringAssert.Contains(html, expectedTemplate, "Expected template was not found");
            StringAssert.Contains(html, expectedContent, "Expected content was not found");
        }

        /// <summary>
        /// test that .UseLayout&lt;TLayout&gt; works
        /// </summary>
        [TestMethod]
        public void UseLayoutT_Test()
        {
            // this text appears in the layout:
            const string expectedTemplate = "<title>Layout2</title>";
            // this text appears in the content:
            const string expectedContent = @"<b>Jan 1st is 2021-01-01</b>";

            // render a component that has no layout
            var html = new ComponentRenderer<Simple>()
                .UseLayout<Layout2>()   // set the layout
                .Render();

            Console.WriteLine(html);

            StringAssert.Contains(html, expectedTemplate, "Expected template was not found");
            StringAssert.Contains(html, expectedContent, "Expected content was not found");
        }

        /// <summary>
        /// test that .UseLayout&lt;TLayout&gt; works
        /// </summary>
        [TestMethod]
        public void UseLayout_Test()
        {
            // this text appears in the layout:
            const string expectedTemplate = "<title>LayoutFile</title>";
            // this text appears in the content:
            const string expectedContent = @"<b>Jan 1st is 2021-01-01</b>";

            // get layout type:
            var layout = typeof(LayoutFile);

            // render a component that has no layout
            var html = new ComponentRenderer<Simple>()
                .UseLayout(layout)   // set the layout
                .Render();

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
        public void UseLayout_ThrowsIfTypeInvalid()
        {
            // NOT a layout type:
            var layout = typeof(Simple);

            // render a component that has no layout
            var cr = new ComponentRenderer<Simple>();

            Assert.ThrowsException<ArgumentException>(() =>
            {
                // try to set the layout
                cr.UseLayout(layout);
            });
        }

        #endregion

    }

}