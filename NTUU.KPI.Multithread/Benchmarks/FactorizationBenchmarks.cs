using BenchmarkDotNet.Attributes;
using NTUU.KPI.Multithread.Interfaces;
using NTUU.KPI.Multithread.Services;

namespace NTUU.KPI.Multithread.Benchmarks;

[MemoryDiagnoser]
public class FactorizationBenchmarks
{
    private readonly IFactorizationService _service = new FactorizationService();

    [Params(1, 2, 4, 8, 16)]
    public int ThreadCount { get; set; }

    [Params(
        105_327_569L,        // 10223 × 10303
        2_999_999_999_971L,  // 3 × 999_999_999_989
        1_000_000_000_039L)]
    public long Number { get; set; }

    [Benchmark(Baseline = true)]
    public IReadOnlyList<long> Sequential()
        => _service.FactorizeSequential(Number);

    [Benchmark]
    public IReadOnlyList<long> Parallel()
        => _service.FactorizeParallel(Number, ThreadCount);
}
