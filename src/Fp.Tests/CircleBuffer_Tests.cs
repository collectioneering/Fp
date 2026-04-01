using System;
using System.Collections.Generic;
using System.Linq;

namespace Fp.Tests;

public class CircleBuffer_Tests
{
    [Fact]
    public void Constructor_CapacityZero_Allowed()
    {
        _ = new CircleBuffer<byte>(0);
    }

    [Fact]
    public void Constructor_CapacityUnderZero_Disallowed()
    {
        Assert.Throws<ArgumentException>(() => new CircleBuffer<byte>(-1));
    }

    [Fact]
    public void Capacity_ReportsCorrectly()
    {
        CircleBuffer<byte> cb = new(10);
        Assert.Equal(10, cb.Capacity);
    }

    [Fact]
    public void Add_UnderCapacity_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(50);
        Assert.Equal(50, cb[0]);
    }

    [Fact]
    public void Add_AtCapacity_Throws()
    {
        // ReSharper disable CollectionNeverQueried.Local
        CircleBuffer<byte> cb = new(10);
        // ReSharper restore CollectionNeverQueried.Local
        for (int i = 0; i < 10; i++) cb.Add(50);
        Assert.Throws<InvalidOperationException>(() => cb.Add(50));
    }

    [Fact]
    public void Insert_FromEmpty_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Insert(0, 50);
        Assert.Equal(50, cb[0]);
    }

    [Fact]
    public void Insert_FromStart_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(10);
        cb.Add(20);
        cb.Insert(0, 50);
        Assert.Equal(new byte[] { 50, 10, 20 }, cb.ToArray());
    }

    [Fact]
    public void Insert_FromMiddle_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(10);
        cb.Add(20);
        cb.Insert(1, 50);
        Assert.Equal(new byte[] { 10, 50, 20 }, cb.ToArray());
    }

    [Fact]
    public void Insert_FromEnd_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(10);
        cb.Add(20);
        cb.Insert(2, 50);
        Assert.Equal(new byte[] { 10, 20, 50 }, cb.ToArray());
    }

    [Fact]
    public void Remove_Empty_False()
    {
        // ReSharper disable CollectionNeverUpdated.Local
        CircleBuffer<byte> cb = new(10);
        // ReSharper restore CollectionNeverUpdated.Local
        Assert.False(cb.Remove(20));
        Assert.Empty(cb);
    }

    [Fact]
    public void Remove_Missing_False()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        Assert.False(cb.Remove(50));
        Assert.Equal(new byte[] { 0, 10, 20, 30, 40 }, cb.ToArray());
    }

    [Fact]
    public void Remove_Existing_True()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        Assert.True(cb.Remove(20));
        Assert.Equal(new byte[] { 0, 10, 30, 40 }, cb.ToArray());
    }

    [Fact]
    public void RemoveAt_Empty_Throws()
    {
        // ReSharper disable CollectionNeverUpdated.Local
        CircleBuffer<byte> cb = new(10);
        // ReSharper restore CollectionNeverUpdated.Local
        Assert.Throws<IndexOutOfRangeException>(() => cb.RemoveAt(0));
    }

    [Fact]
    public void RemoveAt_InvalidIndex_Throws()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        Assert.Throws<IndexOutOfRangeException>(() => cb.RemoveAt(8));
    }

    [Fact]
    public void Remove_ValidIndex_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        cb.RemoveAt(2);
        Assert.Equal(new byte[] { 0, 10, 30, 40 }, cb.ToArray());
    }

    [Fact]
    public void Contains_Empty_NoResults()
    {
        // ReSharper disable CollectionNeverUpdated.Local
        CircleBuffer<byte> cb = new(10);
        // ReSharper restore CollectionNeverUpdated.Local
#pragma warning disable xUnit2017
        Assert.False(cb.Contains(0));
#pragma warning restore xUnit2017
    }

    [Fact]
    public void IndexOf_Empty_NoResults()
    {
        // ReSharper disable CollectionNeverUpdated.Local
        CircleBuffer<byte> cb = new(10);
        // ReSharper restore CollectionNeverUpdated.Local
        Assert.Equal(-1, cb.IndexOf(0));
    }

    [Fact]
    public void Contains_Missing_NoResults()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(10);
#pragma warning disable xUnit2017
        Assert.False(cb.Contains(20));
#pragma warning restore xUnit2017
    }

    [Fact]
    public void IndexOf_Missing_NoResults()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(10);
        Assert.Equal(-1, cb.IndexOf(20));
    }

    [Fact]
    public void Contains_Existing_Found()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
#pragma warning disable xUnit2017
        Assert.True(cb.Contains(30));
#pragma warning restore xUnit2017
    }

    [Fact]
    public void IndexOf_Existing_Found()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        Assert.Equal(3, cb.IndexOf(30));
    }

    [Fact]
    public void GetEnumerator_ReturnsCorrect()
    {
        CircleBuffer<byte> cb = new(10);
        byte[] seq = new byte[] { 0, 10, 20, 30, 40 };
        cb.AddRange(seq);
        Assert.Equal(seq, cb.ToArray());
    }

    [Fact]
    public void IndexerGet_ValidIndex_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(50);
        Assert.Equal(50, cb[0]);
    }

    [Fact]
    public void IndexerSet_ValidIndex_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(50);
        cb[0] = 120;
        Assert.Equal(120, cb[0]);
    }

    [Fact]
    public void IndexerGet_InvalidIndex_Throws()
    {
        // ReSharper disable CollectionNeverUpdated.Local
        CircleBuffer<byte> cb = new(10);
        // ReSharper restore CollectionNeverUpdated.Local
        Assert.Throws<IndexOutOfRangeException>(() => cb[0]);
    }

    [Fact]
    public void IndexerSet_InvalidIndex_Throws()
    {
        // ReSharper disable CollectionNeverUpdated.Local
        CircleBuffer<byte> cb = new(10);
        // ReSharper restore CollectionNeverUpdated.Local
        Assert.Throws<IndexOutOfRangeException>(() => cb[0] = 120);
    }

    [Fact]
    public void AddRemoveCycle_SequenceCorrect()
    {
        CircleBuffer<byte> cb = new(100);
        Random r = new();
        byte[] a = new byte[60];
        r.NextBytes(a);
        foreach (byte b in a)
            cb.Add(b);
        List<byte> list = new(a);
        Assert.True(cb.SequenceEqual(list));
        cb.RemoveAt(40);
        list.RemoveAt(40);
        Assert.True(cb.SequenceEqual(list));
        cb.RemoveAt(10);
        list.RemoveAt(10);
        Assert.True(cb.SequenceEqual(list));
        cb.Insert(5, 10);
        list.Insert(5, 10);
        Assert.True(cb.SequenceEqual(list));
        cb.Insert(50, 60);
        list.Insert(50, 60);
        Assert.True(cb.SequenceEqual(list));
    }

    [Fact]
    public void AddBuffers_SequenceCorrect()
    {
        CircleBuffer<byte> cb = new(100);
        Random r = new();
        byte[] a = new byte[60];
        r.NextBytes(a);
        cb.AddRange(a);
        List<byte> list = new(a);
        // 60 @ 0
        Assert.True(cb.SequenceEqual(list));
        Assert.Equal(0, cb.First);
        cb.RemoveRange(40, 10);
        list.RemoveRange(40, 10);
        // 50 @ 0
        Assert.True(cb.SequenceEqual(list));
        Assert.Equal(0, cb.First);
        cb.RemoveRange(10, 10);
        list.RemoveRange(10, 10);
        // 40 @ 10
        Assert.True(cb.SequenceEqual(list));
        Assert.Equal(10, cb.First);
        cb.InsertRange(20, a);
        list.InsertRange(20, a);
        // 100 @ 10
        Assert.True(cb.SequenceEqual(list));
        Assert.Equal(10, cb.First);
        cb.RemoveRange(80, 20);
        list.RemoveRange(80, 20);
        // 80 @ 10
        Assert.True(cb.SequenceEqual(list));
        Assert.Equal(10, cb.First);
        cb.RemoveRange(20, 20);
        list.RemoveRange(20, 20);
        // 60 @ 30
        Assert.True(cb.SequenceEqual(list));
        Assert.Equal(30, cb.First);
    }
}
