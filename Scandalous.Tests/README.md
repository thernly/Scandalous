# Scandalous Test Suite

This project contains comprehensive xUnit tests for the Scandalous document scanning application.

## Test Coverage

### FileNameValidatorTests
Tests for the `FileNameValidator` static class that validates Windows file names.

**Test Categories:**
- **Null/Empty validation** - Ensures null, empty, and whitespace names are rejected
- **Valid names** - Tests valid base names and full file names (with extensions)
- **Invalid characters** - Validates rejection of Windows-prohibited characters (`< > : " / \ | ? *`)
- **Reserved names** - Tests rejection of Windows reserved names (CON, PRN, AUX, NUL, COM1-9, LPT1-9)
- **Ending validation** - Ensures names cannot end with space or period
- **Dot handling** - Validates rejection of `.` and `..`
- **Extension separator** - Tests base name validation (no periods allowed)
- **Length validation** - Tests 255 character limit
- **Edge cases** - Hidden files, multiple extensions, Unicode, similar-but-not-reserved names

**Total Tests:** ~40 test cases

### FolderValidatorTests
Tests for the `FolderValidator` static class that validates Windows folder paths.

**Test Categories:**
- **Null/Empty validation** - Ensures null, empty, and whitespace paths are rejected
- **Valid paths** - Tests simple names, absolute paths, relative paths, root paths, drive letters
- **Invalid path characters** - Validates rejection of prohibited characters
- **Invalid segment characters** - Tests segment-level character validation
- **Reserved names** - Tests rejection of reserved names in any path segment
- **Ending validation** - Ensures segments cannot end with space or period
- **Dot handling** - Validates rejection of `.` and `..` as path segments
- **Segment length** - Tests 255 character limit per segment
- **Drive letter handling** - Special validation for Windows drive letters
- **Edge cases** - Double separators, mixed separators, Unicode, long paths

**Total Tests:** ~35 test cases

### ScanConfigurationTests
Tests for the `ScanConfiguration` class that holds scanning settings.

**Test Categories:**
- **Valid construction** - Tests constructor with various valid parameter combinations
- **Default values** - Validates default parameter values
- **Parameterless constructor** - Tests deserialization constructor
- **OutputFolder validation** - Tests null, empty, invalid characters, and reserved names
- **BaseFileName validation** - Tests null, empty, extension separator, invalid characters
- **TessdataFolder validation** - Tests null, empty, and invalid paths
- **Property get/set** - Validates all properties can be set and retrieved
- **Enum values** - Tests all valid enum values (ColorMode, DocumentOptions, PaperSource)
- **Edge cases** - Long valid paths, various DPI values, language codes

**Total Tests:** ~30 test cases

### ConfigurationManagerTests
Tests for the `ConfigurationManager` class that handles settings persistence.

**Test Categories:**
- **Save configuration** - Tests successful saving and overwriting
- **Load configuration** - Tests loading non-existent and saved configurations
- **Tessdata enumeration** - Tests `GetInstalledTessdataLanguageCodes()` with:
  - Null/empty/whitespace paths
  - Non-existent directories
  - Empty directories
  - Directories with .traineddata files
  - Mixed file types
  - Subdirectories (should be ignored)
  - Complex language codes (chi_sim, chi_tra, etc.)
- **JSON format** - Validates camelCase property naming

**Total Tests:** ~15 test cases

## Technology Stack

- **xUnit** 2.6.6 - Testing framework
- **FluentAssertions** 6.12.0 - Fluent assertion library for readable tests
- **Microsoft.NET.Test.Sdk** 17.8.0 - Test platform
- **Moq** 4.20.70 - Mocking framework (available for future use)

## Running the Tests

### Using .NET CLI
```bash
dotnet test
```

### Using Visual Studio
1. Open the solution in Visual Studio
2. Open Test Explorer (Test > Test Explorer)
3. Click "Run All Tests"

### Using Visual Studio Code
1. Install the .NET Core Test Explorer extension
2. Tests will appear in the Test Explorer panel
3. Click the run icon to execute tests

## Test Naming Convention

Tests follow the pattern: `MethodName_Scenario_ExpectedBehavior`

Examples:
- `IsValid_NullName_ReturnsFalse`
- `Constructor_ValidParameters_CreatesInstance`
- `GetInstalledTessdataLanguageCodes_WithTrainedDataFiles_ReturnsLanguageCodes`

## Code Coverage

The test suite provides comprehensive coverage for:
- ✅ **FileNameValidator** - ~95% coverage
- ✅ **FolderValidator** - ~95% coverage
- ✅ **ScanConfiguration** - ~90% coverage
- ✅ **ConfigurationManager** - ~85% coverage

## Future Test Additions

Potential areas for additional testing:
- **DocumentScanner** - Core scanning logic (requires NAPS2 mocking)
- **FormScan** - UI logic (requires UI testing framework or logic extraction)
- **Integration tests** - End-to-end scanning workflows
- **Performance tests** - Large file handling, batch scanning

## Contributing

When adding new tests:
1. Follow the existing naming convention
2. Use FluentAssertions for assertions
3. Group related tests using regions (#region)
4. Add Theory tests for multiple similar scenarios
5. Include edge cases and boundary conditions
6. Clean up test resources in Dispose() if needed
