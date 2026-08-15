using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Scandalous.Avalonia.ViewModels;
using Scandalous.Core.Models;
using Scandalous.Core.Services;
using System.Diagnostics;

namespace Scandalous.Avalonia.Views;

public partial class MainWindow : Window
{
    private bool _isCloseWorkflowRunning;
    private bool _isReissuingClose;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Closing += OnClosing;
    }

    private async void OnLoaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;

        vm.PickOutputFolderAsync = async () =>
        {
            var result = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Output Folder",
                AllowMultiple = false
            });
            return result.FirstOrDefault()?.Path.LocalPath;
        };

        vm.PickTessdataFolderAsync = async () =>
        {
            var result = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Tessdata Folder",
                AllowMultiple = false
            });
            return result.FirstOrDefault()?.Path.LocalPath;
        };

        vm.ShowConfirmationDialogAsync = async (title, message, primaryLabel, secondaryLabel) =>
        {
            var dialog = MessageDialog.CreateConfirmation(title, message, primaryLabel, secondaryLabel);
            return await dialog.ShowDialog<bool>(this);
        };

        vm.ShowErrorDialogAsync = async (title, message) =>
        {
            var dialog = MessageDialog.CreateOk(title, message);
            await dialog.ShowDialog<bool>(this);
        };

        var windowState = await vm.LoadWindowStateAsync();
        if (windowState != null)
            ApplyWindowState(windowState);

        await vm.InitializeAsync();
    }

    private async void OnClosing(object? sender, global::Avalonia.Controls.WindowClosingEventArgs e)
    {
        if (_isReissuingClose)
        {
            _isReissuingClose = false;
            return;
        }

        if (_isCloseWorkflowRunning)
        {
            e.Cancel = true;
            return;
        }

        e.Cancel = true;
        _isCloseWorkflowRunning = true;

        try
        {
            if (DataContext is MainWindowViewModel vm)
            {
                if (vm.IsScanning)
                    await vm.CancelScanAndWaitAsync();

                try
                {
                    await vm.SaveConfigurationAsync();
                    await vm.SaveWindowStateAsync(CreateWindowStateInfo());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to persist state on close: {ex}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Close workflow failed: {ex}");
        }
        finally
        {
            _isCloseWorkflowRunning = false;
            _isReissuingClose = true;
            Close();
        }
    }

    private void ApplyWindowState(WindowStateInfo saved)
    {
        // Width/Height are logical pixels while Position and screen working areas are physical,
        // so normalization runs entirely in logical pixels and the position is converted back.
        var scaling = RenderScaling > 0 ? RenderScaling : 1;
        var logical = new WindowStateInfo
        {
            Width = saved.Width,
            Height = saved.Height,
            Left = saved.Left / scaling,
            Top = saved.Top / scaling,
            State = saved.State
        };

        var bounds = WindowBoundsNormalizer.Normalize(logical, GetScreenAreas(), MinWidth, MinHeight);

        Width = bounds.Width;
        Height = bounds.Height;

        if (bounds.CenterOnScreen)
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        else
            Position = new global::Avalonia.PixelPoint(
                (int)(bounds.Left!.Value * scaling),
                (int)(bounds.Top!.Value * scaling));

        WindowState = bounds.State == Core.Models.WindowState.Maximized
            ? global::Avalonia.Controls.WindowState.Maximized
            : global::Avalonia.Controls.WindowState.Normal;
    }

    /// <summary>
    /// Screen working areas converted from physical to logical pixels.
    /// </summary>
    private List<ScreenArea> GetScreenAreas()
    {
        var areas = new List<ScreenArea>();
        foreach (var screen in Screens.All)
        {
            var scaling = screen.Scaling > 0 ? screen.Scaling : 1;
            var area = screen.WorkingArea;
            areas.Add(new ScreenArea(
                area.X / scaling,
                area.Y / scaling,
                area.Width / scaling,
                area.Height / scaling));
        }

        return areas;
    }

    private WindowStateInfo CreateWindowStateInfo() => new()
    {
        Width = Width,
        Height = Height,
        Left = Position.X,
        Top = Position.Y,
        State = WindowState switch
        {
            global::Avalonia.Controls.WindowState.Maximized => Core.Models.WindowState.Maximized,
            global::Avalonia.Controls.WindowState.Minimized => Core.Models.WindowState.Minimized,
            _ => Core.Models.WindowState.Normal
        }
    };
}
