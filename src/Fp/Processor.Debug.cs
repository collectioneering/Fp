using System;
using System.Collections.Generic;
using System.Linq;

namespace Fp;

public partial class Processor
{
    #region Debugging utilities

    /// <summary>
    /// If true, enables debugging features.
    /// </summary>
    public bool Debug;

    /// <summary>
    /// If true, disable outputs.
    /// </summary>
    public bool Nop;

    /// <summary>
    /// Annotations for memory.
    /// </summary>
    public readonly Dictionary<ReadOnlyMemory<byte>, SortedList<int, MemAnnotation>> MemAnnotations = new();
    private int _memColorIdx;

    /// <summary>
    /// Clears stored memories and annotations.
    /// </summary>
    public void MemClear()
    {
        MemAnnotations.Clear();
    }

    /// <summary>
    /// Labels memory with annotation.
    /// </summary>
    /// <param name="memory">Target memory.</param>
    /// <param name="offset">Data offset.</param>
    /// <param name="length">Data length.</param>
    /// <param name="label">Annotation to add.</param>
    /// <param name="color">Color, random default.</param>
    /// <remarks>No-op if <see cref="Debug"/> is false.<br/>Users should not slice memory struct between label and print, uses <see cref="MemAnnotations"/> which uses the memory as a key.</remarks>
    public void MemLabel(ReadOnlyMemory<byte> memory, int offset, int length, string? label = null,
        ConsoleColor? color = null)
    {
        if (!Debug) return;
        if (!MemAnnotations.TryGetValue(memory, out SortedList<int, MemAnnotation>? list))
            list = MemAnnotations[memory] = new SortedList<int, MemAnnotation>();
        if (color == null)
        {
            color = ConsoleLog.Colors[_memColorIdx];
            _memColorIdx = (_memColorIdx + 1) % (ConsoleLog.Colors.Count - 1);
        }

        if (!list.ContainsKey(offset))
            list.Add(offset, new MemAnnotation(offset, length, label, color.Value));
    }

    /// <summary>
    /// Labels occurrences of sequence in memory with annotation.
    /// </summary>
    /// <param name="memory">Target memory.</param>
    /// <param name="sequence">Sequence to search for.</param>
    /// <param name="label">Annotation to add.</param>
    /// <param name="color">Color, random default.</param>
    /// <remarks>No-op if <see cref="Debug"/> is false.<br/>Users should not slice memory struct between label and print, uses <see cref="MemAnnotations"/> which uses the memory as a key.</remarks>
    /// <returns>Matches (regardless of debug enabled/not).</returns>
    public List<int> MemLabel(ReadOnlyMemory<byte> memory, ReadOnlySpan<byte> sequence, string? label = null,
        ConsoleColor? color = null)
    {
        List<int> matches = Match(memory.Span, 0, memory.Length, sequence);
        if (!Debug) return matches;
        int sl = sequence.Length;
        for (int i = 0; i < matches.Count; i++)
        {
            int offset = matches[i];
            int count = 1;
            while (i + 1 < matches.Count && offset + count * sl == matches[i + 1])
            {
                matches.RemoveAt(i + 1);
                count++;
            }
            MemLabel(memory, offset, count * sl, label, color);
        }

        return matches;
    }

    /// <summary>
    /// Prints memory with associated annotations.
    /// </summary>
    /// <param name="memory">Target memory.</param>
    /// <param name="space">Space between bytes.</param>
    /// <param name="pow2Modulus">Only display power of 2 per line.</param>
    /// <param name="displayWidth">Available display width.</param>
    /// <remarks>No-op if <see cref="Debug"/> is false.<br/>Users should not slice memory struct between label and print, uses <see cref="MemAnnotations"/> which uses the memory as a key.</remarks>
    public void MemPrint(ReadOnlyMemory<byte> memory, bool space = true, bool pow2Modulus = true, int? displayWidth = null)
    {
        if (!Debug) return;
        HexPrint.Print(memory.Span, LogWriter,
            MemAnnotations.TryGetValue(memory,
                out SortedList<int, MemAnnotation>? list)
                ? list.Values.ToArray()
                : Array.Empty<MemAnnotation>(), space,
            pow2Modulus, displayWidth);
    }

    #endregion
}
