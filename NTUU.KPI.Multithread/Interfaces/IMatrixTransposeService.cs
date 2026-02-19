namespace NTUU.KPI.Multithread.Interfaces;

public interface IMatrixTransposeService
{
    float[,] TransposeSequential(float[,] matrix);

    float[,] TransposeParallel(float[,] matrix, int threadCount, int blockSize = 64);
}
