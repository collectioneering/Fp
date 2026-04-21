namespace Fp.Fs.CommandLine;

/// <summary>
/// Provides methods for executing command-line programs.
/// </summary>
public static partial class CommandLineFsProcessor
{
    /// <summary>
    /// Keyword arg for stopping cli execution.
    /// </summary>
    public const string NO_EXECUTE_CLI = "--no-execute-cli";

    /// <summary>
    /// Keyword arg for registering processors known to cli to <see cref="FsProcessor.Registered"/>.
    /// </summary>
    public const string REGISTER_PROCESSORS = "--register-processors";
}
