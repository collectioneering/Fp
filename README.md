[![.NET](https://github.com/collectioneering/Fp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/collectioneering/Fp/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/Fp.svg)](https://www.nuget.org/packages/Fp/)

## File processing

| Package              | Release                                                                                                               |
|----------------------|-----------------------------------------------------------------------------------------------------------------------|
| `Fp`                 | [![NuGet](https://img.shields.io/nuget/v/Fp.svg)](https://www.nuget.org/packages/Fp/)                                 |
| `Fp.Fs`              | [![NuGet](https://img.shields.io/nuget/v/Fp.Fs.svg)](https://www.nuget.org/packages/Fp.Fs/)                           |
| `Fp.Plus`            | [![NuGet](https://img.shields.io/nuget/v/Fp.Plus.svg)](https://www.nuget.org/packages/Fp.Plus/)                       |
| `Fp.Platforms.Nitro` | [![NuGet](https://img.shields.io/nuget/v/Fp.Platforms.Nitro.svg)](https://www.nuget.org/packages/Fp.Platforms.Nitro/) |
| `Fp.Templates`       | [![NuGet](https://img.shields.io/nuget/v/Fp.Templates.svg)](https://www.nuget.org/packages/Fp.Templates/)             |

[Documentation](https://collectioneering.github.io/Fp) | [Samples](https://github.com/collectioneering/Fp/tree/main/samples)

Fp is designed as a framework for implementing concise data format extractors, primarily for archive containers and 2D rasters.

Many utilities for working with primitive data (including endianness-dependent I/O) are provided. See the [Samples](https://github.com/collectioneering/Fp/tree/main/samples) for more.

### Libraries
* Fp: Base file processing library
* Fp.Fs: Filesystem processing library
* Fp.Plus: Extension library (e.g. RGBA32 image (through ImageSharp) / PCM WAVE output)
* Fp.Platforms.Nitro: Nintendo DS file format integrations

### Scripting
* fpx: Script execution program (thin wrapper of [dotnet-script](https://github.com/filipw/dotnet-script))
    - Requires [.NET 10 SDK](https://get.dot.net/) for execution
* Dereliction: Basic Avalonia GUI script editor / testing program
    - Requires [.NET 10 SDK](https://get.dot.net/) for execution

### Details

Filesystem processors are a unit worker derived from `Fp.Fs.FsProcessor` or one of its descendants that are called to operate on each applicable (by default, based on extension) file path, optionally opening a stream and generating console output or file artifacts. If written to only use compatible APIs (e.g. working with the `FileSystemSource` APIs and not directly using `System.IO.File` etc.), processors can be used to work with synthetic filesystems (e.g. directly working on files in a zip file) and more generally be used to efficiently interact with any binary data regardless of source.

File format processors are a unit worker derived from `Fp.FormatSingleProcessor`, `Fp.FormatMultiProcessor`, or one of their descendants. They can be used to efficiently obtain converted data from individual files, or be wrapped with `FormatSingleProcessorFsWrapper` / `FormatMultiProcessorFsWrapper` to be used as a `FsProcessor`.

Programs intended to extract from a set of files / folders (recursively) to a destination can utilize the default `FsProcessor.Run<T>` function where `T` is a user-created processor class. All file I/O is managed by the library, the processor will be automatically fed input file paths and can opt to open these files / create new files (automatically placed in the output directory) via the `Processor` class APIs.

Processors can be designed to generate a `System.Collections.Generic.IEnumerable<Fp.Data>` where `Data` represents an arbitrary content result with an associated filename, such as `Fp.Plus.Images.Rgba32Data` for an RGBA 32-bit image, or `Fp.BufferData<byte>` for an arbitrary byte buffer. This can be a useful pattern for designing an arbitrary import scheme for software that needs to be able to work with new file types without unnecessary pipeline changes.

