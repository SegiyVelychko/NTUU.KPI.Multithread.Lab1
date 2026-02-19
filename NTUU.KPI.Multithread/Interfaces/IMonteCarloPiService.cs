namespace NTUU.KPI.Multithread.Interfaces;

internal interface IMonteCarloPiService
{
    double CalculateSequential(long iterations);
    double CalculateParallel(long iterations, int threadCount);
}
