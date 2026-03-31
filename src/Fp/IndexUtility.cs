using System;

namespace Fp;

/// <summary>
/// Provides utilities for working with <see cref="Index"/>.
/// </summary>
public static class IndexUtility
{
    /// <summary>
    /// Produces an <see cref="Index"/> with the specified offset applied.
    /// </summary>
    /// <param name="index">Index to modify.</param>
    /// <param name="offset">Offset to apply.</param>
    /// <returns>Value with offset applied.</returns>
    public static Index WithOffset(this Index index, int offset)
    {
        if (!index.IsFromEnd)
        {
            return new Index(index.Value + offset);
        }
        int distanceFromEnd = int.MaxValue - index.GetOffset(int.MaxValue);
        return new Index(distanceFromEnd - offset, true);
    }
}
