```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Windows
Processor=?, ProcessorCount=8
Frequency=2533306 ticks, Resolution=394.7411 ns, Timer=TSC
CLR=CORE, Arch=64-bit ? [RyuJIT]
GC=Concurrent Workstation
dotnet cli version: 1.0.0-preview2-003121

Type=NumberListBenchmark  Mode=Throughput  

```
                   Method |        Median |     StdDev | Scaled | Scaled-SD |
------------------------- |-------------- |----------- |------- |---------- |
 StringSplitAndInt32Parse |   140.2091 us |  0.8511 us |   1.00 |      0.00 |
            SpracheSimple | 2,876.2271 us | 18.5988 us |  20.55 |      0.18 |
         SuperpowerSimple | 1,017.0258 us |  9.0141 us |   7.29 |      0.08 |
           SuperpowerChar |   840.0407 us | 10.9828 us |   6.01 |      0.08 |
          SuperpowerToken | 1,112.8692 us | 14.5743 us |   7.95 |      0.11 |
