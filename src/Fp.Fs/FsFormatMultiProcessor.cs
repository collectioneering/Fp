using System.Collections.Generic;

namespace Fp.Fs;

/// <inheritdoc cref="FormatMultiProcessor" />
public class FsFormatMultiProcessor : FormatMultiProcessor, IFsFormatMultiRunnerType
{
    /// <inheritdoc />
    public static void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FormatMultiProcessor, new()
    {
        FsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatMultiProcessorFsWrapper<T>>(info));
    }

    /// <inheritdoc />
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FormatMultiProcessor, new()
    {
        FsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatMultiProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
    }
}
