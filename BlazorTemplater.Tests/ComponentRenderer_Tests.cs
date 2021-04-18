using BlazorTemplater.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

    }
}