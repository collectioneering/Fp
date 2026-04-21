namespace Fp.Fs.CommandLine;

/// <summary>
/// FP input.
/// </summary>
/// <param name="IsFile">True if this is a file.</param>
/// <param name="DirectoryPath">Path to parent (or root path if <see cref="Path"/> is itself root).</param>
/// <param name="Path">Path to target.</param>
public readonly record struct FpInput(bool IsFile, string DirectoryPath, string Path);
