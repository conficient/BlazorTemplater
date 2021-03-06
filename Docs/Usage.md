# Usage

### Creating Templates

Create templates using `.razor` components in your code, e.g. `MyComponent.razor`. Razor Components compile into classes so they can be referenced using the class name (normally the same as filename). Refer to [the official docs](https://docs.microsoft.com/en-us/aspnet/core/blazor/components) if you need more guidance.

They default to the namespace of the folder they are in, but you can override this with the [`@namespace` directive](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/?view=aspnetcore-5.0#namespaces).

### Fluent `ComponentRenderer`

Create an instance of the `ComponentRenderer<T>` class, where `T` is the component type.

Then call either `.AddService()` to add services for dependency injection, or `.Set()` to set parameters on the component. You can call these multiple times for different services and parameters.

The final call is `.Render()` which renders the component as a HTML string.

#### Simple Render
To render a component which does not need any parameters set, you use the following code:
```c#
var html = new ComponentRenderer<MyComponent>().Render();
```
#### Setting Component Parameters

Parameters are set using the `.Set()` method and a lambda function using the component to indicate the name:
```c#
var html = new ComponentRenderer<MyComponent>()
            .Set(c => c.Model, myModel)
            .Set(c => c.Title, "My title")
            .Render();
```
#### Injecting Service Dependencies

You can use [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection) in Razor Components with the `@inject` directive. `ComponentRenderer` has a service container inside to provide these to the component, and you add these with `AddService()`

```c#
var html = new ComponentRenderer<MyComponent>()
            .AddService<IServiceType>(serviceInstance)
            .AddService(anotherService)
            .Render();
```
BlazorTemplater supports two overloads to `AddService`, as shown above. The first uses a contract type (`IServiceType`) plus an instance which is a different class that implements the contract; `serviceInstance` in the example. The second method injects a service value and the contract is inferred as the same as the value.

In ASP.NET Core it's possible to use DI to chain dependencies via constructor injection. That isn't supported here and you need to instantiate your services manually.

#### Errors

Razor Components are classes that execute code, so if there is an error in your component, `Render()` will throw an exception. A common error is not setting parameters resulting in a `NullReferenceException`.

### `Templater` Class

The initial versions of BlazorTemplater used the `Templater` class. I've retained the instructions for that here for reference. It's used by `ComponentRenderer` so it's essentially the same, but doesn't have the fluent interface.

#### `Templater`
Create an instance of the `Templater` class. This is a rendering host that also acts as a service container. This instance can be reused multiple times provided the services to be injected are the same.

Use the `.RenderComponent<Type>(..)` method to generate the HTML.

#### Simple Render
To render a component which does not need any parameters set, use the `.RenderComponent<TComponent>()` method, where `TComponent` is the type generated for the Razor Component.
```c#
var templater = new Templater();
var html = templater.RenderComponent<MyComponent>();
```
#### Setting Component Parameters

Parameters are passed to the `Renderer` as an `IDictionary<string, object>`. These are passed when you call `RenderComponent`:
```c#
var parameters = new Dictionary<string, object>()
    {
        { nameof(MyComponent.Model), myModel }
    };
var html = templater.RenderComponent<MyComponent>(parameters);
```
You can use the string name of a parameter if you wish:
```
var parameters = new Dictionary<string, object>()
    {
        { "Model", myModel }
    };
```
although this isn't recommended as the code will be invalid if the parameter in the component is renamed. Using `nameof(Component.ParameterName)` will ensure that a rename will update your rendering code.

#### Injecting Service Dependencies

You can use [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection) in Razor Components. The `Templater` class acts as a dependency injection (DI) service provider in this respect. To register services, use the `.AddService()` methods:

```c#
var templater = new Templater();
templater.AddService<ITestService>(new TestService());
```

You can inject services in your Razor Components as you would in a UI component, using the `@inject [type] [variable]` statement in the component, e.g.
```c#
@inject ITestService testService
```
The service can then be referenced in the component as required:
```c#
@testService.Add(A,B)
```

In ASP.NET Core it's possible to use DI to chain dependencies via constructor injection. That isn't supported here and you need to instantiate your services manually.

#### Errors

Razor Components are classes that execute code, so if there is an error in your component, `RenderComponent` will throw an exception. A common error is not setting parameters resulting in a `NullReferenceException`.

