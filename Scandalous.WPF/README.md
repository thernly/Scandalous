# Scandalous.WPF

Modern WPF UI for the Scandalous document scanning application.

## Architecture

This project follows the MVVM (Model-View-ViewModel) pattern and reuses the business logic from `Scandalous.Core`.

### Project Structure

```
Scandalous.WPF/
├── Views/           # WPF Views (XAML files)
├── ViewModels/      # ViewModels (C# files)
├── Services/        # WPF-specific services
├── Converters/      # Value converters for data binding
├── Controls/        # Custom WPF controls
├── Themes/          # Application themes and styles
└── App.xaml         # Application entry point
```

### Dependencies

- **Scandalous.Core**: Business logic and services
- **CommunityToolkit.Mvvm**: MVVM framework
- **Microsoft.Extensions.DependencyInjection**: Dependency injection

### Core Services Integration

The WPF UI integrates with the following services from `Scandalous.Core`:

- `IDocumentScanner`: Document scanning operations
- `IConfigurationManager`: Settings persistence
- `IScanConfigurationMapper`: UI state mapping
- `IPdfService`: PDF file operations
- `ILanguageCodeService`: OCR language management
- `IScanExceptionHandler`: Error handling

## Development

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Building

```bash
dotnet build
```

### Running

```bash
dotnet run
```

## Features

- Modern WPF UI with responsive design
- Integration with existing Core business logic
- MVVM architecture with CommunityToolkit.Mvvm
- Dependency injection for service management
- Theme support (light/dark)
- Accessibility features 