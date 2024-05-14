namespace Graphs;

public class UnionFind<T> where T : notnull
{
    private readonly Dictionary<T, T> _parents = new();
    private readonly Dictionary<T, int> _rank = new();

    public UnionFind()
    {
    }

    public UnionFind(IEnumerable<T> elements)
    {
        foreach (var element in elements)
        {
            MakeSet(element);
        }
    }

    public void MakeSet(T element)
    {
        _parents[element] = element;
        _rank[element] = 0;
    }

    public T Find(T element)
    {
        if (!element.Equals(_parents[element]))
            _parents[element] = Find(_parents[element]);
        return _parents[element];
    }

    public void Unite(T element1, T element2)
    {
        var root1 = Find(element1);
        var root2 = Find(element2);

        if (_rank[root1] < _rank[root2])
            _parents[root1] = root2;
        else
        {
            _parents[root1] = root2;
            if (_rank[root1] == _rank[root2])
                _rank[root2]++;
        }
    }
    public bool IsConnected(T element1, T element2)
    {
        return Find(element1).Equals(Find(element2));
    }
}