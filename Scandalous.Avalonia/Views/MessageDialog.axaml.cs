using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Scandalous.Avalonia.Views;

public partial class MessageDialog : Window
{
    public MessageDialog()
    {
        InitializeComponent();
        Opened += (_, _) => PrimaryButton.Focus(NavigationMethod.Tab);
    }

    /// <summary>
    /// Creates a two-button dialog. The primary button is the default action (Enter) and
    /// returns true; the secondary button is the cancel action (Escape) and returns false.
    /// </summary>
    public static MessageDialog CreateConfirmation(string title, string message, string primaryLabel, string secondaryLabel)
    {
        var dialog = new MessageDialog();
        dialog.Title = title;
        dialog.MessageText.Text = message;
        dialog.PrimaryButton.Content = primaryLabel;
        dialog.SecondaryButton.Content = secondaryLabel;
        dialog.SecondaryButton.IsVisible = true;
        return dialog;
    }

    /// <summary>
    /// Creates a single-button acknowledgement dialog. The button acts as both the default
    /// and the cancel action so that Enter and Escape both dismiss it.
    /// </summary>
    public static MessageDialog CreateOk(string title, string message, string buttonLabel = "OK")
    {
        var dialog = new MessageDialog();
        dialog.Title = title;
        dialog.MessageText.Text = message;
        dialog.PrimaryButton.Content = buttonLabel;
        dialog.PrimaryButton.IsCancel = true;
        dialog.SecondaryButton.IsVisible = false;
        return dialog;
    }

    private void PrimaryButton_Click(object? sender, RoutedEventArgs e) => Close(true);
    private void SecondaryButton_Click(object? sender, RoutedEventArgs e) => Close(false);
}
