using Base_MicroService;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Memory_MicroService
{
    public class MemoryMicroService:BaseMicroService<MemoryMicroService>
    {
        protected override void OnTick([NotNull]object sender, [NotNull]ElapsedEventArgs e)
        {
            Console.WriteLine(string.Intern("Reclaiming Memory"));
            ReclaimMemory();
        }

        /// <summary>
        /// Reclaim memory
        /// </summary>
        public static void ReclaimMemory()
        {
            long mem2 = GC.GetTotalMemory(false);
            Console.WriteLine(string.Intern("*** Memory ***"));
            Console.WriteLine(string.Intern("tMemory before GC: ") + ToBytes(mem2));
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            long mem3 = GC.GetTotalMemory(false);
            Console.WriteLine(string.Intern("tMemory after GC: ") + ToBytes(mem3));
            Console.WriteLine("tApp memory being used: " + ToBytes(Environment.WorkingSet));
            int gen1 = 0;
            int gen2 = 0;
            for (int x=0; x < GC.MaxGeneration; x++)
            {
                if (x == 0)
                    gen1 = GC.CollectionCount(x);
                else if (x == 1)
                    gen2 = GC.CollectionCount(x);
                Console.WriteLine("ttGeneration " + (x) + " Collection Count: " + GC.CollectionCount(x));
                const string category = ".NET CLR Memory";
                const string counter = "% Time in GC";
                string instance = Process.GetCurrentProcess().ProcessName;
                float percent = 0.0F;
                if (PerformanceCounterCategory.Exists(category) && PerformanceCounterCategory.CounterExists(counter,category)&&
                    PerformanceCounterCategory.InstanceExists(instance, category))
                {
                    var gcPerf = new PerformanceCounter(category, counter, instance);
                    percent = gcPerf.NextValue();
                    string suffix = "%";
                    if (percent > 50.0)
                    {
                        suffix += " <- High Watermark Warning";
                    }
                    Console.WriteLine("ttTime Spent in GC: " + $"{percent:00.##}" + suffix);
                }
                Subscribe();
            }

        }
    }
}
