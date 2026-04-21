namespace Fp.Fs.CommandLine;

/// <summary>
/// FP target.
/// </summary>
/// <param name="InputRootPath">Input root path.</param>
/// <param name="TargetPath">Target path.</param>
public readonly record struct FpTarget(string InputRootPath, string TargetPath);
