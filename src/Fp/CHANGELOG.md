# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## Unreleased

## [0.28.0] 2026-02-03

### Added

- `public static long BitUtil.GetBitsForBytes(long bytes)`

- Added intermediary types `BaseUnmanagedIntegerHelper<T>` and `BaseUnmanagedIntegerArrayHelper<T>` for helpers to provide additional functionality

- `Processor.TryRead` overloads for better C# pattern

- `FpUtil.Ascii(string, Span<byte>)`

### Changed

- Target TFMs are now `netstandard2.1`, `net8.0`, `net10.0`
- Deprecated array-based `Processor.Read` / `Processor.Write` etc. overloads
- `public void Processor.OutputAll(ReadOnlySpan<byte> span, Stream? outputStream = null)` changed to `public void Processor.OutputAll(ReadOnlySpan<byte> span)` for minimal utility gain
- `public static int BitUtil.GetBitsForBytes(int bytes)` throws when an overflow is detected
- Renamed `forceNew` parameter on `ref Span` `Processor.Read` overloads to `forceUseCurrentSpan` for better clarity

### Fixed

- `Processor.Read` leniency behaviour when performing operations on MemoryStream

### Removed

- Internal member `Processor.WriteBaseSpan` - not necessary for all targets

## [0.27.1] 2022-09-06

### Changed

- Updated README

## [0.27.0] 2022-09-06

### Added

- `MemAnnotation` record struct for memory annotations
- `Processor.DecodeHex(ReadOnlySpan<char>, Span<byte>, bool)` overload
- `Processor.GetHexByteCount(ReadOnlySpan<char>, out int)`
- `Processor.Initialize(ProcessorConfiguration)` overload
- `Processor.LogChunk(string, bool)`
- `string StringBuilderLog.Delimiter { get; set; }` for control over default line delimiter
- `StringBuilderLog.GetContent()` / `StringBuilderLog.Clear()`
- `IChunkWriter`
- Vectorized (System.Numerics) bitwise operations

### Changed

- `StringData` is now a record struct
- `DataUtil` renamed to `BufferExtensions`
- `Processor.DecodeHex(string, bool)` now uses `ReadOnlySpan<char>` instead of `string`
- `ProcessorConfiguration` constructor parameter order adjusted with defaults for most args
- `ILogReceiver ProcessorConfiguration.LogReceiver` is now `ILogWriter? ProcessorConfiguration.LogWriter`
- `Processor` now uses `NullLog.Instance` as default log receiver
- `Processor` now uses `Array.Empty<string>()` as default args
- Hex writing now uses `IChunkWriter` interface type
- `ILogReceiver.LogLevel` moved to outer scope
- `ILogReceiver` renamed to `ILogWriter` with base interface `IChunkWriter`
- `Processor.MemClear()` always clears annotations
- `Processor.MemPrint(ReadOnlyMemory<byte>,bool,bool)` added `int displayWidth` parameter

### Fixed

- Fixed vectorized Bitwise OR on single byte

## [0.26.0] 2022-01-30

### Added

- FileProcessorInfo (moved from Fp.Fs FsProcessorInfo)

### Changed

- Arg setup now happens in `Processor.Prepare(ProcessorConfiguration?)`
- FormatProcessor is now derived from FileProcessor
- Adjusted FormatProcessor members (moved commons to FileProcessor)

## [0.25.0] 2022-01-18

### Added

- Additional filename members in FormatProcessor

### Changed

- FormatProcessor.Name now FormatProcessor.InputFile (parity with FsProcessor)

### Removed

- ProcessorException
- Scripting class
- Filesystem-related code (moved to Fp.Fs)

## [0.24.0] 2022-01-16

### Added

- WriteContext extensions in ReadWriteContextByteExtensions
- `WriteContext<byte>.GetAdvance(int)` to claim buffer
- Clearing of stored singletons via `One<T>.Clear` / and `One.ClearAll`
- `StringData` type for data containing string
- `Data.SupportsFormat` and `Data.SupportedFormats` for pre-checking output support
- `FormatProcessor` / `FormatSingleProcessor` / `FormatMultiProcessor` for pure file-based operation
- `Processor.Configuration` for sharing

### Changed

- Fix xor arm intrinsic
- Initialize Processor.LittleEndian to true in constructor
- Use local variables in static Processor.GetX/SetX
- Moved Data.CastNumber to FpUtil
- Moved Processor.PaddingMode to CipherPaddingMode
- Moved ReadContextByteExtensions to ReadWriteContextByteExtensions
- Use ReadOnlySpan param on WriteContext.WriteAdvance
- Moved `T FsProcessor.Initialize<T>(string[]? args)` to Processor

### Removed

- `DataProcessor` / `LayeredDataProcessor` / `ProcessorChild`

## [0.23.2] 2022-01-11

### Changed

- Fix intro printing on Windows

## [0.23.1] 2021-12-27

### Changed

- Fix parallel check in InitializeProcessors

## [0.23.0] 2021-12-27

### Changed

- Update xmldoc
- Parallel value 0 (default) corresponds to synchronous execution, otherwise interpret as async of n workers
- Always dispose stream for parallel-mode FSS read

## [0.22.0] 2021-12-17

### Changed

- Miscellaneous API changes
- Internal fixes
- Changed RealFileSystemSource to public
- Fixed xml documentation standards
- Adjusted all filesystem-related operation to use FsProcessor
- Fixed extraneous newlines on print
- Fixed incorrect documentation on AlignUp/AlignDown

## [0.21.1] 2021-11-09

### Changed

- Fixed console output on Windows

## [0.21.0] 2021-11-09

### Changed

- Updated / fixed Coordinator API
- Add length-based precedence to `ValidateExtension`
- Respect endianness on unmanaged helpers for `ReadOnlySpan<byte>`
- Changed `net5.0` target to `net6.0`
