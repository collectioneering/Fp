using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fp.Fs.CommandLine;

/// <summary>
/// Provides methods for executing command-line programs.
/// </summary>
public class CommandLineFsProcessor : IFsRunnerType
{
    /// <inheritdoc />
    public static void Run(FileSystemSource? fileSystemSource, IList<string>? args, params FsProcessorFactory[] factories)
    {
        FsProcessor.Registered.Factories.UnionWith(factories);
        Debug.Assert(FsProcessor.NO_EXECUTE_CLI == FpFsRootCommand.NoExecuteCliOption);
        var rootCommand = new FpFsRootCommand(fileSystemSource, factories);
        var parseResult = rootCommand.Parse(args?.ToArray() ?? []);
        parseResult.InvocationConfiguration.Output = Console.Out;
        parseResult.InvocationConfiguration.Error = Console.Error;
        parseResult.Invoke();
    }

    /// <inheritdoc />
    public static void Run(Action func, IList<string>? args, FileProcessorInfo? info = null)
    {
        Run(null, args, new DelegateFsProcessorFactory(info, () => new ScriptingDirectProcessor(func)));
    }

    /// <inheritdoc />
    public static void Run(Action func, IList<string>? args, string name, string description, params string?[] extensions)
    {
        Run(func, args, new FileProcessorInfo(name, description, description, extensions));
    }

    /// <inheritdoc />
    public static void Run(Func<IEnumerable<Data>> func, IList<string>? args, FileProcessorInfo? info = null)
    {
        Run(null, args, new DelegateFsProcessorFactory(info, () => new ScriptingSegmentedProcessor(func)));
    }

    /// <inheritdoc />
    public static void Run(Func<IEnumerable<Data>> func, IList<string>? args, string name, string description, params string?[] extensions)
    {
        Run(func, args, new FileProcessorInfo(name, description, description, extensions));
    }

    /// <inheritdoc />
    public static void Run<T>(IList<string>? args, FileProcessorInfo? info = null) where T : FsProcessor, new()
    {
        Run(null, args, new GenericNewFsProcessorFactory<T>(info));
    }

    /// <inheritdoc />
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FsProcessor, new()
    {
        Run(null, args, new GenericNewFsProcessorFactory<T>(new FileProcessorInfo(name, description, description, extensions)));
    }
}
