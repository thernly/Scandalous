using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Scandalous.Avalonia.ViewModels;
using Scandalous.Core.Models;

namespace Scandalous.Avalonia.Views;

public partial class MainWindow : Window
{
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

        vm.ShowYesNoDialogAsync = async (title, message) =>
        {
            var dialog = MessageDialog.CreateYesNo(title, message);
            return await dialog.ShowDialog<bool>(this);
        };

        vm.ShowErrorDialogAsync = async (title, message) =>
        {
            var dialog = MessageDialog.CreateOk(title, message);
            await dialog.ShowDialog<bool>(this);
        };

        var windowState = await vm.LoadWindowStateAsync();
        if (windowState != null)
        {
            Width = windowState.Width;
            Height = windowState.Height;
            if (!double.IsNaN(windowState.Left) && !double.IsNaN(windowState.Top))
                Position = new global::Avalonia.PixelPoint((int)windowState.Left, (int)windowState.Top);
            WindowState = windowState.State switch
            {
                Core.Models.WindowState.Maximized => global::Avalonia.Controls.WindowState.Maximized,
                Core.Models.WindowState.Minimized => global::Avalonia.Controls.WindowState.Minimized,
                _ => global::Avalonia.Controls.WindowState.Normal
            };
        }

        await vm.InitializeAsync();
    }

    private async void OnClosing(object? sender, global::Avalonia.Controls.WindowClosingEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;

        await vm.SaveConfigurationAsync();

        var windowState = new WindowStateInfo
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
        await vm.SaveWindowStateAsync(windowState);
    }
}
