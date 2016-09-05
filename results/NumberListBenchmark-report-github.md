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
 StringSplitAndInt32Parse |   139.1851 us |  3.3844 us |   1.00 |      0.00 |
            SpracheSimple | 2,946.0222 us | 13.3321 us |  20.96 |      0.47 |
         SuperpowerSimple |   847.5184 us |  5.0221 us |   6.03 |      0.14 |
           SuperpowerChar |   649.3021 us |  3.9925 us |   4.62 |      0.11 |
          SuperpowerToken | 1,206.7713 us | 36.9006 us |   8.61 |      0.32 |
