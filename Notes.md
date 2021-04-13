
# View vs Component Rendering

## Speed Tests

BenchmarkDotNet runs

### Summary .NET Core 3.1

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.867 (2004/?/20H1)
AMD Ryzen 7 1700, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=6.0.100-preview.3.21202.5
  [Host]     : .NET Core 3.1.13 (CoreCLR 4.700.21.11102, CoreFX 4.700.21.11602), X64 RyuJIT
  DefaultJob : .NET Core 3.1.13 (CoreCLR 4.700.21.11102, CoreFX 4.700.21.11602), X64 RyuJIT


|               Method |     Mean |     Error |    StdDev |   Median |      Min |       Max |   Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|--------------------- |---------:|----------:|----------:|---------:|---------:|----------:|--------:|-------:|-------:|----------:|
| Test_BlazorTemplater | 97.10 us | 19.071 us | 53.476 us | 68.95 us | 64.58 us | 260.29 us |  9.5215 | 3.7842 | 0.1221 |  57.54 KB |
| Test_RazorTemplating | 50.75 us |  0.866 us |  0.768 us | 50.95 us | 49.02 us |  51.91 us | 10.1318 |      - |      - |  41.65 KB |

### Summary NET 5.0
```
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.867 (2004/?/20H1)
AMD Ryzen 7 1700, 1 CPU, 16 logical and 8 physical cores
.NET Core SDK=6.0.100-preview.3.21202.5
  [Host]     : .NET Core 5.0.5 (CoreCLR 5.0.521.16609, CoreFX 5.0.521.16609), X64 RyuJIT
  DefaultJob : .NET Core 5.0.5 (CoreCLR 5.0.521.16609, CoreFX 5.0.521.16609), X64 RyuJIT
```

|               Method |     Mean |    Error |   StdDev |      Min |      Max |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
|--------------------- |---------:|---------:|---------:|---------:|---------:|--------:|-------:|------:|----------:|
| Test_BlazorTemplater | 42.72 us | 0.486 us | 0.431 us | 42.11 us | 43.58 us |  4.2725 | 1.0986 |     - |  26.54 KB |
| Test_RazorTemplating | 44.42 us | 0.573 us | 0.536 us | 43.66 us | 45.24 us | 10.2539 |      - |     - |  41.95 KB |


Library versions used:
 - BlazorTemplater: v1.1.1
 - RazorTemplating: 1.5

 - RazorLight: v2.0.0-rc.3
 - RazorEngine: TBC

## Key Points

 - Razor Components only require a .NET Standard 2.0 class library
 - Can be referenced in .NET Framework libraries ?

## To Test

 - [ ] Can .NET Std library work if referenced in a Framework library?


