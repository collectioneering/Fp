# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.22.0] 2021-12-17
### Changed
- Miscellaneous API changes
- Internal fixes
- Changed RealFileSystemSource to public
- Fixed xml documentation standards
- Adjusted all filesystem-related operation to use FsProcessor
- Fixed extraneous newlines on print
- Fixed incorrect documentation on AlignUp/AlignDown

## [0.21.1] 2021-11-9
### Changed
- Fixed console output on Windows

## [0.21.0] 2021-11-9
### Changed
- Updated / fixed Coordinator API
- Add length-based precedence to `ValidateExtension`
- Respect endianness on unmanaged helpers for `ReadOnlySpan<byte>`
- Changed `net5.0` target to `net6.0`