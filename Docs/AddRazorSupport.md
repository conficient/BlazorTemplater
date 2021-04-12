# Adding Razor Component Support

If you have a .NET Standard class library you can add support for Razor Components to these by following these steps:

#### .NET Standard 2.0
##### 1. SDK
Change the default project SDK from 
```
<Project Sdk="Microsoft.NET.Sdk">
```
to the Razor SDL:
```
<Project Sdk="Microsoft.NET.Sdk.Razor">
```
##### 2. RazorLangVersion
Add the `<RazorLangVersion>` tag to the `<PropertyGroup>`, e.g.:
```
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <RazorLangVersion>3.0</RazorLangVersion>
  </PropertyGroup>
```
##### 3. Add Packages
Include these two NUGET packages in the package references
```
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components" Version="3.1.13" />
    <PackageReference Include="Microsoft.AspNetCore.Components.Web" Version="3.1.13" />
  </ItemGroup>
```
##### 4. Add an `_Imports.razor` file
Create a file called `_Imports.razor` in the root of the project, containing this:
```
@using Microsoft.AspNetCore.Components.Web
```
This file is used by the Razor tooling to import default namespaces into components, so common namespaces such as the one above are automatically in scope. It's not always required but useful to avoid needing to add `@using ..` statements to components repeatedly.

These steps replicate the changes made for Razor Class Libraries.

## Other Project Types

Most applications using .NET Core 3.1 or higher, based on `<Project Sdk="Microsoft.NET.Sdk">` can be updated to support Razor Components. I created a test Console application `BlazorTemplater.ConsoleApp` and applied the steps above, and it can render the `Sample.razor` component as HTML.

Blazor Server and Blazor WASM applications do not need these changes as they use Razor Components by default.