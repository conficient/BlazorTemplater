using BlazorTemplater.Library;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BlazorTemplater.Tests
{
    [TestClass]
    public class Templater_Tests
    {
        #region Ctor

        [TestMethod]
        public void Ctor_Test()
        {
            var templater = new Templater();
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

            var templater = new Templater();
            var actual = templater.RenderComponent<Simple>();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion Simple render

        #region Non-Generic Simple render

        /// <summary>
        /// Test a simple component with no parameters
        /// </summary>
        [TestMethod]
        public void RenderComponent_Simple_TestNonGeneric()
        {
            const string expected = @"<b>Jan 1st is 2021-01-01</b>";

            var templater = new Templater();
            var actual = templater.RenderComponent(typeof(Simple));

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

            var templater = new Templater();
            var model = new TestModel()
            {
                Name = "Steve Sanderson",
                Description = "is awesome"
            };
            var parameters = new Dictionary<string, object>()
            {
                { nameof(Parameters.Model), model }
            };
            var html = templater.RenderComponent<Parameters>(parameters);

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

            var templater = new Templater();
            var model = new TestModel()
            {
                Name = "Safia & Pranav",
                Description = "are awesome too"
            };
            var parameters = new Dictionary<string, object>()
            {
                { nameof(Parameters.Model), model }
            };
            var html = templater.RenderComponent<Parameters>(parameters);

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

            var templater = new Templater();
            var html = templater.RenderComponent<Parameters>();

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
            var templater = new Templater();

            // we should get a NullReferenceException thrown as Model parameter is not set
            Assert.ThrowsException<NullReferenceException>(() =>
            {
                _ = templater.RenderComponent<ErrorTest>();
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

            // create a templater and register an ITestService. The service adds values
            var templater = new Templater();
            templater.AddService<ITestService>(new TestService());

            var parameters = new Dictionary<string, object>()
            {
                { nameof(ServiceInjection.A), a },
                { nameof(ServiceInjection.B), b }
            };
            var actual = templater.RenderComponent<ServiceInjection>(parameters);

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        //Test that services from injected service providers works
        [TestMethod]
        public void AddServiceProvider_TestInstanceMethod()
        {
            // set up
            const int a = 2;
            const int b = 3;
            const int c = a + b;
            string expected = $"<p>If you add {a} and {b} you get {c}</p>";

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ITestService>(new TestService());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // create a templater and register an ITestService. The service adds values
            var templater = new Templater();
            templater.AddServiceProvider(serviceProvider);

            var parameters = new Dictionary<string, object>()
            {
                { nameof(ServiceInjection.A), a },
                { nameof(ServiceInjection.B), b }
            };
            var actual = templater.RenderComponent<ServiceInjection>(parameters);

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
            // on Windows the string contains \r\n and on unix it's just \n
            string expected = $"<b>Jan 1st is 2021-01-01</b>{Environment.NewLine}    <p>Dan Roth is cool!</p>";

            var templater = new Templater();
            var model = new TestModel()
            {
                Name = "Dan Roth",
                Description = "is cool"
            };
            var parameters = new Dictionary<string, object>()
            {
                { nameof(Parameters.Model), model }
            };
            var html = templater.RenderComponent<NestedComponents>(parameters);

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Cascading Values

        [TestMethod]
        public void RenderComponent_CascadingValues_Test()
        {
            const string expected = "<p>The name is Bill</p>";
            var info = new CascadeInfo() { Name = "Bill" };

            var templater = new Templater();
            var parameters = new Dictionary<string, object>()
            {
                { nameof(CascadeParent.Info), info}
            };
            var html = templater.RenderComponent<CascadeParent>(parameters);

            // trim leading space and trailing CRLF from output
            var actual = html.Trim();

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
            // create a templater and register an ITestService. The service adds values
            var templater = new Templater();
            templater.AddService<ITestService>(new TestService());

            // Render the component 100 times
            const int count = 100;
            for (int a = 1; a <= count; a++)
            {
                // set up
                const int b = 3;
                int c = a + b;
                string expected = $"<p>If you add {a} and {b} you get {c}</p>";

                var parameters = new Dictionary<string, object>()
                {
                    { nameof(ServiceInjection.A), a },
                    { nameof(ServiceInjection.B), b }
                };

                // render both components
                var actual = templater.RenderComponent<ServiceInjection>(parameters);

                Console.WriteLine(actual);
                Assert.AreEqual(expected, actual);
            }
        }

        #endregion

        #region Code-Behind Test

        /// <summary>
        /// Test a simple component with no parameters
        /// </summary>
        [TestMethod]
        public void RenderComponent_CodeBehind_Test()
        {
            const string expected = @"<p>Yes Jon, we can use code-behind files with BlazorTemplater</p>";

            var templater = new Templater();
            var parameters = new Dictionary<string, object>()
            {
                { nameof(CodeBehind.App), "BlazorTemplater" },
                { nameof(CodeBehind.Name), "Jon" }
            };
            var actual = templater.RenderComponent<CodeBehind>(parameters);

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Layouts

        /// <summary>
        /// Verify that a component using `@layout ...` is applied
        /// </summary>
        [TestMethod]
        public void Layout_AppliedFromAttribute()
        {
            const string expectedContent = @"<p>Name = Test!</p>";
            const string expectedLayout = @"<title>LayoutFile</title>";

            var templater = new Templater();
            var parameters = new Dictionary<string, object>()
            {
                { nameof(LayoutComponent.Name), "Test!" }
            };
            var actual = templater.RenderComponent<LayoutComponent>(parameters);

            Console.WriteLine(actual);
            StringAssert.Contains(actual, expectedContent, "Content not found");
            StringAssert.Contains(actual, expectedLayout);
        }

        [TestMethod]
        public void UseLayoutT_AppliesLayout()
        {
            const string expectedContent = @"<b>Jan 1st is 2021-01-01</b>";
            const string expectedLayout = @"<title>LayoutFile</title>";

            var templater = new Templater();
            templater.UseLayout<LayoutFile>();
            // render Simple component that does not have a layout
            var actual = templater.RenderComponent<Simple>();

            Console.WriteLine(actual);
            StringAssert.Contains(actual, expectedContent, "Content not found");
            StringAssert.Contains(actual, expectedLayout);
        }

        [TestMethod]
        public void UseLayout_AppliesLayout()
        {
            const string expectedContent = @"<b>Jan 1st is 2021-01-01</b>";
            const string expectedLayout = @"<title>LayoutFile</title>";

            var layoutToUse = typeof(LayoutFile);

            var templater = new Templater();
            // use a type parameter
            templater.UseLayout(layoutToUse);
            // render Simple component that does not have a layout
            var actual = templater.RenderComponent<Simple>();

            Console.WriteLine(actual);
            StringAssert.Contains(actual, expectedContent, "Content not found");
            StringAssert.Contains(actual, expectedLayout);
        }

        [TestMethod]
        public void UseLayout_ThrowsIfTypeIsNotValid()
        {
            // type does not inherit from LayoutComponentBase
            var layoutToUse = typeof(Simple);

            var templater = new Templater();
            // attempt to use invalid layout:
            Assert.ThrowsException<ArgumentException>(() =>
            {
                templater.UseLayout(layoutToUse);
            });
        }

        [TestMethod]
        public void GetLayoutFromAttribute_TestWhenPresent()
        {
            // LayoutComponent should specify LayoutFile as the layout
            var expected = typeof(LayoutFile);

            var actual = Templater.GetLayoutFromAttribute<LayoutComponent>();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetLayoutFromAttribute_TestWhenNotPresent()
        {
            // Simple component has no layout
            var actual = Templater.GetLayoutFromAttribute<Simple>();

            Assert.IsNull(actual);
        }

        #endregion

        #region Css Isolated
        /// <summary>
        /// Test a SimpleWithCss component with no parameters
        /// </summary>
        [TestMethod]
        public void RenderComponent_SimpleWithCss_Test()
        {
            const string expected = @"<style type=""text/css"">p
{
    font-size: large;
    color: darkblue;
    width: 100%;
    padding: 1rem;
    text-align: center;
}</style><p>
    <b>Jan 1st is 2021-01-01</b>
</p>";

            var templater = new Templater();
            var actual = templater.RenderComponent<SimpleWithCss>();

            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        } 

        /// <summary>
        /// Test a SimpleWithCss component with no parameters
        /// </summary>
        [TestMethod]
        public void RenderComponent_TemplatedSimpleWithCss_Test()
        {
            const string expected = @"
            <p>This is a layout component that should be rendered components!</p>
            <div class=""content px-4"">
                <style type=""text/css"">p {
    font-size: large;
    color: darkgreen;
    width: 100%;
    padding: 1rem;
    text-align: center;
}p.child {
    font-size: medium;
    color: darkblue;
    width: 100%;
    padding: 1rem;
    text-align: center;
}</style><p class=""child"">
    <b>Jan 1st is 2021-01-01</b>
</p>";

            var templater = new Templater();
            var actual = templater.RenderComponent<TemplatedSimpleWithCss>();

            Console.WriteLine(actual);
            StringAssert.Contains(actual, expected, "Content not found");
        }  

        /// <summary>
        /// Test a SimpleWithCss component with no parameters
        /// </summary>
        [TestMethod]
        public void RenderComponent_MultipleComponentsWithCss_Test()
        {
            const string expected = @"<p>This is a layout component that should be rendered components!</p>
            <div class=""content px-4"">
                <style type=""text/css"">.nested {
    font-size: large;
    color: darkblue;
    width: 100%;
    padding: 1rem;
    text-align: center;
}

.nested strong{
    color: darkred;
    font-style: italic;
}p {
    font-size: large;
    color: darkgreen;
    width: 100%;
    padding: 1rem;
    text-align: center;
}p
{
    font-size: large;
    color: darkblue;
    width: 100%;
    padding: 1rem;
    text-align: center;
}</style><p>
    <b>Jan 1st is 2021-01-01</b>
</p>
<p class=""nested"">
    This component has <strong>owned CSS</strong>
</p>";

            var templater = new Templater();
            var actual = templater
                .AddScoppedCss<ComponentWithCss>()
                .RenderComponent<MultipleComponentsWithCss>();

            Console.WriteLine(actual);
            StringAssert.Contains(actual, expected, "Content not found");
        }
        #endregion Simple render
    }
}