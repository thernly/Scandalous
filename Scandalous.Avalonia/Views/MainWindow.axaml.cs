using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Scandalous.Avalonia.ViewModels;

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

        await vm.InitializeAsync();
    }

    private async void OnClosing(object? sender, global::Avalonia.Controls.WindowClosingEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
            await vm.SaveConfigurationAsync();
    }
}
