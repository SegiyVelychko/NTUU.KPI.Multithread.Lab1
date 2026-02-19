using NTUU.KPI.Multithread.Interfaces;
using System.Collections.Concurrent;

namespace NTUU.KPI.Multithread.Services;

internal sealed class PrimeCalculatorService : IPrimeCalculatorService
{
    public IReadOnlyList<long> CalculateSequential(long from, long to)
    {
        var primes = new List<long>();

        for (long number = from; number <= to; number++)
            if (IsPrime(number))
                primes.Add(number);

        return primes;
    }

    public IReadOnlyList<long> CalculateParallel(long from, long to, int threadCount)
    {
        var primes = new ConcurrentBag<long>();
        var options = new ParallelOptions { MaxDegreeOfParallelism = threadCount };

        Parallel.For(from, to + 1, options, number =>
        {
            if (IsPrime(number))
                primes.Add(number);
        });

        return [.. primes.Order()];
    }

    public IReadOnlyList<long> CalculateParallelLinq(long from, long to, int threadCount)
    {
        return [.. ParallelEnumerable
            .Range(0, (int)(to - from + 1))
            .WithDegreeOfParallelism(threadCount)
            .Select(i => from + i)
            .Where(IsPrime)
            .Order()];
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
