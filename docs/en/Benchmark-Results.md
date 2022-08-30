``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.22000
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
  DefaultJob : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT


```
|                            Method |      Mean |     Error |    StdDev | Code Size |  Gen 0 | Gen 1 | Gen 2 | Allocated | Completed Work Items | Lock Contentions |
|---------------------------------- |----------:|----------:|----------:|----------:|-------:|------:|------:|----------:|---------------------:|-----------------:|
|       EqualsSingleProperly_Lambda |  3.268 μs | 0.0635 μs | 0.0780 μs |      6 KB | 0.3319 |     - |     - |      2 KB |                    - |                - |
| EqualsSingleProperly_AutoFilterer |  4.135 μs | 0.0616 μs | 0.0546 μs |      0 KB | 0.2670 |     - |     - |      2 KB |                    - |                - |
|               StringFilter_Lambda | 23.582 μs | 0.2512 μs | 0.2350 μs |     20 KB | 2.6245 |     - |     - |     16 KB |                    - |                - |
|         StringFilter_AutoFilterer |  8.117 μs | 0.1265 μs | 0.1122 μs |      0 KB | 0.6561 |     - |     - |      4 KB |                    - |                - |
|                  MinAndMax_Lambda |  4.763 μs | 0.0723 μs | 0.0676 μs |     11 KB | 0.4654 |     - |     - |      3 KB |                    - |                - |
|            MinAndMax_AutoFilterer |  4.245 μs | 0.0349 μs | 0.0310 μs |      0 KB | 0.2823 |     - |     - |      2 KB |                    - |                - |
|              ComplexFilter_Lambda | 16.613 μs | 0.0699 μs | 0.0620 μs |     17 KB | 1.7090 |     - |     - |     11 KB |                    - |                - |
|        ComplexFilter_AutoFilterer | 13.625 μs | 0.1886 μs | 0.1764 μs |      0 KB | 0.7019 |     - |     - |      4 KB |                    - |                - |


> You can find benchmark test methods source codes from [here](https://github.com/enisn/AutoFilterer/blob/develop/tests/AutoFilterer.Benchmark/AutoFiltererStartup.cs).
