using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fp.Fs.CommandLine;

partial class CommandLineFsProcessor
{
    /// <summary>
    /// Processes using processor factory.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processors.</param>
    /// <param name="factories">Processor factories.</param>
    public static void Run(FileSystemSource? fileSystemSource, IList<string>? args, params FsProcessorFactory[] factories)
    {
        Run(fileSystemSource, args, (IReadOnlyList<FsProcessorFactory>)factories);
    }

    /// <summary>
    /// Processes using processor factory.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processors.</param>
    /// <param name="factories">Processor factories.</param>
    public static void Run(FileSystemSource? fileSystemSource, IList<string>? args, IReadOnlyList<FsProcessorFactory> factories)
    {
        var rootCommand = new FpFsRootCommand(fileSystemSource, factories);
        var parseResult = rootCommand.Parse(args?.ToArray() ?? []);
        parseResult.InvocationConfiguration.Output = Console.Out;
        parseResult.InvocationConfiguration.Error = Console.Error;
        parseResult.Invoke();
    }

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    public static void Run(FileSystemSource? fileSystemSource, Action func, IList<string>? args, FileProcessorInfo? info = null)
    {
        Run(fileSystemSource, args, new DelegateFsProcessorFactory(info, () => new ScriptingDirectProcessor(func)));
    }

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    public static void Run(Action func, IList<string>? args, FileProcessorInfo? info = null)
    {
        Run(null, args, new DelegateFsProcessorFactory(info, () => new ScriptingDirectProcessor(func)));
    }

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    public static void Run(FileSystemSource? fileSystemSource, Action func, IList<string>? args, string name, string description, params string?[] extensions)
    {
        Run(fileSystemSource, func, args, new FileProcessorInfo(name, description, description, extensions));
    }

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    public static void Run(Action func, IList<string>? args, string name, string description, params string?[] extensions)
    {
        Run(null, func, args, new FileProcessorInfo(name, description, description, extensions));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    public static void Run(FileSystemSource? fileSystemSource, Func<IEnumerable<Data>> func, IList<string>? args, FileProcessorInfo? info = null)
    {
        Run(fileSystemSource, args, new DelegateFsProcessorFactory(info, () => new ScriptingSegmentedProcessor(func)));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    public static void Run(Func<IEnumerable<Data>> func, IList<string>? args, FileProcessorInfo? info = null)
    {
        Run(null, args, new DelegateFsProcessorFactory(info, () => new ScriptingSegmentedProcessor(func)));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    public static void Run(FileSystemSource? fileSystemSource, Func<IEnumerable<Data>> func, IList<string>? args, string name, string description, params string?[] extensions)
    {
        Run(fileSystemSource, func, args, new FileProcessorInfo(name, description, description, extensions));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    public static void Run(Func<IEnumerable<Data>> func, IList<string>? args, string name, string description, params string?[] extensions)
    {
        Run(null, func, args, new FileProcessorInfo(name, description, description, extensions));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(FileSystemSource? fileSystemSource, IList<string>? args, FileProcessorInfo? info = null) where T : FsProcessor, new()
    {
        Run(fileSystemSource, args, new GenericNewFsProcessorFactory<T>(info));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FsProcessor, new()
    {
        Run(null, args, new GenericNewFsProcessorFactory<T>(info));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(FileSystemSource? fileSystemSource, IList<string>? args, string name, string description, params string?[] extensions) where T : FsProcessor, new()
    {
        Run(fileSystemSource, args, new GenericNewFsProcessorFactory<T>(new FileProcessorInfo(name, description, description, extensions)));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FsProcessor, new()
    {
        Run(null, args, new GenericNewFsProcessorFactory<T>(new FileProcessorInfo(name, description, description, extensions)));
    }
}
