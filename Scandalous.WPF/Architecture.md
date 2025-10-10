# Scandalous.WPF Architecture Design

## 1. MVVM Architecture Pattern

### Overview
The application follows the Model-View-ViewModel (MVVM) pattern with the following layers:

- **Model**: `Scandalous.Core` - Business logic, services, and data models
- **View**: WPF XAML files - User interface presentation
- **ViewModel**: C# classes - Presentation logic and data binding

### Key Principles
- **Separation of Concerns**: Each layer has a single responsibility
- **Dependency Injection**: Services are injected into ViewModels
- **Data Binding**: Views are bound to ViewModels through XAML
- **Commands**: User interactions are handled through RelayCommands
- **Observable Properties**: UI updates automatically through INotifyPropertyChanged

### Technology Stack
- **MVVM Framework**: CommunityToolkit.Mvvm
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **UI Framework**: WPF (.NET 9.0)
- **Business Logic**: Scandalous.Core (reused from WinForms)

## 2. ViewModel Hierarchy and Relationships

### MainWindowViewModel
```
MainWindowViewModel
├── Properties (Observable)
│   ├── StatusMessage
│   ├── IsScanning
│   ├── ScanProgress
│   ├── UiState (ScanConfiguration mapping)
│   ├── ScannedPages (ObservableCollection)
│   ├── AvailableScanners (ObservableCollection)
│   └── AvailableLanguages (ObservableCollection)
├── Commands
│   ├── LoadConfigurationCommand
│   ├── ScanDocumentsCommand
│   ├── LoadAvailableScannersCommand
│   ├── LoadAvailableLanguagesCommand
│   └── SaveConfigurationCommand
└── Dependencies
    ├── IDocumentScanner
    ├── IConfigurationManager
    ├── IScanConfigurationMapper
    ├── IPdfService
    ├── ILanguageCodeService
    └── IScanExceptionHandler
```

### Future ViewModels (Phase 3+)
```
ScanConfigurationViewModel (if needed for complex configuration)
├── OutputSettingsViewModel
├── DocumentOptionsViewModel
├── ColorModeViewModel
├── ScannerSettingsViewModel
└── OcrSettingsViewModel

ImagePreviewViewModel
├── CurrentImage
├── ZoomLevel
├── Rotation
└── ImageList

ProgressViewModel
├── CurrentOperation
├── ProgressPercentage
├── EstimatedTimeRemaining
└── CancelCommand
```

## 3. Data Binding Strategy

### Property Binding
```xml
<!-- Simple property binding -->
<TextBlock Text="{Binding StatusMessage}"/>

<!-- Nested property binding -->
<TextBox Text="{Binding UiState.OutputFolder}"/>

<!-- Collection binding -->
<ListBox ItemsSource="{Binding AvailableScanners}"/>

<!-- Command binding -->
<Button Command="{Binding ScanDocumentsCommand}"/>
```

### Value Converters
```csharp
// Boolean to visibility
public class BooleanToVisibilityConverter : IValueConverter

// Inverse boolean (for IsEnabled)
public class InverseBooleanConverter : IValueConverter

// Enum to string (for display)
public class EnumToStringConverter : IValueConverter

// File size formatting
public class FileSizeConverter : IValueConverter
```

### Validation
```csharp
// Data annotations in ViewModels
[Required]
[StringLength(255)]
public string OutputFolder { get; set; }

// Custom validation rules
public class FolderExistsValidationRule : ValidationRule
```

## 4. Navigation Strategy

### Single Window Approach (Recommended)
- **Main Window**: Contains all functionality in a single, well-organized interface
- **Panels**: Collapsible/expandable panels for different sections
- **Tabs**: If needed for complex workflows
- **Dialogs**: Modal dialogs for configuration or error messages

### Layout Structure
```
MainWindow
├── StatusBar (top)
├── Main Content
│   ├── Left Panel (Scan Controls)
│   │   ├── Output Settings
│   │   ├── Document Options
│   │   ├── Color Mode
│   │   ├── Scanner Settings
│   │   └── OCR Settings
│   ├── Splitter (resizable)
│   └── Right Panel (Preview)
│       ├── Image Preview
│       └── Progress Bar
└── Status Bar (bottom)
```

### Dialog Strategy
```csharp
// Error dialogs
public class ErrorDialogService
{
    public Task ShowErrorAsync(string message, string title = "Error");
    public Task ShowExceptionAsync(Exception ex);
}

// Configuration dialogs
public class ConfigurationDialogService
{
    public Task<bool> ShowFolderPickerAsync(string initialPath);
    public Task<string> ShowLanguageSelectorAsync(List<string> languages);
}
```

## 5. Async/Await Patterns

### ViewModel Async Patterns
```csharp
// Command pattern for async operations
[RelayCommand]
private async Task ScanDocumentsAsync()
{
    try
    {
        IsScanning = true;
        await _documentScanner.ScanDocuments(configuration);
    }
    finally
    {
        IsScanning = false;
    }
}

// Event handling with UI thread dispatch
private void OnPageScanned(object? sender, PageScannedEventArgs e)
{
    Application.Current.Dispatcher.Invoke(() =>
    {
        ScannedPages.Add(e.ImageFilePath);
        UpdateProgress();
    });
}
```

### Service Integration Patterns
```csharp
// Service calls with proper error handling
public async Task LoadConfigurationAsync()
{
    try
    {
        var config = await _configurationManager.LoadConfigurationAsync();
        UiState = _scanConfigurationMapper.BuildUIStateFromConfiguration(config);
    }
    catch (Exception ex)
    {
        var result = _exceptionHandler.HandleScanException(ex);
        StatusMessage = result.UserMessage;
    }
}

// Background operations
public async Task PerformBackgroundOperationAsync()
{
    await Task.Run(() =>
    {
        // CPU-intensive work
    });
}
```

### UI Responsiveness Patterns
```csharp
// Progress reporting
public async Task ScanWithProgressAsync()
{
    var progress = new Progress<int>(value =>
    {
        ScanProgress = value;
        StatusMessage = $"Scanning... {value}%";
    });
    
    await _documentScanner.ScanDocumentsAsync(configuration, progress);
}

// Cancellation support
private CancellationTokenSource? _cancellationTokenSource;

[RelayCommand]
private async Task CancelScanAsync()
{
    _cancellationTokenSource?.Cancel();
}
```

## 6. Error Handling Strategy

### Exception Flow
```
Service Layer (Core) → ExceptionHandler → ViewModel → UI
```

### Error Types
- **Scan Errors**: DeviceFeederEmptyException, ScanDriverUnknownException
- **Configuration Errors**: Validation errors, file access errors
- **UI Errors**: Binding errors, XAML parsing errors

### Error Display
```csharp
// Centralized error handling
public class ErrorHandlingService
{
    public void HandleError(Exception ex, string context);
    public Task ShowUserFriendlyErrorAsync(Exception ex);
    public void LogError(Exception ex, string context);
}
```

## 7. Testing Strategy

### Unit Testing
```csharp
// ViewModel testing with mocked services
[Test]
public async Task ScanDocumentsCommand_WhenSuccessful_UpdatesStatus()
{
    // Arrange
    var mockScanner = new Mock<IDocumentScanner>();
    var viewModel = new MainWindowViewModel(mockScanner.Object, ...);
    
    // Act
    await viewModel.ScanDocumentsCommand.ExecuteAsync(null);
    
    // Assert
    Assert.AreEqual("Scanning completed successfully", viewModel.StatusMessage);
}
```

### Integration Testing
```csharp
// Service integration testing
[Test]
public async Task ViewModel_WithRealServices_LoadsConfiguration()
{
    // Arrange
    var services = new ServiceCollection();
    ConfigureServices(services);
    var serviceProvider = services.BuildServiceProvider();
    
    // Act
    var viewModel = serviceProvider.GetService<MainWindowViewModel>();
    await viewModel.LoadConfigurationCommand.ExecuteAsync(null);
    
    // Assert
    Assert.IsNotNull(viewModel.UiState);
}
```

## 8. Performance Considerations

### Memory Management
- **ObservableCollections**: Use for frequently changing collections
- **Weak Events**: For event subscriptions to prevent memory leaks
- **Disposal**: Proper disposal of services and resources

### UI Responsiveness
- **Async Operations**: All I/O operations are async
- **Progress Reporting**: Real-time progress updates
- **Background Processing**: CPU-intensive work on background threads

### Data Binding Performance
- **Virtualization**: For large lists (scanned pages)
- **Lazy Loading**: Load images on demand
- **Caching**: Cache frequently accessed data

## 9. Accessibility

### Screen Reader Support
- **Semantic markup**: Proper use of headers, labels, and descriptions
- **Keyboard navigation**: All functionality accessible via keyboard
- **High contrast**: Support for high contrast themes

### Input Validation
- **Real-time validation**: Immediate feedback on user input
- **Clear error messages**: User-friendly error descriptions
- **Alternative input methods**: Support for different input devices

## 10. Future Extensibility

### Plugin Architecture
- **Service interfaces**: All services are interface-based for easy mocking/testing
- **Configuration system**: Extensible configuration management
- **Theme system**: Support for custom themes and styling

### Feature Additions
- **Batch processing**: Multiple document scanning
- **Advanced OCR**: Additional OCR features
- **Export formats**: Support for additional output formats 