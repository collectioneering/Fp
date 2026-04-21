namespace Fp.Fs.CommandLine;

/// <summary>
/// Settings for execution.
/// </summary>
/// <param name="OutputRootDirectory">Output source.</param>
/// <param name="Parallel">Thread count.</param>
public record FsExecutionSettings(string OutputRootDirectory, int Parallel);
