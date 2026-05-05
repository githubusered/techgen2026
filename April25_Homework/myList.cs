public class MyList
{
    private int[] _items;
    private int _count;

    public int Count => _count;

    public MyList()
    {
        _items = new int[4];
        _count = 0;
    }

    public void Add(int item)
    {
        EnsureCapacity();
        _items[_count++] = item;
    }

    public void AddRange(int[] items)
    {
        if (items == null) return;

        foreach (int item in items)
            Add(item);
    }

    public bool Remove(int item)
    {
        int index = IndexOf(item);
        if (index == -1) return false;

        for (int i = index; i < _count - 1; i++)
            _items[i] = _items[i + 1];

        _count--;
        return true;
    }

    public bool TryGet(int index, out int value)
    {
        if (index < 0 || index >= _count)
        {
            value = default;
            return false;
        }

        value = _items[index];
        return true;
    }

    private void EnsureCapacity()
    {
        if (_count < _items.Length) return;

        int[] newArr = new int[_items.Length * 2];

        for (int i = 0; i < _items.Length; i++)
            newArr[i] = _items[i];

        _items = newArr;
    }

    public int IndexOf(int item)
    {
        for (int i = 0; i < _count; i++)
            if (_items[i] == item)
                return i;

        return -1;
    }

    public bool Contains(int item) => IndexOf(item) != -1;

    public void Clear() => _count = 0;

    public int this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();

            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException();

            _items[index] = value;
        }
    }
}