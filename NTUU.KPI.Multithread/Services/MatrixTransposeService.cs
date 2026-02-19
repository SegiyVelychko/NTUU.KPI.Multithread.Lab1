using NTUU.KPI.Multithread.Interfaces;

namespace NTUU.KPI.Multithread.Services;

internal sealed class MatrixTransposeService : IMatrixTransposeService
{
    public float[,] TransposeSequential(float[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        var result = new float[cols, rows];

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result[j, i] = matrix[i, j];

        return result;
    }

    public float[,] TransposeParallel(float[,] matrix, int threadCount, int blockSize = 64)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        var result = new float[cols, rows];

        int blocksPerRow = (rows + blockSize - 1) / blockSize;
        int blocksPerCol = (cols + blockSize - 1) / blockSize;
        int totalBlocks = blocksPerRow * blocksPerCol;

        var options = new ParallelOptions { MaxDegreeOfParallelism = threadCount };

        Parallel.For(0, totalBlocks, options, blockIndex =>
        {
            int blockRaw = blockIndex / blocksPerCol;
            int blockColumn = blockIndex % blocksPerCol;

            int rowStart = blockRaw * blockSize;
            int colStart = blockColumn * blockSize;
            int rowEnd = Math.Min(rowStart + blockSize, rows);
            int colEnd = Math.Min(colStart + blockSize, cols);

            for (int i = rowStart; i < rowEnd; i++)
                for (int j = colStart; j < colEnd; j++)
                    result[j, i] = matrix[i, j];
        });

        return result;
    }
}
