using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Fp.Fs;

/// <summary>
/// Base type for file processors.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
[SuppressMessage("ReSharper", "NotAccessedField.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class FsProcessor : FileProcessor
{
    #region Properties and fields

    /// <summary>
    /// Currently running <see cref="FsProcessor"/> on this thread.
    /// </summary>
    public static FsProcessor Current => s_current ?? throw new InvalidOperationException();

    /// <summary>
    /// Currently running <see cref="FsProcessor"/> on this thread, or null.
    /// </summary>
    public static FsProcessor? NullableCurrent => s_current;

    [ThreadStatic] internal static FsProcessor? s_current;

    /// <summary>
    /// Per-thread instance.
    /// </summary>
    public static FsProcessor FsInstance => s_fsInstance ??= new FsProcessor { LogWriter = ConsoleLog.Default };

    [ThreadStatic] private static FsProcessor? s_fsInstance;

    /// <summary>
    /// Hint for segmented processor to perform dry-run.
    /// </summary>
    public bool Dry;

    /// <summary>
    /// ID of worker thread processor is using.
    /// </summary>
    public int WorkerId;

    /// <summary>
    /// Root input directory.
    /// </summary>
    public string InputRootDirectory = null!;

    /// <summary>
    /// Current input directory.
    /// </summary>
    public string InputDirectory = null!;

    /// <summary>
    /// Root output directory.
    /// </summary>
    public string OutputRootDirectory = null!;

    /// <summary>
    /// Current output directory.
    /// </summary>
    public string OutputDirectory = null!;

    /// <summary>
    /// Current output file index.
    /// </summary>
    public int OutputCounter;

    /// <summary>
    /// Filesystem provider for this processor.
    /// </summary>
    public FileSystemSource FileSystem = null!;

    /// <summary>
    /// Set by subclasses to indicate if no more processors
    /// should be run on the current input file.
    /// </summary>
    /// <remarks>
    /// Not applicable in a multithreaded environment.
    /// </remarks>
    public bool Lock;

    /// <summary>
    /// Current file path.
    /// </summary>
    public FpPath FilePath => FpPath.GetFromString(InputFile) ?? throw new InvalidOperationException();

    /// <summary>
    /// Current file path without extension.
    /// </summary>
    public FpPath FilePathNoExt => new(Path.GetFileNameWithoutExtension(InputFile), InputDirectory);

    private bool _overrideProcess = true;
    private bool _overrideProcessSegmented = true;

    #endregion

    #region Main operation functions

    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="fileSystem">Filesystem source.</param>
    /// <param name="inputRoot">Input root directory.</param>
    /// <param name="outputRoot">Output root directory.</param>
    /// <param name="file">Input file.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <param name="workerId">Worker ID.</param>
    public void Prepare(FileSystemSource fileSystem, string inputRoot, string outputRoot, string file,
        ProcessorConfiguration? configuration = null, int workerId = 0)
    {
        Prepare(configuration);
        InputRootDirectory = fileSystem.NormalizePath(inputRoot);
        InputFile = fileSystem.NormalizePath(Path.Combine(InputRootDirectory, file));
        InputDirectory = Path.GetDirectoryName(InputFile) ?? throw new ArgumentException("File is root");
        OutputRootDirectory = fileSystem.NormalizePath(outputRoot);
        OutputDirectory = fileSystem.NormalizePath(Join(false,
            OutputRootDirectory, InputDirectory.Substring(InputRootDirectory.Length)));
        ValidateExtension(InputFile, out SelectedExtension);
        OutputCounter = 0;
        FileSystem = fileSystem;
        WorkerId = workerId;
    }

    /// <summary>
    /// Initializes a new file processor with an isolated filesystem containing the specified binary files.
    /// </summary>
    /// <param name="main">Main file.</param>
    /// <param name="args">Arguments.</param>
    /// <param name="additionalFiles">Additional files to pass to processor.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>Created processor.</returns>
    public T Initialize<T>(BufferData<byte> main, string[]? args = null,
        IEnumerable<BufferData<byte>>? additionalFiles = null) where T : FsProcessor, new()
    {
        T child = new();
        IEnumerable<BufferData<byte>> seq = new[] { main };
        if (additionalFiles != null) seq = seq.Concat(additionalFiles);
        var layer1 = new SegmentedFileSystemSource(FileSystem, true, seq);
        child.Prepare(layer1, InputRootDirectory, OutputRootDirectory, main.BasePath,
            new ProcessorConfiguration(args ?? Array.Empty<string>(), Preload, Debug, Nop, LogWriter));
        return child;
    }

    /// <summary>
    /// Processes current file.
    /// </summary>
    protected virtual void ProcessImpl() => _overrideProcess = false;

    /// <summary>
    /// Processes current file in parts.
    /// </summary>
    /// <returns>Generated outputs.</returns>
    protected virtual IEnumerable<Data> ProcessSegmentedImpl()
    {
        _overrideProcessSegmented = false;
        yield break;
    }

    /// <summary>
    /// Processes current file.
    /// </summary>
    public void Process()
    {
        ShieldProcess();
        if (_overrideProcess) return;
        foreach (Data d in ShieldProcessSegmented())
        {
            if (Nop || d is MetaData && !Debug) continue;
            using Data data = d;
            using Stream stream =
                (FileSystem ?? throw new InvalidOperationException()).OpenWrite(GenPath(data.GetExtension(),
                    data.BasePath));
            data.WriteConvertedData(stream, data.DefaultFormat);
        }
    }

    /// <summary>
    /// Processes current file in parts.
    /// </summary>
    /// <returns>Generated outputs.</returns>
    public IEnumerable<Data> ProcessSegmented()
    {
        foreach (Data entry in ShieldProcessSegmented()) yield return entry;
        if (_overrideProcessSegmented) yield break;
        FileSystemSource prevFs = FileSystem ?? throw new InvalidOperationException();
        SegmentedFileSystemSource fs = new(prevFs, false);
        FileSystem = fs;
        try
        {
            ShieldProcess();
            foreach ((string path, Stream stream) in fs)
                yield return new BufferData<byte>(path, GetMemory(stream));
        }
        finally
        {
            FileSystem = prevFs;
        }
    }

    /// <summary>
    /// Sets <see cref="Current"/>.
    /// </summary>
    public void ShieldUp() => s_current = this;

    /// <summary>
    /// Unsets <see cref="Current"/>.
    /// </summary>
    public static void ShieldDown() => s_current = null;

    /// <summary>
    /// Processes current file.
    /// </summary>
    protected void ShieldProcess()
    {
        ShieldUp();
        try
        {
            ProcessImpl();
        }
        finally
        {
            ShieldDown();
        }
    }

    /// <summary>
    /// Processes current file in parts.
    /// </summary>
    /// <returns>Generated outputs.</returns>
    protected IEnumerable<Data> ShieldProcessSegmented()
    {
        try
        {
            ShieldUp();
            using var enumerator = ProcessSegmentedImpl().GetEnumerator();
            ShieldDown();
            bool has;
            do
            {
                ShieldUp();
                try
                {
                    has = enumerator.MoveNext();
                }
                finally
                {
                    ShieldDown();
                }
                if (has)
                {
                    yield return enumerator.Current;
                }
            } while (has);
        }
        finally
        {
            ShieldDown();
        }
    }

    #endregion

    #region Factory utilities

    /// <summary>
    /// Creates a factory for the specified type, obtaining information from applied <see cref="FsProcessorInfoAttribute"/> if possible.
    /// </summary>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>Processor factory.</returns>
    public static FsProcessorFactory GetFsFactory<T>() where T : FsProcessor, new()
    {
        FileProcessorInfo? processorInfo = null;
        try
        {
            object[] attrs = typeof(T).GetCustomAttributes(typeof(FsProcessorInfoAttribute), true);
            if (attrs.FirstOrDefault() is FsProcessorInfoAttribute attr) processorInfo = attr.Info;
        }
        catch
        {
            // When reflection doesn't work, just fallback
        }

        return new GenericNewFsProcessorFactory<T>(processorInfo);
    }

    #endregion
}
