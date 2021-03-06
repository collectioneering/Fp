using System;

namespace Fp.Fs;

/// <summary>
/// Processor factory.
/// </summary>
public abstract record FsProcessorFactory
{
    /// <summary>
    /// Processor info.
    /// </summary>
    public FileProcessorInfo Info { get; init; }

    /// <summary>
    /// Creates a new instance of <see cref="FsProcessorFactory"/>.
    /// </summary>
    /// <param name="info">Processor info.</param>
    public FsProcessorFactory(FileProcessorInfo? info) => Info = info ?? new FileProcessorInfo();

    /// <summary>
    /// Creates a new processor instance from this factory.
    /// </summary>
    /// <returns>Instantiated processor.</returns>
    public abstract FsProcessor CreateProcessor();
}

/// <summary>
/// Processor descriptor.
/// </summary>
/// <typeparam name="T">Processor type.</typeparam>
public record GenericNewFsProcessorFactory<T> : FsProcessorFactory where T : FsProcessor, new()
{
    /// <summary>
    /// Creates a new instance of <see cref="GenericNewFsProcessorFactory{T}"/>.
    /// </summary>
    /// <param name="info">Processor info.</param>
    public GenericNewFsProcessorFactory(FileProcessorInfo? info) : base(info)
    {
    }

    /// <inheritdoc />
    public override FsProcessor CreateProcessor() => new T { Info = Info };
}

/// <summary>
/// Processor descriptor.
/// </summary>
public record DelegateFsProcessorFactory : FsProcessorFactory
{
    /// <summary>
    /// Source delegate.
    /// </summary>
    public Func<FsProcessor> Delegate { get; init; }

    /// <summary>
    /// Creates a new instance of <see cref="DelegateFsProcessorFactory"/>.
    /// </summary>
    /// <param name="info">Processor info.</param>
    /// <param name="del">Source delegate.</param>
    public DelegateFsProcessorFactory(FileProcessorInfo? info, Func<FsProcessor> del) : base(info) => Delegate = del;

    /// <inheritdoc />
    public override FsProcessor CreateProcessor()
    {
        var re = Delegate();
        re.Info = Info;
        return re;
    }
}
