# AutoFilterer Benchmark Results

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.400
  [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT


```
|                            Method |      Mean |     Error |    StdDev | Code Size |  Gen 0 | Completed Work Items | Lock Contentions | Allocated |
|---------------------------------- |----------:|----------:|----------:|----------:|-------:|---------------------:|-----------------:|----------:|
|       EqualsSingleProperly_Lambda |  3.458 μs | 0.0684 μs | 0.1413 μs |      6 KB | 0.3319 |                    - |                - |      2 KB |
| EqualsSingleProperly_AutoFilterer |  4.253 μs | 0.0837 μs | 0.1201 μs |      0 KB | 0.2670 |                    - |                - |      2 KB |
|               StringFilter_Lambda | 24.991 μs | 0.4590 μs | 0.3584 μs |     20 KB | 2.7161 |                    - |                - |     17 KB |
|         StringFilter_AutoFilterer | 15.094 μs | 0.2793 μs | 0.4964 μs |      0 KB | 1.2512 |                    - |                - |      8 KB |
|                  MinAndMax_Lambda |  5.022 μs | 0.0677 μs | 0.0600 μs |     12 KB | 0.4654 |                    - |                - |      3 KB |
|            MinAndMax_AutoFilterer |  4.667 μs | 0.0645 μs | 0.0603 μs |      0 KB | 0.2823 |                    - |                - |      2 KB |
|              ComplexFilter_Lambda | 17.529 μs | 0.2060 μs | 0.1826 μs |     17 KB | 1.7090 |                    - |                - |     11 KB |
|        ComplexFilter_AutoFilterer | 14.475 μs | 0.2793 μs | 0.2332 μs |      0 KB | 0.7019 |                    - |                - |      4 KB |


> You can find benchmark test methods source codes from [here](https://github.com/enisn/AutoFilterer/blob/develop/tests/AutoFilterer.Benchmark/AutoFiltererStartup.cs).
