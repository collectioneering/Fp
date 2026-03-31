using System;
using System.Runtime.InteropServices;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Bitwise : ProcessorTestBase
{
    [Test]
    public void GetAlignmentStart_MultiElementLessFromStart_Next()
    {
        Span<ulong> v = stackalloc ulong[2];
        Assert.That(Processor.GetAlignmentStart(MemoryMarshal.Cast<ulong, byte>(v)[1..], sizeof(ulong)), Is.EqualTo(sizeof(ulong) - 1));
    }

    [Test]
    public void GetAlignmentStart_MultiElementLessFromStartAnd_Current()
    {
        Span<ulong> v = stackalloc ulong[2];
        Assert.That(Processor.GetAlignmentStart(MemoryMarshal.Cast<ulong, byte>(v)[1..^1], sizeof(ulong)), Is.EqualTo(sizeof(ulong) - 1));
    }

    [Test]
    public void GetAlignmentStart_MultiElementLessFromEnd_Current()
    {
        Span<ulong> v = stackalloc ulong[2];
        Assert.That(Processor.GetAlignmentStart(MemoryMarshal.Cast<ulong, byte>(v)[..^1], sizeof(ulong)), Is.EqualTo(0));
    }

    [Test]
    public void GetAlignmentStart_LessFromStart_End()
    {
        Span<ulong> v = stackalloc ulong[1];
        Assert.That(Processor.GetAlignmentStart(MemoryMarshal.Cast<ulong, byte>(v)[1..], sizeof(ulong)), Is.EqualTo(sizeof(ulong) - 1));
    }

    [Test]
    public void GetAlignmentStart_LessFromStartAndEnd_End()
    {
        Span<ulong> v = stackalloc ulong[1];
        Assert.That(Processor.GetAlignmentStart(MemoryMarshal.Cast<ulong, byte>(v)[1..^1], sizeof(ulong)), Is.EqualTo(sizeof(ulong) - 2));
    }

    [Test]
    public void GetAlignmentStart_LessFromEnd_End()
    {
        Span<ulong> v = stackalloc ulong[1];
        Assert.That(Processor.GetAlignmentStart(MemoryMarshal.Cast<ulong, byte>(v)[..^1], sizeof(ulong)), Is.EqualTo(0));
    }

    [Test]
    public void ContainsAtLeastOneAligned_LessFromStart_False()
    {
        Span<ulong> v = stackalloc ulong[1];
        Assert.That(Processor.ContainsAtLeastOneAligned(MemoryMarshal.Cast<ulong, byte>(v)[1..], sizeof(ulong)), Is.False);
    }

    [Test]
    public void ContainsAtLeastOneAligned_LessFromEnd_False()
    {
        Span<ulong> v = stackalloc ulong[1];
        Assert.That(Processor.ContainsAtLeastOneAligned(MemoryMarshal.Cast<ulong, byte>(v)[..^1], sizeof(ulong)), Is.False);
    }

    [Test]
    public void ContainsAtLeastOneAligned_Exact_True()
    {
        Span<ulong> v = stackalloc ulong[1];
        Assert.That(Processor.ContainsAtLeastOneAligned(MemoryMarshal.Cast<ulong, byte>(v), sizeof(ulong)), Is.True);
    }

    [Test]
    public void ContainsAtLeastOneAligned_MoreFromStart_True()
    {
        Span<ulong> v = stackalloc ulong[2];
        Assert.That(Processor.ContainsAtLeastOneAligned(MemoryMarshal.Cast<ulong, byte>(v)[1..], sizeof(ulong)), Is.True);
    }

    [Test]
    public void ContainsAtLeastOneAligned_MoreFromEnd_True()
    {
        Span<ulong> v = stackalloc ulong[2];
        Assert.That(Processor.ContainsAtLeastOneAligned(MemoryMarshal.Cast<ulong, byte>(v)[..^1], sizeof(ulong)), Is.True);
    }

    [Test]
    public void ApplyTransform_BasicTransform_Works()
    {
        int[] source = { 0, 1, 2, 3, 4, 5, 6, 7 };
        int[] expected = { 0, 2, 4, 6, 8, 10, 12, 14 };
        Processor.ApplyTransform(source, (v, _) => v * 2);
        Assert.That(source, Is.EqualTo(expected));
    }
}
