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
          Method |      Median |     StdDev | Scaled | Scaled-SD |
---------------- |------------ |----------- |------- |---------- |
         Sprache | 283.8618 us | 10.0276 us |   1.00 |      0.00 |
 SuperpowerToken |  81.1563 us |  2.8775 us |   0.29 |      0.01 |
