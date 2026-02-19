using BenchmarkDotNet.Attributes;
using NTUU.KPI.Multithread.Interfaces;
using NTUU.KPI.Multithread.Services;

namespace NTUU.KPI.Multithread.Benchmarks;

[MemoryDiagnoser]
public class WordCountBenchmarks()
{
    private readonly IFilesCreator _filesCreator = new TestFilesCreator();
    private readonly IWordCountService _wordCountService = new WordCountService();
    private static readonly string InitialPath = Directory.GetCurrentDirectory();

    private static string _rootPath = null!;

    [Params(1, 2, 4, 8, 16, 32)]
    public int ThreadCount { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        _rootPath = await _filesCreator.GenerateAsync(InitialPath, fileCount: 1000);
    }

    [Benchmark(Baseline = true)]
    public async Task<long> SequentialAsync()
        => await _wordCountService.CountWordsSequentialAsync(_rootPath);

    [Benchmark]
    public async Task<long> ParallelSemaphoreAsync()
        => await _wordCountService.CountWordsSemaphoreSlimAsync(_rootPath, ThreadCount);

    [Benchmark]
    public async Task<long> ParallelForEachAsync()
        => await _wordCountService.CountWordsParallelForEachAsync(_rootPath, ThreadCount);

    [GlobalCleanup]
    public async Task Cleanup()
    {
        await _filesCreator.CleanupAsync(_rootPath);
    }
}
