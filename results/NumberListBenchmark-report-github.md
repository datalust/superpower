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
                   Method |        Median |      StdDev | Scaled | Scaled-SD |
------------------------- |-------------- |------------ |------- |---------- |
 StringSplitAndInt32Parse |   140.9253 us |   1.3282 us |   1.00 |      0.00 |
            SpracheSimple | 2,895.8916 us | 118.4367 us |  20.81 |      0.85 |
         SuperpowerSimple | 1,060.6123 us |   6.3693 us |   7.52 |      0.08 |
           SuperpowerChar |   872.3670 us |   1.5458 us |   6.18 |      0.06 |
          SuperpowerToken | 1,170.2532 us |  10.4641 us |   8.28 |      0.10 |
