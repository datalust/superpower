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
 StringSplitAndInt32Parse |   141.0917 us |   0.7050 us |   1.00 |      0.00 |
            SpracheSimple | 3,001.3214 us | 123.1638 us |  21.49 |      0.87 |
         SuperpowerSimple |   905.1306 us |   6.2815 us |   6.43 |      0.05 |
           SuperpowerChar |   708.5013 us |   7.7933 us |   5.05 |      0.06 |
          SuperpowerToken | 1,182.0515 us |   9.0299 us |   8.38 |      0.07 |
