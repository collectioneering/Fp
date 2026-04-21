using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fp.Fs.CommandLine;

partial class CommandLineFsProcessor
{
    /// <summary>
    /// Processes using processor factory.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processors.</param>
    /// <param name="factories">Processor factories.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task<int> RunAsync(
        FileSystemSource? fileSystemSource,
        IList<string>? args,
        IReadOnlyList<FsProcessorFactory> factories,
        CancellationToken cancellationToken = default)
    {
        var rootCommand = new FpFsRootCommand(fileSystemSource, factories);
        var parseResult = rootCommand.Parse(args?.ToArray() ?? []);
        parseResult.InvocationConfiguration.Output = Console.Out;
        parseResult.InvocationConfiguration.Error = Console.Error;
        return await parseResult.InvokeAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static Task<int> RunAsync(
        FileSystemSource? fileSystemSource,
        Action func,
        IList<string>? args,
        FileProcessorInfo? info = null,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(fileSystemSource, args, [new DelegateFsProcessorFactory(info, () => new ScriptingDirectProcessor(func))], cancellationToken);
    }

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static Task<int> RunAsync(
        Action func,
        IList<string>? args,
        FileProcessorInfo? info = null,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(null, args, [new DelegateFsProcessorFactory(info, () => new ScriptingDirectProcessor(func))], cancellationToken);
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
    /// <param name="cancellationToken">Cancellation token.</param>
    public static Task<int> RunAsync(
        FileSystemSource? fileSystemSource,
        Action func,
        IList<string>? args,
        string name,
        string description,
        string?[] extensions,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(fileSystemSource, func, args, new FileProcessorInfo(name, description, description, extensions), cancellationToken);
    }

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static Task<int> RunAsync(
        Action func,
        IList<string>? args,
        string name,
        string description,
        string?[] extensions,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(null, func, args, new FileProcessorInfo(name, description, description, extensions), cancellationToken);
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static Task<int> RunAsync(
        FileSystemSource? fileSystemSource,
        Func<IEnumerable<Data>> func,
        IList<string>? args,
        FileProcessorInfo? info = null,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(fileSystemSource, args, [new DelegateFsProcessorFactory(info, () => new ScriptingSegmentedProcessor(func))], cancellationToken);
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static Task<int> RunAsync(
        Func<IEnumerable<Data>> func,
        IList<string>? args,
        FileProcessorInfo? info = null,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(null, args, [new DelegateFsProcessorFactory(info, () => new ScriptingSegmentedProcessor(func))], cancellationToken);
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
    /// <param name="cancellationToken">Cancellation token.</param>
    public static Task<int> RunAsync(
        FileSystemSource? fileSystemSource,
        Func<IEnumerable<Data>> func,
        IList<string>? args,
        string name,
        string description,
        string?[] extensions,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(fileSystemSource, func, args, new FileProcessorInfo(name, description, description, extensions), cancellationToken);
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static Task<int> RunAsync(
        Func<IEnumerable<Data>> func,
        IList<string>? args,
        string name,
        string description,
        string?[] extensions,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(null, func, args, new FileProcessorInfo(name, description, description, extensions), cancellationToken);
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static Task<int> RunAsync<T>(
        FileSystemSource? fileSystemSource,
        IList<string>? args,
        FileProcessorInfo? info = null,
        CancellationToken cancellationToken = default)
        where T : FsProcessor, new()
    {
        return RunAsync(fileSystemSource, args, [new GenericNewFsProcessorFactory<T>(info)], cancellationToken);
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static Task<int> RunAsync<T>(
        IList<string>? args,
        FileProcessorInfo? info = null,
        CancellationToken cancellationToken = default)
        where T : FsProcessor, new()
    {
        return RunAsync(null, args, [new GenericNewFsProcessorFactory<T>(info)], cancellationToken);
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static Task<int> RunAsync<T>(
        FileSystemSource? fileSystemSource,
        IList<string>? args,
        string name,
        string description,
        string?[] extensions,
        CancellationToken cancellationToken = default)
        where T : FsProcessor, new()
    {
        return RunAsync(fileSystemSource, args, [new GenericNewFsProcessorFactory<T>(new FileProcessorInfo(name, description, description, extensions))], cancellationToken);
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static Task<int> RunAsync<T>(
        IList<string>? args,
        string name,
        string description,
        string?[] extensions,
        CancellationToken cancellationToken = default)
        where T : FsProcessor, new()
    {
        return RunAsync(null, args, [new GenericNewFsProcessorFactory<T>(new FileProcessorInfo(name, description, description, extensions))], cancellationToken);
    }
}
