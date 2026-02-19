using NTUU.KPI.Multithread.Interfaces;
using System.Collections.Concurrent;

namespace NTUU.KPI.Multithread.Services;

// https://www.geeksforgeeks.org/dsa/fermats-factorization-method-for-large-numbers/
internal sealed class FactorizationService : IFactorizationService
{
    public IReadOnlyList<long> FactorizeSequential(long number)
    {
        var factors = new List<long>();
        DecomposeSequential(number, factors);
        return [.. factors.Order()];
    }

    public IReadOnlyList<long> FactorizeParallel(long number, int threadCount)
    {
        var factors = new ConcurrentBag<long>();
        DecomposeParallel(number, factors, threadCount, currentDepth: 0);
        return [.. factors.Order()];
    }

    private static void DecomposeSequential(long number, List<long> factors)
    {
        if (IsPrime(number))
        {
            factors.Add(number);
            return;
        }

        var (a, b) = FermatSplit(number);
        DecomposeSequential(a, factors);
        DecomposeSequential(b, factors);
    }

    private static void DecomposeParallel(
        long number,
        ConcurrentBag<long> factors,
        int threadCount,
        int currentDepth)
    {
        if (IsPrime(number))
        {
            factors.Add(number);
            return;
        }

        var (a, b) = FermatSplit(number);

        int maxParallelDepth = (int)Math.Log2(Math.Max(1, threadCount));

        if (currentDepth < maxParallelDepth)
        {
            Parallel.Invoke(
                () => DecomposeParallel(a, factors, threadCount, currentDepth + 1),
                () => DecomposeParallel(b, factors, threadCount, currentDepth + 1));
        }
        else
        {
            DecomposeParallel(a, factors, threadCount, currentDepth + 1);
            DecomposeParallel(b, factors, threadCount, currentDepth + 1);
        }
    }

    private static (long, long) FermatSplit(long number)
    {
        if (number % 2 == 0)
            return (2, number / 2);

        long a = (long)Math.Ceiling(Math.Sqrt(number));

        if (a * a == number)
            return (a, a);

        // Захист від переповнення: якщо a > √(long.MaxValue) → ділимо наївно
        long safeLimit = (long)Math.Sqrt(long.MaxValue);

        while (a < safeLimit)
        {
            long b1 = a * a - number;
            long b = (long)Math.Sqrt(b1);

            if (b * b == b1)
                return (a - b, a + b);

            a++;
        }

        // Fallback: якщо Ферма не знайшов → Trial Division
        for (long i = 2; i * i <= number; i++)
            if (number % i == 0)
                return (i, number / i);

        return (1, number);
    }

    // https://en.wikipedia.org/wiki/Wheel_factorization
    // https://uk.wikipedia.org/wiki/%D0%9F%D1%80%D0%BE%D1%81%D1%82%D0%B5_%D1%87%D0%B8%D1%81%D0%BB%D0%BE#:~:text=%D0%9F%D1%80%D0%BE%D1%81%D1%82%D0%B5%20%D1%87%D0%B8%D1%81%D0%BB%D0%BE%20%E2%80%94%20%D1%86%D0%B5%20%D0%BD%D0%B0%D1%82%D1%83%D1%80%D0%B0%D0%BB%D1%8C%D0%BD%D0%B5%20%D1%87%D0%B8%D1%81%D0%BB%D0%BE,%D1%80%D0%BE%D0%B7%D0%B1%D0%B8%D0%B2%D0%B0%D1%8E%D1%82%D1%8C%20%D0%BD%D0%B0%20%D0%BF%D1%80%D0%BE%D1%81%D1%82%D1%96%20%D1%96%20%D1%81%D0%BA%D0%BB%D0%B0%D0%B4%D0%B5%D0%BD%D1%96.
    private static bool IsPrime(long number)
    {
        if (number <= 3)
            return number > 1;

        if (number % 2 == 0 || number % 3 == 0)
            return false;

        for (long i = 5; i * i <= number; i += 6)
        {
            if (number % i == 0 || number % (i + 2) == 0)
                return false;
        }

        return true;
    }
}
