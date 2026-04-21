using System;
using System.Collections.Generic;
using System.Linq;

namespace Fp.Fs;

// ReSharper disable InconsistentNaming
public partial class FsProcessor
{
    /// <summary>
    /// Registered scripting processors.
    /// </summary>
    public static readonly FsProcessorSource Registered = new();
}
