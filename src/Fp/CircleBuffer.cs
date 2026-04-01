using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fp;

/// <summary>
/// Stores circular buffer of elements.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public class CircleBuffer<T> : IList<T>
{
    /// <summary>
    /// Maximum capacity.
    /// </summary>
    public int Capacity => _capacity;

    private readonly int _capacity;
    private readonly T[] _entries;
    private int _first;
    private int _count;

    internal int First => _first;

    /// <summary>
    /// Creates a new instance of <see cref="CircleBuffer{T}"/>.
    /// </summary>
    /// <param name="capacity">Buffer capacity.</param>
    public CircleBuffer(int capacity)
    {
        if (capacity < 0) throw new ArgumentException("Capacity cannot be less than 0", nameof(capacity));
        _capacity = capacity;
        _entries = new T[capacity];
        _first = 0;
    }

    /// <inheritdoc />
    public T this[int i]
    {
        get
        {
            RangeThrow(i, false);
            return _entries[Index(i)];
        }
        set
        {
            RangeThrow(i, false);
            _entries[Index(i)] = value;
        }
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        RangeThrow(index, true);
        ShrinkForRemove(index, 1);
    }

    /// <summary>
    /// Removes a range of values.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    public void RemoveRange(int index, int count)
    {
        RangeThrow(index, true);
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        ShrinkForRemove(index, count);
    }

    /// <inheritdoc />
    public void Insert(int index, T value)
    {
        RangeThrow(index, true);
        ExpandForInsert(index, 1);
        _entries[Index(index)] = value;
    }

    /// <summary>
    /// Inserts a range of values at the specified index.
    /// </summary>
    /// <param name="index">Index at which to insert new values.</param>
    /// <param name="values">Values to insert.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if the added values would exceed this container's capacity.</exception>
    public void InsertRange(int index, ReadOnlySpan<T> values)
    {
        RangeThrow(index, true);
        ExpandForInsert(index, values.Length);
        int remaining = values.Length;
        int writeIndex = Index(index);
        int firstSegmentLength = Math.Min(remaining, _capacity - writeIndex);
        values[..firstSegmentLength].CopyTo(_entries.AsSpan(writeIndex, firstSegmentLength));
        remaining -= firstSegmentLength;
        if (remaining > 0)
        {
            values[firstSegmentLength..].CopyTo(_entries);
        }
    }

    private void ShrinkForRemove(int index, int valueCount)
    {
        if ((uint)(index + valueCount) > _count)
        {
            throw new IndexOutOfRangeException($"Cannot remove {valueCount} items at index {index} with length {_count} and capacity {_capacity}");
        }
        if (index < _count / 2)
        {
            // Better to move elements below
            for (int j = index - 1; j >= 0; j--) _entries[Index(j + valueCount)] = _entries[Index(j)];
            for (int j = 0; j < valueCount; j++)
            {
                _entries[_first] = default!;
                // Move bottom up
                _first = (_first + 1) % _capacity;
            }
        }
        else
        {
            // Better to move elements above
            for (int j = index + valueCount; j < _count; j++) _entries[Index(j - valueCount)] = _entries[Index(j)];
            for (int i = 0; i < valueCount; i++)
            {
                _entries[Index(_count - 1 - i)] = default!;
            }
        }
        _count -= valueCount;
    }

    private void ExpandForInsert(int index, int valueCount)
    {
        if ((uint)(_count + valueCount) > _capacity)
        {
            throw new InvalidOperationException($"Cannot push {valueCount} items with length {_count} and capacity {_capacity}");
        }
        int oldCount = _count;
        _count += valueCount;
        if (index < oldCount / 2)
        {
            // Better to move elements below
            _first = (_capacity + _first - valueCount) % _capacity;
            for (int j = 0; j < index; j++) _entries[Index(j)] = _entries[Index(j + valueCount)];
        }
        else
        {
            // Better to move elements above
            for (int j = _count - 1; j >= index + valueCount; j--) _entries[Index(j)] = _entries[Index(j - valueCount)];
        }
    }

    private void RangeThrow(int i, bool insertionOrRemoval)
    {
        if (i < 0 || (insertionOrRemoval ? i > _count : i >= _count)) throw new IndexOutOfRangeException($"Invalid index {i} for list of length {_count}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Index(int i) => (_first + i) % _capacity;

    /// <inheritdoc />
    public int IndexOf(T item)
    {
        for (int i = 0; i < _count; i++)
            if (_entries[Index(i)]?.Equals(item) ?? item == null)
            {
                return i;
            }

        return -1;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
            yield return _entries[Index(i)];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Add(T item)
    {
        Insert(_count, item);
    }

    /// <summary>
    /// Adds all entries from specified collection.
    /// </summary>
    /// <param name="span">Source collection.</param>
    public void AddRange(ReadOnlySpan<T> span)
    {
        InsertRange(_count, span);
    }

    /// <inheritdoc />
    public void Clear()
    {
        for (int i = 0; i < _count; i++)
            _entries[Index(i)] = default!;
        _count = 0;
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
        for (int i = 0; i < _count; i++)
            if (_entries[Index(i)]?.Equals(item) ?? item == null)
            {
                return true;
            }

        return false;
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException();
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
        if (array.Length - arrayIndex < _count) throw new ArgumentException();
        int a = Math.Min(_count, _capacity - _first);
        Array.Copy(_entries, _first, array, arrayIndex, a);
        if (a != _count)
            Array.Copy(_entries, 0, array, arrayIndex + a, _count - a);
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        for (int i = 0; i < _count; i++)
            if (_entries[Index(i)]?.Equals(item) ?? item == null)
            {
                RemoveAt(i);
                return true;
            }

        return false;
    }

    /// <inheritdoc />
    public int Count => _count;

    /// <inheritdoc />
    public bool IsReadOnly => false;
}
