using NTUU.KPI.Multithread.Interfaces;
using System.Text.RegularExpressions;

namespace NTUU.KPI.Multithread.Services;

internal sealed class WordCountService : IWordCountService
{
    private static readonly Regex WordRegex = new(@"\b\w+\b", RegexOptions.Compiled);

    public async Task<long> CountWordsParallelForEachAsync(string rootDirectory, int threadCount)
    {
        var files = Directory.GetFiles(rootDirectory, "*", SearchOption.AllDirectories);
        long total = 0;

        await Parallel.ForEachAsync(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = threadCount },
            async (file, ct) =>
            {
                var text = await File.ReadAllTextAsync(file, ct);

                long count = CountWords(text);
                Interlocked.Add(ref total, count);
            });

        return total;
    }

    public async Task<long> CountWordsSemaphoreSlimAsync(string rootDirectory, int threadCount)
    {
        var files = Directory.GetFiles(rootDirectory, "*", SearchOption.AllDirectories);
        var semaphore = new SemaphoreSlim(threadCount);
        long total = 0;

        var tasks = files.Select(async file =>
        {
            await semaphore.WaitAsync();
            try
            {
                var text = await File.ReadAllTextAsync(file);
                Interlocked.Add(ref total, CountWords(text));
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        return total;
    }

    public async Task<long> CountWordsSequentialAsync(string rootDirectory)
    {
        var files = Directory.GetFiles(rootDirectory, "*", SearchOption.AllDirectories);
        long total = 0;

        foreach (var file in files)
        {
            var text = await File.ReadAllTextAsync(file);
            total += CountWords(text);
        }

        return total;
    }

    public async Task<long> CountWordsWhenAllAsync(string rootDirectory)
    {
        var files = Directory.GetFiles(rootDirectory, "*", SearchOption.AllDirectories);

        var tasks = files.Select(async file =>
        {
            var text = await File.ReadAllTextAsync(file);
            return CountWords(text);
        });

        var results = await Task.WhenAll(tasks);

        return results.Sum();
    }

    private static long CountWords(string text)
        => WordRegex.Count(text);
}
