
using System.Diagnostics;
using System.Runtime.CompilerServices;

//Time(() => ProgUnoptimized(10_000_000));
Time(() => ProgOptimized(90_000_000));
Time(() => ProgOptimized(90_000_000));
Time(() => ProgOptimized(90_000_000));

long ProgOptimized(int x)
{
    var dictCutoff = x;

    ushort[] numToLength = new ushort[dictCutoff];
    long[] numbers = new long[1024];

    long maxI = 1;
    long maxLen = 0;

    for (int i = 1; i < x; i += 2)
    {
        if (GetCachedLen(i, out var len, dictCutoff, numToLength))
        {
            if (len > maxLen)
            {
                maxI = i;
                maxLen = len;
            }
        }
        else
        {
            var lenOffset = GetSeries(i, out var numbersCount, numbers, dictCutoff, numToLength);
            for (var index = 0; index < numbersCount; index++)
            {
                var i1 = numbers[index];
                StoreLenInCache(i1, (ushort)(lenOffset + (numbersCount - index)), dictCutoff, numToLength);
            }

            var newLen = lenOffset + numbersCount;
            if (newLen > maxLen)
            {
                maxI = i;
                maxLen = newLen;
            }
        }
    }

    Console.WriteLine($"x: {x}, found: {maxI} (length: {maxLen})");
    return maxI;
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
void StoreLenInCache(long k, ushort v, int dictCutoff, ushort[] numToLength)
{
    if (k < dictCutoff)
    {
        numToLength[k] = v;
    }
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
bool GetCachedLen(long f, out ushort l, int dictCutoff, ushort[] numToLength)
{
    l = 0;

    if (f < dictCutoff)
    {
        var a = numToLength[(int)f];
        if (a == 0)
        {
            return false;
        }

        l = a;
        return true;
    }

    return false;
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
ushort GetSeries(long num, out int numbersCount, long[] numbers, int dictCutoff, ushort[] numToLength)
{
    numbersCount = 0;
    do
    {
        if (GetCachedLen(num, out var l, dictCutoff, numToLength))
        {
            return l;
        }

        numbers[numbersCount++] = num;

        if (num % 2 == 0)
        {
            num = num / 2;
        }
        else
        {
            num = 3 * num + 1;
        }
    } while (num > 1);

    return 0;
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