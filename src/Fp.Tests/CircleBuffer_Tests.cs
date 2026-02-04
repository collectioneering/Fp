using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Fp.Tests;

public class CircleBuffer_Tests
{
    [Test]
    public void Constructor_CapacityZero_Allowed()
    {
        Assert.That(() => new CircleBuffer<byte>(0), Throws.Nothing);
    }

    [Test]
    public void Constructor_CapacityUnderZero_Disallowed()
    {
        Assert.That(() => new CircleBuffer<byte>(-1), Throws.ArgumentException);
    }

    [Test]
    public void Capacity_ReportsCorrectly()
    {
        CircleBuffer<byte> cb = new(10);
        Assert.That(cb.Capacity, Is.EqualTo(10));
    }

    [Test]
    public void Add_UnderCapacity_Success()
    {
        CircleBuffer<byte> cb = new(10);
        Assert.That(() => cb.Add(50), Throws.Nothing);
        Assert.That(cb[0], Is.EqualTo(50));
    }

    [Test]
    public void Add_AtCapacity_Throws()
    {
        CircleBuffer<byte> cb = new(10);
        for (int i = 0; i < 10; i++) cb.Add(50);
        Assert.That(() => cb.Add(50), Throws.InvalidOperationException);
    }

    [Test]
    public void Insert_FromEmpty_Success()
    {
        CircleBuffer<byte> cb = new(10);
        Assert.That(() => cb.Insert(0, 50), Throws.Nothing);
        Assert.That(cb[0], Is.EqualTo(50));
    }

    [Test]
    public void Insert_FromStart_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(10);
        cb.Add(20);
        Assert.That(() => cb.Insert(0, 50), Throws.Nothing);
        Assert.That(cb.ToArray(), Is.EqualTo(new byte[] { 50, 10, 20 }));
    }

    [Test]
    public void Insert_FromMiddle_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(10);
        cb.Add(20);
        Assert.That(() => cb.Insert(1, 50), Throws.Nothing);
        Assert.That(cb.ToArray(), Is.EqualTo(new byte[] { 10, 50, 20 }));
    }

    [Test]
    public void Insert_FromEnd_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(10);
        cb.Add(20);
        Assert.That(() => cb.Insert(2, 50), Throws.Nothing);
        Assert.That(cb.ToArray(), Is.EqualTo(new byte[] { 10, 20, 50 }));
    }

    [Test]
    public void Remove_Empty_False()
    {
        CircleBuffer<byte> cb = new(10);
        Assert.That(cb.Remove(20), Is.False);
        Assert.That(cb.Count, Is.EqualTo(0));
    }

    [Test]
    public void Remove_Missing_False()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        Assert.That(cb.Remove(50), Is.False);
        Assert.That(cb.ToArray(), Is.EqualTo(new byte[] { 0, 10, 20, 30, 40 }));
    }

    [Test]
    public void Remove_Existing_True()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        Assert.That(cb.Remove(20), Is.True);
        Assert.That(cb.ToArray(), Is.EqualTo(new byte[] { 0, 10, 30, 40 }));
    }

    [Test]
    public void RemoveAt_Empty_Throws()
    {
        CircleBuffer<byte> cb = new(10);
        Assert.That(() => cb.RemoveAt(0), Throws.InstanceOf<IndexOutOfRangeException>());
    }

    [Test]
    public void RemoveAt_InvalidIndex_Throws()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        Assert.That(() => cb.RemoveAt(8), Throws.InstanceOf<IndexOutOfRangeException>());
    }

    [Test]
    public void Remove_ValidIndex_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        Assert.That(() => cb.RemoveAt(2), Throws.Nothing);
        Assert.That(cb.ToArray(), Is.EqualTo(new byte[] { 0, 10, 30, 40 }));
    }

    [Test]
    public void Contains_Empty_NoResults()
    {
        CircleBuffer<byte> cb = new(10);
        Assert.That(cb.Contains(0), Is.False);
    }

    [Test]
    public void IndexOf_Empty_NoResults()
    {
        CircleBuffer<byte> cb = new(10);
        Assert.That(cb.IndexOf(0), Is.EqualTo(-1));
    }

    [Test]
    public void Contains_Missing_NoResults()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(10);
        Assert.That(cb.Contains(20), Is.False);
    }

    [Test]
    public void IndexOf_Missing_NoResults()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(10);
        Assert.That(cb.IndexOf(20), Is.EqualTo(-1));
    }

    [Test]
    public void Contains_Existing_Found()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        Assert.That(cb.Contains(30), Is.True);
    }

    [Test]
    public void IndexOf_Existing_Found()
    {
        CircleBuffer<byte> cb = new(10);
        cb.AddRange(new byte[] { 0, 10, 20, 30, 40 });
        Assert.That(cb.IndexOf(30), Is.EqualTo(3));
    }

    [Test]
    public void GetEnumerator_ReturnsCorrect()
    {
        CircleBuffer<byte> cb = new(10);
        byte[] seq = new byte[] { 0, 10, 20, 30, 40 };
        cb.AddRange(seq);
        Assert.That(cb.ToArray(), Is.EqualTo(seq));
    }

    [Test]
    public void IndexerGet_ValidIndex_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(50);
        Assert.That(cb[0], Is.EqualTo(50));
    }

    [Test]
    public void IndexerSet_ValidIndex_Success()
    {
        CircleBuffer<byte> cb = new(10);
        cb.Add(50);
        Assert.That(() => cb[0] = 120, Throws.Nothing);
        Assert.That(cb[0], Is.EqualTo(120));
    }

    [Test]
    public void IndexerGet_InvalidIndex_Throws()
    {
        CircleBuffer<byte> cb = new(10);
        Assert.That(() => cb[0], Throws.InstanceOf<IndexOutOfRangeException>());
    }

    [Test]
    public void IndexerSet_InvalidIndex_Throws()
    {
        CircleBuffer<byte> cb = new(10);
        Assert.That(() => cb[0] = 120, Throws.InstanceOf<IndexOutOfRangeException>());
    }

    [Test]
    public void AddRemoveCycle_SequenceCorrect()
    {
        CircleBuffer<byte> cb = new(100);
        Random r = new();
        byte[] a = new byte[60];
        r.NextBytes(a);
        foreach (byte b in a)
            cb.Add(b);
        List<byte> list = new(a);
        Assert.That(cb.SequenceEqual(list));
        cb.RemoveAt(40);
        list.RemoveAt(40);
        Assert.That(cb.SequenceEqual(list));
        cb.RemoveAt(10);
        list.RemoveAt(10);
        Assert.That(cb.SequenceEqual(list));
        cb.Insert(5, 10);
        list.Insert(5, 10);
        Assert.That(cb.SequenceEqual(list));
        cb.Insert(50, 60);
        list.Insert(50, 60);
        Assert.That(cb.SequenceEqual(list));
    }
}
