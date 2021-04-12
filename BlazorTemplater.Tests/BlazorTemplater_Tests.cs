using BlazorTemplater.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BlazorTemplater.Tests
{
    [TestClass]
    public class BlazorTemplater_Tests
    {
        #region Ctor

        [TestMethod]
        public void Ctor_Test()
        {
            var templater = new BlazorTemplater();
            Assert.IsNotNull(templater);
        }

        #endregion Ctor

        #region Simple render

        /// <summary>
        /// Test a simple component with no parameters
        /// </summary>
        [TestMethod]
        public void RenderComponent_Simple_Test()
        {
            const string expected = @"<b>Jan 1st is 2021-01-01</b>";

            var renderer = new BlazorTemplater();
            var actual = renderer.RenderComponent<Simple>();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion Simple render

        #region Parameters

        /// <summary>
        /// Test a component with a parameter
        /// </summary>
        [TestMethod]
        public void RenderComponent_Parameters_Test()
        {
            // expected output
            const string expected = "<p>Steve Sanderson is awesome!</p>";

            var renderer = new BlazorTemplater();
            var model = new TestModel()
            {
                Name = "Steve Sanderson",
                Description = "is awesome"
            };
            var parameters = new Dictionary<string, object>()
            {
                { nameof(Parameters.Model), model }
            };
            var html = renderer.RenderComponent<Parameters>(parameters);

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test a component with a parameter
        /// </summary>
        [TestMethod]
        public void RenderComponent_Parameters_TestHtmlEncoding()
        {
            // expected output
            const string expected = "<p>Safia &amp; Pranav are awesome too!</p>";

            var renderer = new BlazorTemplater();
            var model = new TestModel()
            {
                Name = "Safia & Pranav",
                Description = "are awesome too"
            };
            var parameters = new Dictionary<string, object>()
            {
                { nameof(Parameters.Model), model }
            };
            var html = renderer.RenderComponent<Parameters>(parameters);

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test a component with a parameter which isn't set
        /// </summary>
        [TestMethod]
        public void RenderComponent_Parameters_TestIfModelNotSet()
        {
            // expected output
            const string expected = "<p>No model!</p>";

            var renderer = new BlazorTemplater();
            var html = renderer.RenderComponent<Parameters>();

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
        public void RenderComponent_Error_Test()
        {
            var renderer = new BlazorTemplater();

            // we should get a NullReferenceException thrown as Model parameter is not set
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                _ = renderer.RenderComponent<ErrorTest>();
            });
        }

        #endregion Errors

        #region Dependency Injection

        /// <summary>
        /// Test that Service Injection works
        /// </summary>
        [TestMethod]
        public void AddService_TestInstanceMethod()
        {
            // set up
            const int a = 2;
            const int b = 3;
            const int c = a + b;
            string expected = $"<p>If you add {a} and {b} you get {c}</p>";

            // create a renderer and register an ITestService. The service adds values
            var renderer = new BlazorTemplater();
            renderer.AddService<ITestService>(new TestService());

            var parameters = new Dictionary<string, object>()
            {
                { nameof(ServiceInjection.A), a },
                { nameof(ServiceInjection.B), b }
            };
            var actual = renderer.RenderComponent<ServiceInjection>(parameters);

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion Dependency Injection

        #region Nesting

        /// <summary>
        /// Test that a component containing other components render correctly
        /// </summary>
        [TestMethod]
        public void RenderComponent_Nested_Test()
        {
            // expected output
            // the spaces before the <p> come from the Parameters.razor component
            const string expected = "<b>Jan 1st is 2021-01-01</b>\r\n    <p>Dan Roth is cool!</p>";

            var renderer = new BlazorTemplater();
            var model = new TestModel()
            {
                Name = "Dan Roth",
                Description = "is cool"
            };
            var parameters = new Dictionary<string, object>()
            {
                { nameof(Parameters.Model), model }
            };
            var html = renderer.RenderComponent<NestedComponents>(parameters);

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Multiple Renders

        /// <summary>
        /// The BlazorRenderer instance can be re-used if the services required are the save
        /// </summary>
        [TestMethod]
        public void RenderComponent_ReusingRenderer()
        {
            // set up
            const int a = 2;
            const int b = 3;
            const int c = a + b;
            string expected1 = $"<p>If you add {a} and {b} you get {c}</p>";

            const int x = 456;
            const int y = 123;
            const int z = x + y;
            string expected2 = $"<p>If you add {x} and {y} you get {z}</p>";

            // create a renderer and register an ITestService. The service adds values
            var renderer = new BlazorTemplater();
            renderer.AddService<ITestService>(new TestService());

            var parameters1 = new Dictionary<string, object>()
            {
                { nameof(ServiceInjection.A), a },
                { nameof(ServiceInjection.B), b }
            };

            // new parameters
            var parameters2 = new Dictionary<string, object>()
            {
                { nameof(ServiceInjection.A), x },
                { nameof(ServiceInjection.B), y }
            };

            // render both components
            var actual1 = renderer.RenderComponent<ServiceInjection>(parameters1);
            var actual2 = renderer.RenderComponent<ServiceInjection>(parameters2);

            Console.WriteLine(actual1);
            Console.WriteLine(actual2);


            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(expected2, actual2);

        }

        #endregion
    }
}