using System;
using System.IO;

namespace Com.GitHub.ThreePrime.BaseOptimizer {
    static class Program {
        static readonly string[] CSV = new [] {
            "Class,Base,Next Prime,Memory Use (MiB/Gdigit),Memory Waste (MiB/Gdigit),Calculation Speed (MIPS)",
            "{0},{1},{2},{3:N2},{4:N2},{5:N2}",
            ""
        };
        static readonly string[] TABLE = new [] {
            @"+--------+--------------------+------------+-------------------------+---------------------------+--------------------------+
| Class  | Base               | Next Prime | Memory Use (MiB/Gdigit) | Memory Waste (MiB/Gdigit) | Calculation Speed (MIPS) |
+--------+--------------------+------------+-------------------------+---------------------------+--------------------------+",
            "| {0,6} | {1,18} | {2,10} | {3,23:N2} | {4,25:N2} | {5,24:N2} |",
            "+--------+--------------------+------------+-------------------------+---------------------------+--------------------------+"
        };
        static string[] Format = TABLE;

        static bool IsPrime(ulong num) {
            for (ulong i = 2; i < num; ++i) {
                if (num % i == 0) {
                    return false;
                }
            }
            return true;
        }

        static void Test<TNum>(string name, Func<TNum, TNum, TNum> add, TNum max, ulong size, int byteSize) where TNum : IConvertible {
            ulong prime = 1;
            ulong baseNum = 1;
            ulong maxUL = max.ToUInt64(null);
            while (baseNum < maxUL) {
                while (!IsPrime(++prime));
                if (maxUL / prime > baseNum) {
                    baseNum *= prime;
                } else {
                    break;
                }
            }
            double memoryUse = 1000000000.0 / Math.Log10(baseNum) * ((double) byteSize) / 1024.0 / 1024.0;
            double minimumMemoryUse = 1000000000.0 / Math.Log10(256) / 1024.0 / 1024.0;
            TNum num = max;
            DateTime start = DateTime.Now;
            for (int i = 0; i < 1000000; ++i) {
                num = add(num, max);
            }
            DateTime end = DateTime.Now;
            double mips = 1.0 / (end - start).TotalSeconds;
            Console.WriteLine(Format[1], name, baseNum, prime, memoryUse, memoryUse - minimumMemoryUse, mips);
        }

        static void Main(string[] args) {
            if (args.Length > 0) {
                switch (args[0]) {
                    case "csv":
                        Format = CSV;
                        break;
                    case "table":
                        Format = TABLE;
                        break;
                }
            }
            Console.WriteLine(Format[0]);
            TextWriter stdout = Console.Out;
            using (MemoryStream fakeStream = new MemoryStream()) {
                using (TextWriter writer = new StreamWriter(fakeStream)) {
                    Console.SetOut(writer);
                    Test("int", (a, b) => a + b, int.MaxValue, uint.MaxValue, 4);
                    Console.SetOut(stdout);
                }
            }
            Test("sbyte", (a, b) => (sbyte) (a + b), sbyte.MaxValue, byte.MaxValue, 1);
            Test("byte", (a, b) => (byte) (a + b), byte.MaxValue, byte.MaxValue, 1);
            Test("short", (a, b) => (short) (a + b), short.MaxValue, ushort.MaxValue, 2);
            Test("ushort", (a, b) => (ushort) (a + b), ushort.MaxValue, ushort.MaxValue, 2);
            Test("int", (a, b) => a + b, int.MaxValue, uint.MaxValue, 4);
            Test("uint", (a, b) => a + b, uint.MaxValue, uint.MaxValue, 4);
            Test("long", (a, b) => a + b, long.MaxValue, ulong.MaxValue, 8);
            Test("ulong", (a, b) => a + b, ulong.MaxValue, ulong.MaxValue, 8);
            Console.WriteLine(Format[2]);
        }
    }
}
