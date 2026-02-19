using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using NTUU.KPI.Multithread.Interfaces;
using NTUU.KPI.Multithread.Services;

namespace NTUU.KPI.Multithread.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class MatrixTransposeBenchmarks
{
    private float[,] _matrix = null!;
    private readonly IMatrixTransposeService _service = new MatrixTransposeService();

    private static readonly int _rawSize = 10000;
    private static readonly int _columnSize = 10000;

    [Params(1, 2, 4, 8, 16, 32)]
    public int ThreadCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var rng = new Random(42);
        _matrix = new float[_rawSize, _columnSize];
        for (int i = 0; i < _rawSize; i++)
            for (int j = 0; j < _columnSize; j++)
                _matrix[i, j] = (float)rng.NextDouble();
    }

    [Benchmark(Baseline = true)]
    public float[,] Sequential()
        => _service.TransposeSequential(_matrix);

    [Benchmark]
    public float[,] ParallelBlocked()
        => _service.TransposeParallel(_matrix, ThreadCount, 64);
}
