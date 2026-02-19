using BenchmarkDotNet.Attributes;
using NTUU.KPI.Multithread.Interfaces;
using NTUU.KPI.Multithread.Services;

namespace NTUU.KPI.Multithread.Benchmarks;

[MemoryDiagnoser]
public class PrimeCalculatorBenchmarks
{
    private readonly IPrimeCalculatorService _primeCalculatorService = new PrimeCalculatorService();

    [Params(1, 2, 4, 8, 16, 32)]
    public int ThreadCount { get; set; }

    private const long From = 1;
    private const long To = 2_000_000;

    [Benchmark(Baseline = true)]
    public IReadOnlyList<long> Sequential()
        => _primeCalculatorService.CalculateSequential(From, To);

    [Benchmark]
    public IReadOnlyList<long> ParallelFor()
        => _primeCalculatorService.CalculateParallel(From, To, ThreadCount);

    [Benchmark]
    public IReadOnlyList<long> ParallelLinq()
        => _primeCalculatorService.CalculateParallelLinq(From, To, ThreadCount);
}
