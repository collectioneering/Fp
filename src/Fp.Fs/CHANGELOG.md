# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.28.0] 2026-02-03

### Changed

- Target TFMs are now `netstandard2.1`, `net8.0`, `net10.0`

## [0.27.1] 2022-09-06

### Changed

- Updated README

## [0.27.0] 2022-09-06

### Changed

- Updated Fp dependency to 0.27.0

## [0.26.0] 2022-01-30

### Added

- Add FileProcessorInfo-accepting Run overloads for FsFormatSingle/FsFormatMulti/Fs

### Changed

- Updated Fp dependency to 0.26.0
- Arg setup now happens in `Processor.Prepare(ProcessorConfiguration?)`
- Format processor wrappers now run cleanup on the wrapped processors
- FsProcessor is now derived from FileProcessor
- Adjusted FsProcessor members (moved commons to FileProcessor, Source removed, supplanted by Info)

### Removed

- FsProcessorInfo (moved to Fp as FileProcessorInfo)

## [0.25.0] 2022-01-18

- Initial version
