``` ini
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19042.1083 (20H2/October2020Update)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100-preview.5.21302.13
  [Host]     : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT
  DefaultJob : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT


```

| Method                            |      Mean |     Error |    StdDev | Completed Work Items | Lock Contentions |  Gen 0 |  Gen 1 | Gen 2 | Allocated | Code Size |
| --------------------------------- | --------: | --------: | --------: | -------------------: | ---------------: | -----: | -----: | ----: | --------: | --------: |
| EqualsSingleProperly_Lambda       |  3.184 μs | 0.0153 μs | 0.0144 μs |               0.0000 |                - | 0.2861 |      - |     - |      2 KB |      3 KB |
| EqualsSingleProperly_AutoFilterer |  8.348 μs | 0.0576 μs | 0.0539 μs |               0.0000 |                - | 0.6409 |      - |     - |      4 KB |      0 KB |
| StringFilter_Lambda               | 26.384 μs | 0.1595 μs | 0.1414 μs |               0.0001 |                - | 2.4414 | 0.0305 |     - |     15 KB |     23 KB |
| StringFilter_AutoFilterer         | 14.163 μs | 0.1540 μs | 0.1440 μs |               0.0000 |                - | 1.0681 |      - |     - |      7 KB |      0 KB |
| MinAndMax_Lambda                  |  5.176 μs | 0.0661 μs | 0.0586 μs |               0.0000 |                - | 0.4501 |      - |     - |      3 KB |      7 KB |
| MinAndMax_AutoFilterer            |  8.756 μs | 0.0653 μs | 0.0579 μs |               0.0000 |                - | 0.6561 |      - |     - |      4 KB |      0 KB |
| ComplexFilter_Lambda              | 18.311 μs | 0.1326 μs | 0.1175 μs |               0.0001 |                - | 1.5259 |      - |     - |      9 KB |     14 KB |
| ComplexFilter_AutoFilterer        | 25.776 μs | 0.1496 μs | 0.1399 μs |               0.0001 |                - | 2.1057 |      - |     - |     13 KB |      0 KB |

- You can find benchmark test methods source codes from [here]([AutoFilterer/AutoFiltererStartup.cs at develop · enisn/AutoFilterer (github.com)](https://github.com/enisn/AutoFilterer/blob/develop/tests/AutoFilterer.Benchmark/AutoFiltererStartup.cs)). 