using System.Collections.Generic;

namespace Fp.Fs.CommandLine;

/// <summary>
/// Provides methods for executing command-line programs.
/// </summary>
public class CommandLineFsFormatSingleProcessor : IFsFormatSingleRunnerType
{
    /// <inheritdoc />
    public static void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FormatSingleProcessor, new()
    {
        CommandLineFsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info));
    }

    /// <inheritdoc />
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FormatSingleProcessor, new()
    {
        CommandLineFsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
    }
}

/// <summary>
/// Provides methods for executing command-line programs.
/// </summary>
public class CommandLineFsFormatSingleProcessor<TData> : IFsFormatSingleRunnerType<TData> where TData : Data
{
    /// <inheritdoc />
    public static void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FormatSingleProcessor<TData>, new()
    {
        CommandLineFsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info));
    }

    /// <inheritdoc />
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FormatSingleProcessor<TData>, new()
    {
        CommandLineFsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
    }
}
