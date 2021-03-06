using System;
using System.Diagnostics.CodeAnalysis;

namespace Fp.Fs;

/// <summary>
/// Processor information attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class FsProcessorInfoAttribute : Attribute
{
    /// <summary>
    /// Processor information.
    /// </summary>
    public readonly FileProcessorInfo Info;

    /// <summary>
    /// Processor information attribute.
    /// </summary>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extendedDescription">Processor extended description.</param>
    /// <param name="extensions">Processor extensions.</param>
    public FsProcessorInfoAttribute(string name, string description, string extendedDescription,
        params string?[] extensions)
    {
        Info = new FileProcessorInfo(name, description, extendedDescription, extensions);
    }
}
