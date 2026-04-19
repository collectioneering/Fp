using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fp.Fs;

/// <summary>
/// Execution manager.
/// </summary>
public static class Coordinator
{
    #region Constants

    /// <summary>
    /// Default output folder name.
    /// </summary>
    public const string DefaultOutputFolderName = "fp_output";

    #endregion

    #region Effective read-only

    /// <summary>
    /// Default name for current executable (argv[0] or generic name).
    /// </summary>
    public static string DefaultCurrentExecutableName
    {
        get
        {
            if (s_defaultCurrentExecutableName != null) return s_defaultCurrentExecutableName;
            try
            {
                s_defaultCurrentExecutableName = Environment.GetCommandLineArgs()[0];
            }
            catch
            {
                s_defaultCurrentExecutableName = "<program>";
            }

            return s_defaultCurrentExecutableName;
        }
    }

    private static string? s_defaultCurrentExecutableName;

    #endregion

    #region CLI tools

    internal static FpInput ResolveFpInputForPath(string str)
    {
        string full = Path.GetFullPath(str);
        return new FpInput(
            File.Exists(str),
            Path.GetDirectoryName(full) ?? Path.GetPathRoot(full) ?? throw new ArgumentException($"Failed to resolve path root for [{str}] (full: [{full}])"),
            full);
    }

    internal static void AppendProcessorInfos(
        StringBuilder stringBuilder,
        IEnumerable<FileProcessorInfo> infos,
        string linePrefix = "")
    {
        foreach (var i in infos)
        {
            stringBuilder.Append(i.Name).AppendLine()
                .Append(linePrefix).Append("    Extensions:");
            if (i.Extensions.Length == 0) stringBuilder.Append(" <all>");
            else stringBuilder.Append(' ').Append(i.Extensions[0] ?? "<empty>");
            foreach (string? ext in i.Extensions.Skip(1))
                stringBuilder.Append(", ").Append(ext ?? "<empty>");
            stringBuilder.AppendLine()
                .Append(linePrefix).Append("    ").Append(i.ExtendedDescription.Replace("\n", "\n    ")).AppendLine();
        }
    }

    /// <summary>
    /// Gets processor configuration from cli.
    /// </summary>
    /// <param name="exeName">Executable name.</param>
    /// <param name="args">Command-line arguments.</param>
    /// <param name="logWriter">Log receiver for errors.</param>
    /// <param name="enableParallel">If true, enable async options.</param>
    /// <param name="configuration">Generated configuration.</param>
    /// <param name="executionSettings">Generated execution settings.</param>
    /// <param name="inputs">Generated input sources.</param>
    /// <returns>True if parsing succeeded.</returns>
    public static bool CliGetConfiguration(IList<string> exeName,
        IReadOnlyList<string> args,
        ILogWriter? logWriter,
        bool enableParallel,
        [NotNullWhen(true)] out ProcessorConfiguration? configuration,
        [NotNullWhen(true)] out ExecutionSettings? executionSettings,
        out List<FpInput> inputs)
    {
        logWriter ??= NullLog.Instance;
        configuration = null;
        executionSettings = null;
        inputs = new List<FpInput>();
        List<string> exArgs = new();
        string? outputRootDirectory = null;
        int parallel = 0;
        bool preload = false;
        bool debug = false;
        bool nop = false;
        bool argTime = false;
        for (int i = 0; i < args.Count; i++)
        {
            string str = args[i];
            if (argTime)
            {
                exArgs.Add(str);
                continue;
            }

            if (str.Length == 0) continue;
            if (str[0] != '-')
            {
                inputs.Add(ResolveFpInputForPath(str));
                continue;
            }

            switch (str.Substring(1))
            {
                case "-":
                    argTime = true;
                    break;
                case "d":
                case "-debug":
                    debug = true;
                    break;
                case "m":
                case "-multithread":
                    if (!enableParallel)
                    {
                        logWriter.WriteWarning($"Multithreading is not currently supported, ignoring switch {str}");
                        break;
                    }

                    string? arg = GetArgValue(args, i);
                    if (arg == null)
                    {
                        logWriter.WriteError($"No argument specified for switch {str}, requires int");
                        return false;
                    }

                    if (!int.TryParse(arg, out int maxParallelRes))
                    {
                        logWriter.WriteError($"Switch {str} requires int, got {arg}");
                        return false;
                    }

                    if (maxParallelRes < 1)
                    {
                        logWriter.WriteError($"Switch {str} requires value >= 1, got {maxParallelRes}");
                        return false;
                    }

                    parallel = maxParallelRes;
                    i++;
                    break;
                case "n":
                case "-nop":
                    nop = true;
                    break;
                case "o":
                case "-outdir":
                    outputRootDirectory = GetArgValue(args, i);
                    i++;
                    break;
                case "p":
                case "-preload":
                    preload = true;
                    break;
                default:
                    logWriter.WriteError($"Unknown switch {str}");
                    return false;
            }
        }

        if (inputs.Count == 0)
        {
            var sb = new StringBuilder(exeName[0]);
            foreach (string str in exeName.Skip(1))
                sb.Append(' ').Append(str);

            var sb2 = new StringBuilder();
            AppendProcessorInfos(sb2, FsProcessor.Registered.Factories.Select(static v => v.Info));

            logWriter.WriteInformation(@$"Usage:
    {sb} <inputs...> [options/flags] [-- [args...]]

{sb2}
Parameters:
    inputs           : Input files/directories.
    args             : Arguments for processor. (Optional)

Options:
    -m|--multithread : Use specified # of workers
    -o|--outdir      : Output directory

Flags:
    -d|--debug       : Enable debug
    -n|--nop         : No outputs
    -p|--preload     : Load all streams to memory
");
            return false;
        }

        if (outputRootDirectory == null)
        {
            string commonInput = inputs[0].DirectoryPath;
            outputRootDirectory =
                Path.Combine(
                    inputs.Any(input => commonInput != input.DirectoryPath || commonInput == input.Path)
                        ? Path.GetFullPath(".")
                        : commonInput,
                    DefaultOutputFolderName);
        }

        configuration = new ProcessorConfiguration(exArgs, preload, debug, nop, logWriter);
        executionSettings = new ExecutionSettings(outputRootDirectory, parallel);
        return true;
    }

    /// <summary>
    /// Processes filesystem tree using command-line argument inputs.
    /// </summary>
    /// <param name="args">Command-line argument.s.</param>
    /// <param name="exeName">Executable name.</param>
    /// <param name="fileSystem">Filesystem to read from.</param>
    /// <param name="logWriter">Log output target.</param>
    /// <returns>A task that will execute recursively.</returns>
    /// <exception cref="ArgumentException">Thrown if an invalid number of arguments is provided.</exception>
    public static void CliRunFilesystem<T>(string[] args,
        IList<string>? exeName = null,
        ILogWriter? logWriter = null,
        FileSystemSource? fileSystem = null) where T : FsProcessor, new() =>
        CliRunFilesystem(args, exeName, logWriter, fileSystem, [FsProcessor.GetFsFactory<T>()]);

    /// <summary>
    /// Processes filesystem tree using command-line argument inputs.
    /// </summary>
    /// <param name="exeName">Executable name.</param>
    /// <param name="args">Command-line arguments.</param>
    /// <param name="fileSystem">Filesystem to read from.</param>
    /// <param name="processorFactories">Functions that create new processor instances.</param>
    /// <param name="logWriter">Log output target.</param>
    /// <returns>A task that will execute recursively.</returns>
    /// <exception cref="ArgumentException">Thrown if an invalid number of arguments is provided.</exception>
    public static void CliRunFilesystem(
        string[] args,
        IList<string>? exeName,
        ILogWriter? logWriter,
        FileSystemSource? fileSystem,
        IReadOnlyList<FsProcessorFactory> processorFactories)
    {
        exeName ??= GuessExe(args);
        logWriter ??= ConsoleLog.Default;
        if (!CliGetConfiguration(exeName, args, logWriter, false, out ProcessorConfiguration? conf,
                out ExecutionSettings? exec, out var inputs)) return;
        CliRunFilesystem(conf, exec, inputs, fileSystem, processorFactories);
    }

    /// <summary>
    /// Processes filesystem tree.
    /// </summary>
    /// <param name="processorConfiguration">Processor configuration.</param>
    /// <param name="executionSettings">Execution settings.</param>
    /// <param name="inputs">Inputs.</param>
    /// <param name="fileSystem">Filesystem to read from.</param>
    /// <param name="processorFactories">Functions that create new processor instances.</param>
    /// <returns>A task that will execute recursively.</returns>
    /// <exception cref="ArgumentException">Thrown if an invalid number of arguments is provided.</exception>
    public static void CliRunFilesystem(
        ProcessorConfiguration processorConfiguration,
        ExecutionSettings executionSettings,
        IReadOnlyList<FpInput> inputs,
        FileSystemSource? fileSystem,
        IReadOnlyList<FsProcessorFactory> processorFactories)
    {
        fileSystem ??= FileSystemSource.Default;
        switch (executionSettings.Parallel)
        {
            case 0:
                Recurse(inputs, new ExecutionSource(processorConfiguration, executionSettings, fileSystem), processorFactories);
                break;
            default:
                RecurseAsync(inputs, new ExecutionSource(processorConfiguration, executionSettings, fileSystem), processorFactories).Wait();
                break;
        }
    }

    /// <summary>
    /// Processes filesystem tree using command-line argument inputs.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <param name="exeName">Executable name.</param>
    /// <param name="fileSystem">Filesystem to read from.</param>
    /// <param name="logWriter">Log output target.</param>
    /// <returns>A task that will execute recursively.</returns>
    /// <exception cref="ArgumentException">Thrown if an invalid number of arguments is provided.</exception>
    public static async Task CliRunFilesystemAsync<T>(string[] args,
        IList<string>? exeName = null,
        ILogWriter? logWriter = null,
        FileSystemSource? fileSystem = null) where T : FsProcessor, new() =>
        await CliRunFilesystemAsync(args, exeName, logWriter, fileSystem, [FsProcessor.GetFsFactory<T>()]);

    /// <summary>
    /// Processes filesystem tree using command-line argument inputs.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <param name="exeName">Executable name.</param>
    /// <param name="fileSystem">Filesystem to read from.</param>
    /// <param name="processorFactories">Functions that create new processor instances.</param>
    /// <param name="logWriter">Log output target.</param>
    /// <returns>A task that will execute recursively.</returns>
    /// <exception cref="ArgumentException">Thrown if an invalid number of arguments is provided.</exception>
    public static Task CliRunFilesystemAsync(string[] args,
        IList<string>? exeName,
        ILogWriter? logWriter,
        FileSystemSource? fileSystem,
        IReadOnlyList<FsProcessorFactory> processorFactories)
    {
        exeName ??= GuessExe(args);
        logWriter ??= ConsoleLog.Default;
        if (!CliGetConfiguration(exeName, args, logWriter, true, out ProcessorConfiguration? conf,
                out ExecutionSettings? exec, out var inputs)) return Task.CompletedTask;
        return CliRunFilesystemAsync(conf, exec, inputs, fileSystem, processorFactories);
    }

    /// <summary>
    /// Processes filesystem tree.
    /// </summary>
    /// <param name="processorConfiguration">Processor configuration.</param>
    /// <param name="executionSettings">Execution settings.</param>
    /// <param name="inputs">Inputs.</param>
    /// <param name="fileSystem">Filesystem to read from.</param>
    /// <param name="processorFactories">Functions that create new processor instances.</param>
    /// <returns>A task that will execute recursively.</returns>
    /// <exception cref="ArgumentException">Thrown if an invalid number of arguments is provided.</exception>
    public static async Task CliRunFilesystemAsync(
        ProcessorConfiguration processorConfiguration,
        ExecutionSettings executionSettings,
        IReadOnlyList<FpInput> inputs,
        FileSystemSource? fileSystem,
        IReadOnlyList<FsProcessorFactory> processorFactories)
    {
        fileSystem ??= FileSystemSource.Default;
        switch (executionSettings.Parallel)
        {
            case 0:
                // ReSharper disable once MethodHasAsyncOverload
                Recurse(inputs, new ExecutionSource(processorConfiguration, executionSettings, fileSystem), processorFactories);
                break;
            default:
                await RecurseAsync(inputs, new ExecutionSource(processorConfiguration, executionSettings, fileSystem), processorFactories);
                break;
        }
    }

    /// <summary>
    /// Guesses executable string (might be multiple components) based on args.
    /// </summary>
    /// <param name="args">Arguments to check.</param>
    /// <param name="prependDotNetIfDll">If the first element ends in .dll, prepend dotnet as an element.</param>
    /// <returns>Executable command sequence.</returns>
    /// <remarks>
    /// Just matches up the tail and sends the rest, fallback on argv[0].
    /// </remarks>
    public static IList<string> GuessExe(IList<string>? args,
        bool prependDotNetIfDll = true)
    {
        var list = GuessExeCore(args);
        if (list[0].EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) && prependDotNetIfDll)
            list.Insert(0, "dotnet");
        return list;
    }

    #endregion

    #region General execution

    /// <summary>
    /// Processes filesystem tree asynchronously.
    /// </summary>
    /// <param name="inputs">Input sources.</param>
    /// <param name="src">Execution source.</param>
    /// <param name="processorFactories">Functions that create new processor instances.</param>
    /// <returns>Task that will execute recursively.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="processorFactories"/> is empty or <paramref name="src"/>.<see cref="ExecutionSource.Exec"/>.<see cref="ExecutionSettings.Parallel"/> is less than 1.</exception>
    public static async Task RecurseAsync(IReadOnlyList<FpInput> inputs,
        ExecutionSource src,
        IReadOnlyList<FsProcessorFactory> processorFactories)
    {
        InitializeProcessors(src.Exec, processorFactories, out var processors, out int baseCount, out int parallelCount);
        SeedInputs(inputs, out var dQueue, out var fQueue);
        Dictionary<Task, int> tasks = new();
        src.FileSystem.ParallelAccess = true;
        while (fQueue.Count != 0 || dQueue.Count != 0)
            if (fQueue._TryDequeue(out var deq))
                for (int iBase = 0; iBase < baseCount; iBase++)
                {
                    while (tasks.Count >= parallelCount) tasks.Remove(await Task.WhenAny(tasks.Keys));
                    int workerId = Enumerable.Range(0, parallelCount).Except(tasks.Values).First();
                    FsProcessor processor = processors[workerId * parallelCount + iBase];
                    if (!processor.AcceptFile(deq.TargetPath)) continue;
                    tasks.Add(Task.Run(() => Run(processor, deq, src, workerId)), workerId);
                }
            else
                GetMoreInputs(src.FileSystem, dQueue, fQueue);

        await Task.WhenAll(tasks.Keys);
    }

    /// <summary>
    /// Processes filesystem tree.
    /// </summary>
    /// <param name="inputs">Input sources.</param>
    /// <param name="src">Execution source.</param>
    /// <param name="processorFactories">Functions that create new processor instances.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="processorFactories"/> is empty or <paramref name="src"/>.<see cref="ExecutionSource.Exec"/>.<see cref="ExecutionSettings.Parallel"/> is less than 1.</exception>
    public static void Recurse(IReadOnlyList<FpInput> inputs,
        ExecutionSource src,
        IReadOnlyList<FsProcessorFactory> processorFactories)
    {
        if (src.Exec.Parallel != 0)
            throw new ArgumentException($"Cannot start synchronous operation with {nameof(src.Exec.Parallel)} value of {src.Exec.Parallel}, use {nameof(Coordinator)}.{nameof(RecurseAsync)} instead");
        InitializeProcessors(src.Exec, processorFactories, out var processors, out int baseCount, out _);
        SeedInputs(inputs, out var dQueue, out var fQueue);
        while (fQueue.Count != 0 || dQueue.Count != 0)
            if (fQueue._TryDequeue(out var deq))
                for (int iBase = 0; iBase < baseCount; iBase++)
                {
                    var processor = processors[iBase];
                    if (!processor.AcceptFile(deq.TargetPath)) continue;
                    var res = Run(processor, deq, src, iBase);
                    if (res.Locked) break;
                }
            else
                GetMoreInputs(src.FileSystem, dQueue, fQueue);
    }

    /// <summary>
    /// Operates on a file.
    /// </summary>
    /// <param name="processor">Processor to operate with.</param>
    /// <param name="source">Source info.</param>
    /// <param name="src">Execution source.</param>
    /// <param name="workerId">Worker ID.</param>
    /// <returns>Processing result.</returns>
    public static ProcessResult Run(FsProcessor processor,
        FpTarget source,
        ExecutionSource src,
        int workerId)
    {
        try
        {
            processor.Cleanup();
            processor.Prepare(src.FileSystem, source.InputRootPath, src.Exec.OutputRootDirectory, source.TargetPath, src.Config, workerId);
            bool success;
            if (processor.Debug)
            {
                processor.Process();
                success = true;
            }
            else
            {
                try
                {
                    processor.Process();
                    success = true;
                }
                catch (Exception e)
                {
                    src.Config.LogWriter?.WriteError($"Exception occurred during processing:\n{e}");
                    success = false;
                }
            }

            return new ProcessResult(success, processor.Lock);
        }
        finally
        {
            processor.Cleanup();
        }
    }

    /// <summary>
    /// Operate on a file using segmented operation.
    /// </summary>
    /// <param name="processor">Processor to operate with.</param>
    /// <param name="input">Source info.</param>
    /// <param name="src">Execution source.</param>
    /// <param name="workerId">Worker ID.</param>
    /// <returns>Processing results.</returns>
    public static IEnumerable<Data> RunSegmented(FsProcessor processor,
        (string inputRoot, string file) input,
        ExecutionSource src,
        int workerId)
    {
        try
        {
            processor.Cleanup();
            processor.Prepare(src.FileSystem, input.inputRoot, src.Exec.OutputRootDirectory, input.file, src.Config, workerId);
            if (processor.Debug)
            {
                return processor.ProcessSegmented();
            }
            else
            {
                try
                {
                    return processor.ProcessSegmented();
                }
                catch (Exception e)
                {
                    src.Config.LogWriter?.WriteError($"Exception occurred during processing:\n{e}");
                    return Enumerable.Empty<Data>();
                }
            }
        }
        finally
        {
            processor.Cleanup();
        }
    }

    #endregion

    #region Internals

    private static List<string> GuessExeCore(IList<string>? args)
    {
        if (args == null) return new List<string> { DefaultCurrentExecutableName };
        string[] oargs = Environment.GetCommandLineArgs();
        int i = 0;
        while (i < args.Count && i < oargs.Length)
        {
            if (args[args.Count - i - 1] != oargs[oargs.Length - i - 1])
                return new List<string> { DefaultCurrentExecutableName };
            i++;
        }

        i = oargs.Length - i;
        return i > 0
            ? new List<string>(new ArraySegment<string>(oargs, 0, i))
            : new List<string> { DefaultCurrentExecutableName };
    }

    private static string? GetArgValue(IReadOnlyList<string> args,
        int cPos) =>
        cPos + 1 >= args.Count ? null : args[cPos + 1];

    private static void InitializeProcessors(ExecutionSettings exec,
        IReadOnlyList<FsProcessorFactory> processorFactories,
        out FsProcessor[] processors,
        out int baseCount,
        out int parallelCount)
    {
        if (processorFactories.Count == 0)
            throw new ArgumentException("Cannot start operation with 0 provided processors");
        if (exec.Parallel < 0)
            throw new ArgumentException(
                $"Illegal {nameof(exec.Parallel)} value of {exec.Parallel}");
        parallelCount = Math.Min(TaskScheduler.Current.MaximumConcurrencyLevel,
            Math.Max(1, exec.Parallel));
        baseCount = processorFactories.Count;
        processors = new FsProcessor[parallelCount * baseCount];
        for (int iParallel = 0; iParallel < parallelCount; iParallel++)
        for (int iBase = 0; iBase < baseCount; iBase++)
            processors[iParallel * baseCount + iBase] = processorFactories[iBase].CreateProcessor();
    }

    private static void SeedInputs(IEnumerable<FpInput> inputs,
        out Queue<FpTarget> dQueue,
        out Queue<FpTarget> fQueue)
    {
        dQueue = new Queue<FpTarget>();
        fQueue = new Queue<FpTarget>();
        foreach ((bool isFile, string dir, string item) in inputs)
            (isFile ? fQueue : dQueue).Enqueue(new FpTarget(dir, item));
    }

    private static void GetMoreInputs(FileSystemSource fileSystem,
        Queue<FpTarget> dQueue,
        Queue<FpTarget> fQueue)
    {
        (string inputRoot, string curDir) = dQueue.Dequeue();
        if (!fileSystem.DirectoryExists(curDir)) return;
        foreach (string file in fileSystem.EnumerateFiles(curDir))
            fQueue.Enqueue(new FpTarget(inputRoot, file));
        foreach (string folder in fileSystem.EnumerateDirectories(curDir))
            dQueue.Enqueue(new FpTarget(inputRoot, folder));
    }

    private static bool _TryDequeue<T>(this Queue<T> queue,
        [NotNullWhen(true)] out T? result)
    {
        if (queue.Count != 0)
        {
            result = queue.Dequeue()!;
            return true;
        }

        result = default;
        return false;
    }

    #endregion

    #region Types

    /// <summary>
    /// Settings for execution.
    /// </summary>
    /// <param name="OutputRootDirectory">Output source.</param>
    /// <param name="Parallel">Thread count.</param>
    public record ExecutionSettings(string OutputRootDirectory, int Parallel);

    /// <summary>
    /// FP input.
    /// </summary>
    /// <param name="IsFile">True if this is a file.</param>
    /// <param name="DirectoryPath">Path to parent (or root path if <see cref="Path"/> is itself root).</param>
    /// <param name="Path">Path to target.</param>
    public readonly record struct FpInput(bool IsFile, string DirectoryPath, string Path);

    /// <summary>
    /// FP target.
    /// </summary>
    /// <param name="InputRootPath">Input root path.</param>
    /// <param name="TargetPath">Target path.</param>
    public readonly record struct FpTarget(string InputRootPath, string TargetPath);

    /// <summary>
    /// Represents execution config.
    /// </summary>
    public record ExecutionSource(ProcessorConfiguration Config, ExecutionSettings Exec, FileSystemSource FileSystem);

    #endregion
}
