using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Fp.Fs.CommandLine;

/// <summary>
/// Execution manager.
/// </summary>
public static class FsExecutor
{
    #region CLI tools

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
    public static void ExecuteOnFilesystem(
        ProcessorConfiguration processorConfiguration,
        FsExecutionSettings executionSettings,
        IReadOnlyList<FpInput> inputs,
        FileSystemSource? fileSystem,
        IReadOnlyList<FsProcessorFactory> processorFactories)
    {
        fileSystem ??= FileSystemSource.Default;
        switch (executionSettings.Parallel)
        {
            case 0:
                Recurse(inputs, new FsExecutionSource(processorConfiguration, executionSettings, fileSystem), processorFactories);
                break;
            default:
                RecurseAsync(inputs, new FsExecutionSource(processorConfiguration, executionSettings, fileSystem), processorFactories).Wait();
                break;
        }
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
    public static async Task ExecuteOnFilesystemAsync(
        ProcessorConfiguration processorConfiguration,
        FsExecutionSettings executionSettings,
        IReadOnlyList<FpInput> inputs,
        FileSystemSource? fileSystem,
        IReadOnlyList<FsProcessorFactory> processorFactories)
    {
        fileSystem ??= FileSystemSource.Default;
        switch (executionSettings.Parallel)
        {
            case 0:
                // ReSharper disable once MethodHasAsyncOverload
                Recurse(inputs, new FsExecutionSource(processorConfiguration, executionSettings, fileSystem), processorFactories);
                break;
            default:
                await RecurseAsync(inputs, new FsExecutionSource(processorConfiguration, executionSettings, fileSystem), processorFactories);
                break;
        }
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
    /// <exception cref="ArgumentException">Thrown if <paramref name="processorFactories"/> is empty or <paramref name="src"/>.<see cref="FsExecutionSource.Exec"/>.<see cref="FsExecutionSettings.Parallel"/> is less than 1.</exception>
    public static async Task RecurseAsync(IReadOnlyList<FpInput> inputs,
        FsExecutionSource src,
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
    /// <exception cref="ArgumentException">Thrown if <paramref name="processorFactories"/> is empty or <paramref name="src"/>.<see cref="FsExecutionSource.Exec"/>.<see cref="FsExecutionSettings.Parallel"/> is less than 1.</exception>
    public static void Recurse(IReadOnlyList<FpInput> inputs,
        FsExecutionSource src,
        IReadOnlyList<FsProcessorFactory> processorFactories)
    {
        if (src.Exec.Parallel != 0)
            throw new ArgumentException($"Cannot start synchronous operation with {nameof(src.Exec.Parallel)} value of {src.Exec.Parallel}, use {nameof(FsExecutor)}.{nameof(RecurseAsync)} instead");
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
        FsExecutionSource src,
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
    public static IEnumerable<Data> RunSegmented(
        FsProcessor processor,
        (string inputRoot, string file) input,
        FsExecutionSource src,
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

    private static void InitializeProcessors(FsExecutionSettings exec,
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
}
