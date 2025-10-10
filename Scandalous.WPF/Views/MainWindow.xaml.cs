using System;
using System.Windows;
using System.Windows.Controls;
using Scandalous.WPF.ViewModels;

namespace Scandalous.WPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel? _viewModel;
    private bool _isLeftPanelCollapsed = false;
    private const double LeftPanelMinWidth = 500;

    public MainWindow(MainWindowViewModel viewModel)
    {
        try
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Startup error: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // Optionally log to a file or event log here
            
        }
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (_viewModel is null) return;
        // Load window state from ViewModel
        await _viewModel.LoadWindowStateCommand.ExecuteAsync(null);
    }

    private async void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_viewModel is null) return;
        // Confirm close if scanning
        if (_viewModel.IsScanning)
        {
            var result = System.Windows.MessageBox.Show(
                "A scan is in progress. Are you sure you want to exit?",
                "Confirm Exit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }

        // Save window state through ViewModel
        await _viewModel.OnWindowClosingAsync();
    }

    private void ToggleLeftPanel_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel is null) return;
        if (_isLeftPanelCollapsed)
        {
            // Expand
            _viewModel.LeftPanelWidth = LeftPanelMinWidth;
            LeftPanelCard.Visibility = Visibility.Visible;
            CollapseIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ChevronLeft;
            _isLeftPanelCollapsed = false;
        }
        else
        {
            // Collapse
            _viewModel.LeftPanelWidth = 0;
            LeftPanelCard.Visibility = Visibility.Collapsed;
            CollapseIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ChevronRight;
            _isLeftPanelCollapsed = true;
        }
    }

    private void OutputPath_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
    {
        e.Handled = true;
        e.Effects = e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop) ? System.Windows.DragDropEffects.Copy : System.Windows.DragDropEffects.None;
    }

    private void OutputPath_Drop(object sender, System.Windows.DragEventArgs e)
    {
        if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (files.Length > 0 && System.IO.Directory.Exists(files[0]))
            {
                if (_viewModel != null)
                {
                    _viewModel.OutputSettingsViewModel.OutputPath = files[0];
                }
            }
        }
    }

    private System.Windows.Point _lastMousePosition;
    private bool _isPanning = false;

    private void PreviewImage_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        if (_viewModel?.PreviewViewModel.PreviewImage == null) return;

        double scaleFactor = 1.1;
        if (e.Delta < 0)
        {
            scaleFactor = 1 / scaleFactor;
        }

        var image = (System.Windows.Controls.Image)sender;
        var transform = (System.Windows.Media.ScaleTransform)((System.Windows.Media.TransformGroup)image.RenderTransform).Children[0];
        var position = e.GetPosition(image);

        _viewModel.PreviewViewModel.Scale *= scaleFactor;
        _viewModel.PreviewViewModel.OffsetX = position.X - (position.X - _viewModel.PreviewViewModel.OffsetX) * scaleFactor;
        _viewModel.PreviewViewModel.OffsetY = position.Y - (position.Y - _viewModel.PreviewViewModel.OffsetY) * scaleFactor;

        e.Handled = true;
    }

    private void PreviewImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_viewModel?.PreviewViewModel.PreviewImage == null) return;

        _isPanning = true;
        _lastMousePosition = e.GetPosition((System.Windows.IInputElement)sender);
        ((System.Windows.UIElement)sender).CaptureMouse();
    }

    private void PreviewImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (_isPanning && _viewModel?.PreviewViewModel.PreviewImage != null)
        {
            var currentMousePosition = e.GetPosition((System.Windows.IInputElement)sender);
            _viewModel.PreviewViewModel.OffsetX += (currentMousePosition.X - _lastMousePosition.X);
            _viewModel.PreviewViewModel.OffsetY += (currentMousePosition.Y - _lastMousePosition.Y);
            _lastMousePosition = currentMousePosition;
        }
    }

    private void PreviewImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _isPanning = false;
        ((System.Windows.UIElement)sender).ReleaseMouseCapture();
    }
}