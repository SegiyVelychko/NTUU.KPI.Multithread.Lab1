namespace NTUU.KPI.Multithread.Interfaces;

internal interface IPrimeCalculatorService
{
    IReadOnlyList<long> CalculateSequential(long from, long to);
    IReadOnlyList<long> CalculateParallel(long from, long to, int threadCount);
    IReadOnlyList<long> CalculateParallelLinq(long from, long to, int threadCount);
}
