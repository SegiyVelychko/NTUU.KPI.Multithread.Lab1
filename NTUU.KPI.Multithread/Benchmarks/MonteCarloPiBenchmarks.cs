using BenchmarkDotNet.Attributes;
using NTUU.KPI.Multithread.Interfaces;
using NTUU.KPI.Multithread.Services;

namespace NTUU.KPI.Multithread.Benchmarks;

[MemoryDiagnoser]
public class MonteCarloPiBenchmarks
{
    private readonly IMonteCarloPiService _service = new MonteCarloPiService();

    [Params(1, 2, 4, 8, 16)]
    public int ThreadCount { get; set; }

    [Params(1_000_000L, 10_000_000L, 100_000_000L)]
    public long Iterations { get; set; }

    [Benchmark(Baseline = true)]
    public double Sequential()
        => _service.CalculateSequential(Iterations);

    [Benchmark]
    public double Parallel()
        => _service.CalculateParallel(Iterations, ThreadCount);
}
