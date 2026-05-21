
using System.Diagnostics;
using System.Runtime.CompilerServices;

Time(() => ProgUnoptimized(10_000_000));
Time(() => ProgOptimized(90_000_000));

long ProgOptimized(int x)
{
    var dictCutoff = x * 5;

    int[] numToLength = new int[dictCutoff];
    Dictionary<long, int> numToLengthDict = [];
    List<long> numbers = [];

    long maxI = 1;
    long maxLen = 0;

    for (int i = 1; i < x; i++)
    {
        if (GetCachedLen(i, out var len))
        {
            if (len > maxLen)
            {
                maxI = i;
                maxLen = len;
            }
        }
        else
        {
            var (series, lenOffset) = GetSeries(i);
            for (var index = 0; index < series.Count; index++)
            {
                var i1 = series[index];
                StoreLenInCache(i1, lenOffset + (series.Count - index));
            }

            var newLen = lenOffset + series.Count;
            if (newLen > maxLen)
            {
                maxI = i;
                maxLen = newLen;
            }
        }
    }

    Console.WriteLine($"x: {x}, found: {maxI} (length: {numToLength[maxI]})");
    return maxI;

    void StoreLenInCache(long k, int v)
    {
        if (k < dictCutoff)
        {
            numToLength[k] = v;
        }
        else
        {
            numToLengthDict.Add(k, v);
        }
    }

    bool GetCachedLen(long f, out int l)
    {
        l = 0;

        if (f < dictCutoff)
        {
            var a = numToLength[(int)f];
            if (a == 0)
            {
                return false;
            }
            else
            {
                l = a;
                return true;
            }
        }

        return numToLengthDict.TryGetValue(f, out l);
    }

    (List<long>, int lenOffset) GetSeries(long num)
    {
        numbers.Clear();
        do
        {
            if (GetCachedLen(num, out var l))
            {
                return (numbers, l);
            }

            numbers.Add(num);

            if (num % 2 == 0)
            {
                num = num / 2;
            }
            else
            {
                num = 3 * num + 1;
            }
        } while (num > 1);

        return (numbers, 0);
    }
}

long ProgUnoptimized(long x)
{
    long maxLen = 0;
    long maxNum = 0;

    for (long i = 1; i <= x; i++)
    {
        long len = GetSeriesLength(i);
        if (len > maxLen)
        {
            maxLen = len;
            maxNum = i;
        }
    }

    Console.WriteLine($"x: {x}, found: {maxNum} (length: {maxLen})");
    return maxNum;
}

long GetSeriesLength(long num)
{
    long counter = 0;

    while (num > 1)
    {
        counter++;

        if (num % 2 == 0)
        {
            num = num / 2;
        }
        else
        {
            num = 3 * num + 1;
        }
    }

    return counter;
}

void Time(Action callback, [CallerArgumentExpression("callback")] string expression = null)
{
    var start = Stopwatch.GetTimestamp();
    callback();
    var time = Stopwatch.GetElapsedTime(start);
    Console.WriteLine($"{expression} Took {time.TotalMilliseconds}ms");
}