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
 StringSplitAndInt32Parse |   142.5260 us |  0.4749 us |   1.00 |      0.00 |
            SpracheSimple | 2,942.5297 us | 13.0145 us |  20.64 |      0.11 |
         SuperpowerSimple |   976.2279 us |  8.5184 us |   6.85 |      0.06 |
           SuperpowerChar |   788.2328 us | 13.4030 us |   5.56 |      0.09 |
          SuperpowerToken |   920.3944 us |  6.8714 us |   6.46 |      0.05 |
