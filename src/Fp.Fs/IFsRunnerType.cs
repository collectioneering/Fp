using System;
using System.Collections.Generic;

namespace Fp.Fs;

/// <summary>
/// Provides base runner interface for <see cref="FsProcessor"/>.
/// </summary>
public interface IFsRunnerType
{
    /// <summary>
    /// Processes using processor factory.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processors.</param>
    /// <param name="factories">Processor factories.</param>
    static abstract void Run(FileSystemSource? fileSystemSource, IList<string>? args, params FsProcessorFactory[] factories);

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    static abstract void Run(Action func, IList<string>? args, FileProcessorInfo? info = null);

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    static abstract void Run(Action func, IList<string>? args, string name, string description, params string?[] extensions);


    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    static abstract void Run(Func<IEnumerable<Data>> func, IList<string>? args, FileProcessorInfo? info = null);

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    static abstract void Run(Func<IEnumerable<Data>> func, IList<string>? args, string name, string description, params string?[] extensions);

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    static abstract void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FsProcessor, new();

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    static abstract void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FsProcessor, new();
}
