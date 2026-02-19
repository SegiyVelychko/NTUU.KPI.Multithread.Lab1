namespace NTUU.KPI.Multithread.Interfaces;

internal interface IFactorizationService
{
    IReadOnlyList<long> FactorizeParallel(long number, int threadCount);
    IReadOnlyList<long> FactorizeSequential(long number);
}
