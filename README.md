# BlazorTemplater
A library that generates HTML (e.g. for emails) from [Razor Components](https://docs.microsoft.com/en-us/aspnet/core/blazor/components).

[![Build](https://github.com/conficient/BlazorTemplater/actions/workflows/dotnet-core.yml/badge.svg)](https://github.com/conficient/BlazorTemplater/actions/workflows/dotnet-core.yml) [![Nuget](https://img.shields.io/nuget/dt/blazortemplater?logo=nuget&style=flat-square)](https://www.nuget.org/packages/blazortemplater/)

#### Examples

The `ComponentRenderer` uses a [fluent interface](https://en.wikipedia.org/wiki/Fluent_interface).

Let's render `MyComponent.razor` as a HTML string.
```c#
string html = new ComponentRenderer<MyComponent>().Render();
```


<details>
 <summary><code>MyComponent.razor</code></summary>
            
```html
<p>Hello from BlazorTemplater!</p>
```
            
</details>

**Parameters**

You can set parameters on a component:
```c#
var model = new Model() { Value = "Test" };
var title = "Test";
string html = new ComponentRenderer<MyComponent>()
            .Set(c => c.Model, model)
            .Set(c => c.Title, title)
            .Render();
```
MyComponent has a `Model` parameter and a `Title` parameter. The fluent interface uses a lambda expression to specify the property and ensures the value matches the property type.

<details>
  <summary><code>MyComponent.razor</code></summary>
            
```html
<h1>@Title</h1>
<p>Your model value is @Model.Value</p>
@code
{
 [Parameter] public Model Model { get; set; }
 [Parameter] public string Title { get; set; }
}
```

</details>

**Dependency Injection**

You can specify services to be provided to a component that uses `@inject`, e.g.:
```c#
string html = new ComponentRenderer<MyComponent>()
            .AddService<ITestService>(new TestService())
            .Render();
```

<details>
  <summary><code>MyComponent.razor</code></summary>
            
```html
@inject ITestService MyService
<p>Use service: @MyService.SomeFunction()</p>
```

**Add Service Provider**

New for version 1.5 is the ability to add your own `IServiceProvider` rather than adding services individually. This is useful if you need to re-use the same services repeatedly. Many thanks to [@PhotoAtomic](https://github.com/PhotoAtomic) for this feature!
```c#
IServiceProvider myServiceProvider = GetServiceProvider();
string html = new ComponentRenderer<MyComponent>()
            .AddServiceProvider(myServiceProvider)
            .Render();
```


 
</details>

**Layouts**

If a top-level component has a `@layout` attribute it will be applied. Alternatively you can apply a template explicitly:
```c#
string html = new ComponentRenderer<MyComponent>()
            .UseLayout<MyLayout>()
            .Render();
```
You can also specify via a type:
```c#
void Example(Type layout)
{
    string html = new ComponentRenderer<MyComponent>()
            .UseLayout(layout)
            .Render();
}
```
See [Layouts](Docs/Layouts.md) for more information

#### The 'kitchen sink'
You can chain them all together in any order, provided `.Render()` is last:
```c#
var model = new Model() { Value = "Test" };
var title = "Test";
string html = new ComponentRenderer<MyComponent>()
            .Set(c => c.Title, title)
            .AddService<ITestService>(new TestService())
            .Set(c => c.Model, model)
            .UseLayout<MyLayout>()
            .Render();
```

<details>
  <summary><code>MyComponent.razor</code></summary>
            
```html
@inject ITestService MyService
<h1>@Title</h1>
<p>Your model value is @Model.Value</p>
<p>Use service: @MyService.SomeFunction()</p>
@code
{
 [Parameter] public Model Model { get; set; }
 [Parameter] public string Title { get; set; }
}
```
</details>

            
#### Template Method
You can also use the older templater method (retained for compatability). See [Templater](Docs/Templater.md)

## Getting Started

Add the `BlazorTemplater` NUGET package to your library.

### Usage

See the [usage guide](Docs/Usage.md).

### Supported Project Types

`BlazorTemplater` can be used in 
 - .NET Standard 2.0
 - .NET Standard 2.1
 - .NET Core 3.1
 - .NET 5 
 - .NET 6

Libraries or applications using `BlazorTemplator` need to have the **Razor SDK** enabled to provide compilation and intellisense for `.razor` files. If you have an existing .NET Standard class library that does not have Razor Component support, follow [this guide](Docs/AddRazorSupport.md) to upgrade the library. I did have issues retrofitting Razor support into the .NET Core 3.1 unit test app, so I moved the `.razor` classes into a .NET Standard library `Templater.Library`. This should not be an issue for a Blazor WASM or Blazor Server application using .NET Core 3.1 since they already support.

## Background

Historically developers have used Razor Views (`.cshtml` files) to create easy-to-edit email templates with model-based data binding.

This was not a simple process as the `.cshtml` format wasn't always supported in non-web applications or libraries, and getting the syntax-highlighting and tooling to work with Razor Views was sometimes difficult. Two examples of this approach are [RazorLight](https://github.com/toddams/RazorLight) and [RazorEngine](https://github.com/Antaris/RazorEngine).

With the introduction of [Razor Component Libraries](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/class-libraries) that can contain `.razor` files, it's now much easier to create class libraries which include Razor-based markup than using using Razor Views (`.cshtml` files).

### Technical Differences

A `.cshtml` file was normally a text file or resource in a project that had to be parsed, validated, and a class generated at runtime to create code that could be used with a model to create the HTML required. The meant that RazorView-based libraries had to "compile" the template before it could be used, often at runtime.

In comparison, a **Razor Component** is a class created at _compile time_ so using this is much faster and simpler! Razor Components also support binding of multiple properties (not just a single `Model`), so provides more flexibility in design and usage.

## Supported Features

BlazorRenderer supports using:
 - Rendering of `.razor` templates to HTML
 - Setting `[Parameters]` on Components
 - Injecting [service dependencies](https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection) via `.AddService<..>`
 - Nested Components
 - [Code-behind Components/partial classes](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/?view=aspnetcore-5.0#partial-class-support)
 - [Cascading Values](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/cascading-values-and-parameters)
 - Layouts
### Limitations

The following are not supported/tested:
   - JavaScript
   - EventCallbacks
   - Rerendering
   - CSS and CSS isolation

#### CSS and Html Emails

CSS support in HTML emails is a complicated area. Many email clients (Outlook, GMail, Hotmail etc) have differing levels of what is supported. You can't often reference an external CSS file from the email as external references are often blocked.

A good idea is to use a utility library to pre-process and inline the CSS before creating the email body. A good example of this is [PreMailer.NET](https://github.com/milkshakesoftware/PreMailer.Net).

## Credits and Acknowledgements

The basis of the rendering system is adapted from [Steve Sanderson's](https://github.com/SteveSandersonMS) [BlazorUnitTestingPrototype](https://github.com/SteveSandersonMS/BlazorUnitTestingPrototype) repo which was written as very simple component test library for Blazor.

This was never developed into a functioning product or library. For unit testing Razor Components I recommend [Egil Hansen's bunit](https://github.com/egil/bUnit).

### Version History

| Version  | Changes |
| -------- |-----------|
| v1.0.0   | Inital Release (to Nuget) |
| v1.1.0   | **Breaking change**: renamed `BlazorTemplater` class to `Templater` [#4](https://github.com/conficient/BlazorTemplater/issues/4) |
| v1.2.0   | Added multi-targetting for .NET Std/.NET 5 to fix bug [#12](https://github.com/conficient/BlazorTemplater/issues/12) |
| v1.3.0   | Added `ComponentRenderer<T>` for fluent interface and typed parameter setting |
| v1.4.0   | Added support for Layouts |

