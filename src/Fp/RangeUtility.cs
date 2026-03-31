using System;

namespace Fp;

/// <summary>
/// Provides utilities for working with <see cref="Range"/>.
/// </summary>
public static class RangeUtility
{
    /// <summary>
    /// Produces a <see cref="Range"/> with the specified offset applied, preserving the internal length.
    /// </summary>
    /// <param name="range">Range to modify.</param>
    /// <param name="offset">Offset to apply.</param>
    /// <returns>Value with offset applied.</returns>
    public static Range WithOffset(this Range range, int offset)
    {
        return new Range(range.Start.WithOffset(offset), range.End.WithOffset(offset));
    }
}
