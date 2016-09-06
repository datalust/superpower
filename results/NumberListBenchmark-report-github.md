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
 StringSplitAndInt32Parse |   140.5250 us |   1.7288 us |   1.00 |      0.00 |
            SpracheSimple | 3,043.3244 us | 137.0805 us |  22.13 |      0.99 |
         SuperpowerSimple |   877.2944 us |  30.0207 us |   6.24 |      0.22 |
           SuperpowerChar |   668.7951 us |  14.0927 us |   4.74 |      0.11 |
          SuperpowerToken | 1,169.5392 us |  14.0349 us |   8.32 |      0.14 |
