using NTUU.KPI.Multithread.Interfaces;

namespace NTUU.KPI.Multithread.Services;

internal sealed class MonteCarloPiService : IMonteCarloPiService
{
    public double CalculateSequential(long iterations)
    {
        var random = new Random(42);

        long insideCircle = 0;

        for (long i = 0; i < iterations; i++)
        {
            var x = random.NextDouble();
            var y = random.NextDouble();

            if (x * x + y * y <= 1.0)
                insideCircle++;
        }

        return 4.0 * insideCircle / iterations;
    }

    public double CalculateParallel(long iterations, int threadCount)
    {
        long totalInside = 0;
        var chunkSize = iterations / threadCount;

        var options = new ParallelOptions { MaxDegreeOfParallelism = threadCount };

        Parallel.For(0, threadCount, options, t =>
        {
            var random = new Random(t);

            long localInside = 0;
            var start = t * chunkSize;
            var end = t == threadCount - 1 ? iterations : start + chunkSize;

            for (long i = start; i < end; i++)
            {
                var x = random.NextDouble();
                var y = random.NextDouble();

                if (x * x + y * y <= 1.0)
                    localInside++;
            }

            Interlocked.Add(ref totalInside, localInside);
        });

        return 4.0 * totalInside / iterations;
    }
}
