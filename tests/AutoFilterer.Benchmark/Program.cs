using BenchmarkDotNet.Running;

namespace AutoFilterer.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<AutoFiltererStartup>(args: args);
        }
    }
}
