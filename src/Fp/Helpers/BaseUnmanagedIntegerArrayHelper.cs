using System;
using static Fp.Processor;

namespace Fp.Helpers;

/// <summary>
/// Base unmanaged integer array data helper.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public abstract partial record BaseUnmanagedIntegerArrayHelper<T>
    : BaseUnmanagedArrayHelper<T> where T : unmanaged, System.Numerics.INumber<T>;

/// <summary>
/// Base unmanaged single byte integer array data helper.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public abstract partial record BaseUnmanagedSingleByteIntegerArrayHelper<T>
    : BaseUnmanagedIntegerArrayHelper<T> where T : unmanaged, System.Numerics.INumber<T>
{
    /// <inheritdoc />
    public override ReadOnlySpan<T> this[Span<byte> source]
    {
        get => GetNumberArray<T>(source, true);
        set => SetNumberArray(source, value, true);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<T> this[ReadOnlySpan<byte> source] => GetNumberArray<T>(source, true);
}

/// <summary>
/// Base unmanaged multi-byte integer array data helper.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public abstract partial record BaseUnmanagedMultiByteIntegerArrayHelper<T>(bool LittleEndian)
    : BaseUnmanagedIntegerArrayHelper<T> where T : unmanaged, System.Numerics.INumber<T>
{
    /// <inheritdoc />
    public override ReadOnlySpan<T> this[Span<byte> source]
    {
        get => GetNumberArray<T>(source, LittleEndian);
        set => SetNumberArray(source, value, LittleEndian);
    }

    /// <inheritdoc />
    public override ReadOnlySpan<T> this[ReadOnlySpan<byte> source] => GetNumberArray<T>(source, LittleEndian);
}
