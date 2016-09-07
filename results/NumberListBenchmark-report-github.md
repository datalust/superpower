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
 StringSplitAndInt32Parse |   141.8226 us |   0.7297 us |   1.00 |      0.00 |
            SpracheSimple | 2,967.8178 us | 111.6906 us |  21.28 |      0.78 |
         SuperpowerSimple |   924.2786 us |   5.0936 us |   6.52 |      0.05 |
           SuperpowerChar |   711.4206 us |  11.5167 us |   5.04 |      0.08 |
          SuperpowerToken | 1,179.7802 us |  10.1893 us |   8.32 |      0.08 |
