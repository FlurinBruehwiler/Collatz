
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

//Time(() => ProgUnoptimized(10_000_000));
Time(() => ProgOptimized(90_000_000));
Time(() => ProgOptimized(90_000_000));
Time(() => ProgOptimized(90_000_000));
Time(() => ProgOptimized(90_000_000));

long ProgOptimized(int x)
{
    var dictCutoff = x;

    ushort[] numToLength = new ushort[(dictCutoff + 1) >> 1];
    Entry[] numbers = new Entry[1024];

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
            for (var index = numbersCount - 1; index >= 0; index--)
            {
                var i1 = numbers[index];

                lenOffset += i1.StepCost;

                StoreLenInCache(i1.Num, lenOffset, dictCutoff, numToLength);
            }

            if (lenOffset > maxLen)
            {
                maxI = i;
                maxLen = lenOffset;
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
        numToLength[(int)(k / 2)] = v;
    }
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
bool GetCachedLen(long f, out ushort l, int dictCutoff, ushort[] numToLength)
{
    l = 0;

    if (f < dictCutoff)
    {
        var a = numToLength[(int)(f / 2)];
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
ushort GetSeries(long num, out int numbersCount, Entry[] numbers, int dictCutoff, ushort[] numToLength)
{
    numbersCount = 0;
    do
    {
        if (num < dictCutoff)
        {
            var cachedLen = numToLength[(int)(num / 2)];
            if (cachedLen != 0)
            {
                return cachedLen;
            }
        }

        long next = 3 * num + 1;
        int shift = BitOperations.TrailingZeroCount(next);

        numbers[numbersCount++] = new Entry
        {
            Num = num,
            StepCost = (byte)(1 + shift)
        };

        num = next >> shift;
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
            num /= 2;
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
    GC.Collect();
    GC.Collect();
    GC.Collect();
}

struct Entry
{
    public long Num;
    public byte StepCost;
}
