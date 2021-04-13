
# View vs Component Rendering

## Speed Tests

Single run (no caching for Razor engines)

| Library          |  Time |
|------------------|------:|
| BlazorTemplater  |  96ms |
| RazorLight       |   xxx |
| RazorEngine      |   xxx |
| RazorTemplating  |   xxx |

Repeated runs (100 iterations). Caching is used if supported.

| Library          |   Time |
|------------------|-------:|
| BlazorTemplater  | 1234ms |
| RazorLight       |    xxx |
| RazorEngine      |    xxx |
| RazorTemplating  |    xxx |

Library versions used:
 - BlazorTemplater: v1.1.1
 - RazorLight: v2.0.0-rc.3
 - RazorEngine: TBC
 - RazorTemplating: TBC

## Key Points

 - Razor Components only require a .NET Standard 2.0 class library
 - Can be referenced in .NET Framework libraries ?

## To Test

 - [ ] Can .NET Std library work if referenced in a Framework library?


