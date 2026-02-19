namespace NTUU.KPI.Multithread.Interfaces;

internal interface IWordCountService
{
    Task<long> CountWordsSequentialAsync(string rootDirectory);
    Task<long> CountWordsSemaphoreSlimAsync(string rootDirectory, int threadCount);
    Task<long> CountWordsParallelForEachAsync(string rootDirectory, int threadCount);

}
