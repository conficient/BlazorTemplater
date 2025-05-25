# Issolating Css

CSS Isolated, or CSS scope, it's and adventage to create components with with own CSS. 

Templater with add automatically the CSS scoped from the Layout and from the component is pretend to eb rendered. If you have child components must be add also the compoent type while adding the component.

CSS Isolated you can add like.

```
LayoutComponentWithCss.razor
LayoutComponentWithCss.razor.css
ComponentWithCss.razor
ComponentWithCss.razor.css
```

## Considerations

* CSS must be add to the assembly like a Embedded Resource
* Because component internal ID is not in the Embedded Resource recomendation is have a container wrapper to identify the component css when all is merged

```xaml
  <ItemGroup>
    <EmbeddedResource Include="LayoutComponentWithCss.razor.css" />
    <EmbeddedResource Include="ComponentWithCss.razor.css" />
  </ItemGroup>
```


### Usage

#### Normal Component with or without layout
When the component is rendered issolated css is automatically added between ```html <style></style>``` tags before render de component.

So you can still using as a normal:
```c#
string html = new ComponentRenderer<MyComponent>()
            .Render();
```

#### Component With Child component also with scoped CSS
If the component has also child or childs components with CSS isolated, then must to add this component type before rendering for the css can be added:
```c#
string html = new ComponentRenderer<MyComponent>()
            .AddScoppedCss<ComponentWithCss>()
            .AddScoppedCss<OtherComponentWithCss>()
            .Render();
```

