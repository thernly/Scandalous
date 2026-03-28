using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Scandalous.Avalonia.Views;

public partial class MessageDialog : Window
{
    public MessageDialog()
    {
        InitializeComponent();
    }

    public static MessageDialog CreateYesNo(string title, string message)
    {
        var dialog = new MessageDialog();
        dialog.Title = title;
        dialog.MessageText.Text = message;
        dialog.YesButton.IsVisible = true;
        dialog.NoButton.IsVisible = true;
        dialog.OkButton.IsVisible = false;
        return dialog;
    }

    public static MessageDialog CreateOk(string title, string message)
    {
        var dialog = new MessageDialog();
        dialog.Title = title;
        dialog.MessageText.Text = message;
        dialog.YesButton.IsVisible = false;
        dialog.NoButton.IsVisible = false;
        dialog.OkButton.IsVisible = true;
        return dialog;
    }

    private void YesButton_Click(object? sender, RoutedEventArgs e) => Close(true);
    private void NoButton_Click(object? sender, RoutedEventArgs e) => Close(false);
    private void OkButton_Click(object? sender, RoutedEventArgs e) => Close(true);
}
