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
 StringSplitAndInt32Parse |   138.7217 us |  5.7115 us |   1.00 |      0.00 |
            SpracheSimple | 2,740.3251 us | 11.9340 us |  19.40 |      0.72 |
         SuperpowerSimple |   937.1593 us | 43.9867 us |   6.64 |      0.39 |
           SuperpowerChar |   691.5509 us |  9.9246 us |   4.89 |      0.19 |
          SuperpowerToken | 1,189.8622 us | 15.9538 us |   8.46 |      0.33 |
