using System;
using System.Collections.Generic;
using System.Linq;

namespace Fp.Fs;

// ReSharper disable InconsistentNaming
public partial class FsProcessor : IFsRunnerType
{
    /// <summary>
    /// Keyword arg for stopping cli execution.
    /// </summary>
    public const string NO_EXECUTE_CLI = "--no-execute-cli";

    /// <summary>
    /// Registered scripting processors.
    /// </summary>
    public static readonly FsProcessorSource Registered = new();

    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run(FileSystemSource? fileSystemSource, IList<string>? args, params FsProcessorFactory[] factories)
    {
        Registered.Factories.UnionWith(factories);
        if (args == null || args.Count == 1 && args[0] == NO_EXECUTE_CLI) return;
        Coordinator.CliRunFilesystem(args.ToArray(), default, default, fileSystemSource, factories);
    }

    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run(Action func, IList<string>? args, FileProcessorInfo? info = null)
    {
        Run(null, args, new DelegateFsProcessorFactory(info, () => new ScriptingDirectProcessor(func)));
    }

    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run(Action func, IList<string>? args, string name, string description, params string?[] extensions)
    {
        Run(func, args, new FileProcessorInfo(name, description, description, extensions));
    }

    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run(Func<IEnumerable<Data>> func, IList<string>? args, FileProcessorInfo? info = null)
    {
        Run(null, args, new DelegateFsProcessorFactory(info, () => new ScriptingSegmentedProcessor(func)));
    }

    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run(Func<IEnumerable<Data>> func, IList<string>? args, string name, string description, params string?[] extensions)
    {
        Run(func, args, new FileProcessorInfo(name, description, description, extensions));
    }

    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FsProcessor, new()
    {
        Run(null, args, new GenericNewFsProcessorFactory<T>(info));
    }

    /// <inheritdoc />
    [Obsolete("Install the Fp.Fs.CommandLine package for command-line parsing")]
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FsProcessor, new()
    {
        Run(null, args, new GenericNewFsProcessorFactory<T>(new FileProcessorInfo(name, description, description, extensions)));
    }
}
