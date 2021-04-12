# BlazorTemplater
A library that generates HTML (e.g. for emails) from [Razor Components](https://docs.microsoft.com/en-us/aspnet/core/blazor/components).

[![Build](https://github.com/conficient/BlazorTemplater/actions/workflows/dotnet-core.yml/badge.svg)](https://github.com/conficient/BlazorTemplater/actions/workflows/dotnet-core.yml)

#### Examples
Using the library is simple:
```c#
var renderer = new BlazorTemplater();
var html = renderer.RenderComponent<MyComponent>();
```
This renders the `MyComponent` component as HTML.

**Parameters**

You can also set parameters on a component, e.g.
```c#
var renderer = new BlazorTemplater();
var myModel = new Model() { Value = "test" };
var parameters = new Dictionary<string, object>()
    {
        { nameof(MyComponent.Model), myModel }
    };
var html = renderer.RenderComponent<MyComponent>(parameters);
```
The dictionary is used to pass parameter values the component by name. Using `nameof()` 
instead of hard-coding a string is recommended to avoid code changes causing errors.

**Dependency Injection**

You can also use dependency injection:
```c#
var renderer = new BlazorTemplater();
renderer.AddService<ITestService>(new TestService());
var html = renderer.RenderComponent<MyComponent>();
```

## Getting Started

Add the `BlazorTemplater` NUGET package to your library.

### Supported Project Types

`BlazorTemplater` can be used in 
 - .NET Standard 2.0
 - .NET Standard 2.1
 - .NET Core 3.1
 - .NET 5 
 - .NET 6

Libraries or applications using `BlazorTemplator` need to have the **Razor SDK** enabled to provide compilation and intellisense for `.razor` files. If you have an existing .NET Standard class library that does not have Razor Component support, follow [this guide](Docs/AddRazorSupport) to upgrade the library. I did have issues retrofitting Razor support into the .NET Core 3.1 unit test app, so I moved the `.razor` classes into a .NET Standard library `BlazorTemplater.Library`. This should not be an issue for a Blazor WASM or Blazor Server application using .NET Core 3.1 since they already support.

### Usage

Create an instance of the `BlazorTemplater` class. This instance can be reused multiple times provided the services to be injected are the same.

To render a component which does not need any parameters set, use the `.RenderComponent<TComponent>()` method, where `TComponent` is the type generated for the Razor Component.

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
 - Injecting service sependencies via `.AddService<..>`
 - Nested Components
 
### Limitations

The following are not supported/tested:
   - EventCallbacks
   - Rerendering

## Credits and Acknowledgements

The basis of the rendering system is adapted from [Steve Sanderson's](https://github.com/SteveSandersonMS) [BlazorUnitTestingPrototype](https://github.com/SteveSandersonMS/BlazorUnitTestingPrototype) repo which was written as very simple component test library for Blazor.

This was never developed into a functioning product or library. For unit testing Razor Components I recommend [Egil Hansen's bunit](https://github.com/egil/bUnit).

### Version History

1.0.0   Inital Release (to Nuget)
