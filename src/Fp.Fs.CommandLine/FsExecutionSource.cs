namespace Fp.Fs.CommandLine;

/// <summary>
/// Represents execution config.
/// </summary>
public record FsExecutionSource(ProcessorConfiguration Config, FsExecutionSettings Exec, FileSystemSource FileSystem);
