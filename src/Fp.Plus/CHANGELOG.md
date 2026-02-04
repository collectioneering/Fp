# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.29.0] 2026-02-03

### Changed

- `PlusUtil` moved to `Fp.Plus` namespace

## [0.28.0] 2026-02-03

### Changed

- Target TFMs are now `netstandard2.1`, `net8.0`, `net10.0`

## [0.27.0] 2022-09-06

### Changed

- Updated Fp dependency to 0.27.0
- Updated SixLabors.ImageSharp dependency to 2.1.3

## [0.26.0] 2022-01-30

### Changed

- Updated Fp dependency to 0.26.0

## [0.25.0] 2022-01-18

### Changed

- Updated `Fp` to `0.25.0`

## [0.24.0] 2022-01-16

### Changed

- Updated `Fp` to `0.24.0`

## [0.23.1] 2021-12-27

### Changed

- Updated `Fp` to `0.23.1`

## [0.23.0] 2021-12-27

### Changed

- Updated `Fp` to `0.23.0`

## [0.22.2] 2021-12-20

### Changed

- Wave: Fixed incorrect data chunk position on header when fact chunk is required
- Wave: Always clearing header buffer from pool
- Wave: Fix output buffer size on stereo float

## [0.22.1] 2021-12-19

### Added

- Fact member on `PcmInfo`
- `Wave` class for public accessible `WriteWave` / `WritePcmWave` / `WriteFloatWave` / `WriteStereoFloatWave`

### Changed

- Renamed PcmInfo to WaveInfo (for usage with float data - it already contained WAVE-specific properties anyway)
- Moved `PcmData.PcmWave` to `Wave.WaveFormat`
- Renamed `PlusUtil.Audio` overloads to `PlusUtil.Pcm`

## [0.22.0] 2021-12-17

### Changed

- Updated documentation
- Updated `Fp` to `0.22.0`

## [0.21.0] 2021-11-09

### Changed

- Updated `Fp` to `0.21.0`
- Changed `net5.0` target to `net6.0`
