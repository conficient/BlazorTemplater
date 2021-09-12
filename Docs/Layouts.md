# Layouts

Layouts are `.razor` components that inherit from `LayoutComponentBase` and contain a `@Body` tag that indicates where rendered content should be inserted.

In Blazor applications these usually render a common page structure (e.g. header, navbar, footer etc.) and inject the page in the `@Body` position.

In **BlazorTemplater** layouts are very useful for creating a HTML email template that contains the common structure you want your component to render into. You can also apply different layouts based on some criteria.

### Creating Layouts

Layout components should inherit from `LayoutComponentBase` and have a `@Body` statement where the rendered content should be placed:
```html
@inherits LayoutComponentBase
<html>
<head>
    <title>Layout Example</title>
</head>
<body>
    <div class="content">
        @Body
    </div>
</body>
</html>
```
In normal Blazor applications the Layout would not have the outer `<html>` elements (this would be in `_Host.cshtml` or `index.html`). These are not needed in BlazorTemplater so your layout can contain this.

If you're using BlazorTemplater to generate email content, you might want to use a HTML email template designed for this purpose, such as those at https://github.com/leemunroe/responsive-html-email-template.

### Usage

#### Layout Attribute
A component can specify a `LayoutAttribute` in the markup, e.g. in `MyComponent.razor`
```c#
@layout MyLayout

<p>Hello World</p>
```
When the component is rendered the template is applied automatically:
```c#
string html = new ComponentRenderer<MyComponent>()
            .Render();
```

### UseLayout&lt;T&gt;()
You can also set (or override) a layout using `.UseLayout<T>()`
```c#
string html = new ComponentRenderer<MyComponent>()
            .UseLayout<MyLayout>()
            .Render();
```
This is a type-safe approach since the compiler can validate that `MyLayout` inherits from `LayoutComponentBase`.

### UseLayout(type)
You can also set (or override) a layout using `.UseLayout(myLayoutType)`
```c#
var layout = typeof(MyLayout);
string html = new ComponentRenderer<MyComponent>()
            .UseLayout(layout)
            .Render();
```
This can result in runtime errors if the type used isn't based on `LayoutComponentBase`. It does however allow you to change layouts at runtime. 

#### Dynamic Layout 
For example, let's say you need to brand emails using different different layouts (logos, colour schemes etc). You might have something like this:
```c#

Type GetLayoutForBrand(string brand)
{
    switch (brand)
    {
    case "CocaCola":
        return typeof(CokeLayout);

    case "Pepsi":
        return typeof(PepsiLayout);

    default:
        return typeof(DefaultLayout);
    }
}

string RenderWelcomeEmail(string brand)
{
    var brandLayout = GetLayoutForBrand(brand);
    var html = new ComponentRenderer<WelcomeEmail>
        .UseLayout(brandLayout)
        .Render();
    return html;
}
```
