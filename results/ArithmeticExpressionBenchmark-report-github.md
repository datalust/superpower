```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Windows
Processor=?, ProcessorCount=8
Frequency=2533306 ticks, Resolution=394.7411 ns, Timer=TSC
CLR=CORE, Arch=64-bit ? [RyuJIT]
GC=Concurrent Workstation
dotnet cli version: 1.0.0-preview2-003121

Type=ArithmeticExpressionBenchmark  Mode=Throughput  

```
          Method |      Median |    StdDev | Scaled | Scaled-SD |
---------------- |------------ |---------- |------- |---------- |
         Sprache | 256.6565 us | 3.3398 us |   1.00 |      0.00 |
 SuperpowerToken |  79.7273 us | 1.3012 us |   0.31 |      0.01 |
