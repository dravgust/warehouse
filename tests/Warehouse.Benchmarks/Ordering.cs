using BenchmarkDotNet.Attributes;

namespace Warehouse.Benchmarks;

[MemoryDiagnoser(false)]
public class Ordering
{
    private static readonly Random R = new(80085);

    [Params(100)]
    public int Size { get; set; }

    [Benchmark]
    public int[] OrderBy()
    {
        var list = Enumerable.Range(1, Size).Select(r => R.Next(r));
        return list.OrderBy(x => x).ToArray();
    }

    [Benchmark]
    public int[] Sort()
    {
        var list = Enumerable.Range(1, Size).Select(r => R.Next(r)).ToArray();
        Array.Sort(list);
        return list;
    }

    [Benchmark]
    public int[] Sort_Span()
    {
        Span<int> list = Enumerable.Range(1, Size).Select(r => R.Next(r)).ToArray();
        list.Sort();
        return list.ToArray();
    }

    [Benchmark]
    public List<int> OrderByList()
    {
        var list = Enumerable.Range(1, Size).Select(r => R.Next(r));
        return list.OrderBy(x => x).ToList();
    }

    [Benchmark]
    public List<int> SortList()
    {
        var list = Enumerable.Range(1, Size).Select(r => R.Next(r)).ToList();
        list.Sort();
        return list;
    }
}