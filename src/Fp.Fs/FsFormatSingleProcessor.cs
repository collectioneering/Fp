using System;
using System.Collections.Generic;

namespace Fp.Fs;

/// <inheritdoc cref="FormatSingleProcessor" />
public class FsFormatSingleProcessor : FormatSingleProcessor, IFsFormatSingleRunnerType
{
    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FormatSingleProcessor, new()
    {
        FsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info));
    }

    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FormatSingleProcessor, new()
    {
        FsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
    }
}

/// <inheritdoc cref="FormatSingleProcessor{T}" />
public class FsFormatSingleProcessor<TData> : FormatSingleProcessor<TData>, IFsFormatSingleRunnerType<TData> where TData : Data
{
    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FormatSingleProcessor<TData>, new()
    {
        FsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(info));
    }

    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FormatSingleProcessor<TData>, new()
    {
        FsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
    }
}
