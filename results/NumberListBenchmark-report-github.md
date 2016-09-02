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
 StringSplitAndInt32Parse |   140.8612 us |  0.4380 us |   1.00 |      0.00 |
            SpracheSimple | 2,817.7144 us | 86.9364 us |  20.17 |      0.60 |
         SuperpowerSimple |   962.9979 us |  2.0514 us |   6.83 |      0.03 |
           SuperpowerChar |   783.5973 us | 10.6479 us |   5.56 |      0.08 |
          SuperpowerToken |   941.3789 us |  2.7814 us |   6.68 |      0.03 |
