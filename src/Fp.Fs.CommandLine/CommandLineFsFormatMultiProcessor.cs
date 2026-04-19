using System.Collections.Generic;

namespace Fp.Fs.CommandLine;

/// <summary>
/// Provides methods for executing command-line programs.
/// </summary>
public class CommandLineFsFormatMultiProcessor : IFsFormatMultiRunnerType
{
    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FormatMultiProcessor, new()
    {
        CommandLineFsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatMultiProcessorFsWrapper<T>>(info));
    }

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FormatMultiProcessor, new()
    {
        CommandLineFsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatMultiProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
    }
}
