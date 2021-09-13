# Templater 

The `Templater` class renders the component. It is superceded by `ComponentRenderer` but is used by this, and retained for backward compatibility. It also supports a non-generic `RenderComponent` method so is able to render from a `Type` provided that type implements `IComponent`.

```c#
var templater = new Templater();
var html = templater.RenderComponent<MyComponent>();
```
This renders the `MyComponent` component as HTML.

** Non-Generic method **

The `Templater` from v1.4.1 supports a non-generic `RenderComponent()` overload using the type of a component.

```c#
var componentType = typeof(MyComponent);
var templater = new Templater();
var html = templater.RenderComponent(componentType);
```

This feature was added as it enables the option to create static websites from a set of `RazorComponents` using reflection. See #15

**Parameters**

You can also set parameters on a component, e.g.
```c#
var templater = new Templater();
var myModel = new Model() { Value = "test" };
var parameters = new Dictionary<string, object>()
    {
        { nameof(MyComponent.Model), myModel }
    };
var html = templater.RenderComponent<MyComponent>(parameters);
```
The dictionary is used to pass parameter values the component by name. Using `nameof()` 
instead of hard-coding a string is recommended to avoid code changes causing errors.

**Dependency Injection**

You can also use dependency injection:
```c#
var templater = new Templater();
templater.AddService<ITestService>(new TestService());
var html = templater.RenderComponent<MyComponent>();
```

**Layouts** <small style="background-color: green; color:white; padding: 2px 4px">New!</small>

If a top-level component has a `@layout` attribute it will be applied when the component is rendered.

Alternatively you can apply a template explicitly:
```c#
var templater = new Templater();
templater..UseLayout<MyLayout>();
var html = templater.RenderComponent<MyComponent>();
```
You can also specify via a type:
```c#
void Example(Type layout)
{
    var templater = new Templater();
    templater..UseLayout(layout);
    var html = templater.RenderComponent<MyComponent>();
}
```
See [Layouts](Layouts) for more information.