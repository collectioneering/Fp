using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fp.Fs.CommandLine;

/// <summary>
/// Provides methods for executing command-line programs.
/// </summary>
public static class CommandLineFsFormatSingleProcessor
{
    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(
        FileSystemSource? fileSystemSource,
        IList<string>? args,
        FileProcessorInfo? info = null) where T : FormatSingleProcessor, new()
    {
        CommandLineFsProcessor.Run(fileSystemSource, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(
        IList<string>? args,
        FileProcessorInfo? info = null) where T : FormatSingleProcessor, new()
    {
        CommandLineFsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info));
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
    public static void Run<T>(
        FileSystemSource? fileSystemSource,
        IList<string>? args,
        string name,
        string description,
        params string?[] extensions) where T : FormatSingleProcessor, new()
    {
        CommandLineFsProcessor.Run(fileSystemSource, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(
        IList<string>? args,
        string name,
        string description,
        params string?[] extensions) where T : FormatSingleProcessor, new()
    {
        CommandLineFsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
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
        CancellationToken cancellationToken = default) where T : FormatSingleProcessor, new()
    {
        return CommandLineFsProcessor.RunAsync(fileSystemSource, args, [new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info)], cancellationToken);
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
        CancellationToken cancellationToken = default) where T : FormatSingleProcessor, new()
    {
        return CommandLineFsProcessor.RunAsync(null, args, [new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info)], cancellationToken);
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
        CancellationToken cancellationToken = default) where T : FormatSingleProcessor, new()
    {
        return CommandLineFsProcessor.RunAsync(fileSystemSource, args, [new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions))], cancellationToken);
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
        CancellationToken cancellationToken = default) where T : FormatSingleProcessor, new()
    {
        return CommandLineFsProcessor.RunAsync(null, args, [new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions))], cancellationToken);
    }
}

/// <summary>
/// Provides methods for executing command-line programs.
/// </summary>
public static class CommandLineFsFormatSingleProcessor<TData> where TData : Data
{
    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(
        FileSystemSource? fileSystemSource,
        IList<string>? args,
        FileProcessorInfo? info = null) where T : FormatSingleProcessor<TData>, new()
    {
        CommandLineFsProcessor.Run(fileSystemSource, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(
        IList<string>? args,
        FileProcessorInfo? info = null) where T : FormatSingleProcessor<TData>, new()
    {
        CommandLineFsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info));
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
    public static void Run<T>(
        FileSystemSource? fileSystemSource,
        IList<string>? args,
        string name,
        string description,
        params string?[] extensions) where T : FormatSingleProcessor<TData>, new()
    {
        CommandLineFsProcessor.Run(fileSystemSource, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(
        IList<string>? args,
        string name,
        string description,
        params string?[] extensions) where T : FormatSingleProcessor<TData>, new()
    {
        CommandLineFsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
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
        CancellationToken cancellationToken = default) where T : FormatSingleProcessor<TData>, new()
    {
        return CommandLineFsProcessor.RunAsync(fileSystemSource, args, [new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info)], cancellationToken);
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
        CancellationToken cancellationToken = default) where T : FormatSingleProcessor<TData>, new()
    {
        return CommandLineFsProcessor.RunAsync(null, args, [new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info)], cancellationToken);
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
        CancellationToken cancellationToken = default) where T : FormatSingleProcessor<TData>, new()
    {
        return CommandLineFsProcessor.RunAsync(fileSystemSource, args, [new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions))], cancellationToken);
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
        CancellationToken cancellationToken = default) where T : FormatSingleProcessor<TData>, new()
    {
        return CommandLineFsProcessor.RunAsync(null, args, [new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions))], cancellationToken);
    }
}
