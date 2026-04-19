using System.Collections.Generic;

namespace Fp.Fs;

/// <summary>
/// Provides base runner interface for <see cref="FormatSingleProcessor"/>.
/// </summary>
public interface IFsFormatSingleRunnerType
{
    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    static abstract void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FormatSingleProcessor, new();

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    static abstract void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FormatSingleProcessor, new();
}
