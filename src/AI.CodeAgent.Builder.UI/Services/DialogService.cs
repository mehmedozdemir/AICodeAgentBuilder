using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace AI.CodeAgent.Builder.UI.Services;

/// <summary>
/// Avalonia-based implementation of dialog service.
/// Uses native Avalonia MessageBox for dialogs.
/// </summary>
public sealed class DialogService : IDialogService
{
    private Window? GetMainWindow()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }

    public async Task ShowInformationAsync(string title, string message)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var window = GetMainWindow();
            if (window != null)
            {
                var messageBox = new Window
                {
                    Title = title,
                    Width = 400,
                    Height = 200,
                    Content = new TextBlock
                    {
                        Text = message,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        Margin = new Avalonia.Thickness(20)
                    }
                };

                await messageBox.ShowDialog(window);
            }
        });
    }

    public async Task ShowErrorAsync(string title, string message)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var window = GetMainWindow();
            if (window != null)
            {
                var messageBox = new Window
                {
                    Title = $"❌ {title}",
                    Width = 450,
                    Height = 250,
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = "Error",
                                FontSize = 18,
                                FontWeight = Avalonia.Media.FontWeight.Bold,
                                Foreground = Avalonia.Media.Brushes.Red,
                                Margin = new Avalonia.Thickness(20, 20, 20, 10)
                            },
                            new TextBlock
                            {
                                Text = message,
                                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                                Margin = new Avalonia.Thickness(20, 0, 20, 20)
                            }
                        }
                    }
                };

                await messageBox.ShowDialog(window);
            }
        });
    }

    public async Task ShowWarningAsync(string title, string message)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var window = GetMainWindow();
            if (window != null)
            {
                var messageBox = new Window
                {
                    Title = $"⚠️ {title}",
                    Width = 450,
                    Height = 250,
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                Text = "Warning",
                                FontSize = 18,
                                FontWeight = Avalonia.Media.FontWeight.Bold,
                                Foreground = Avalonia.Media.Brushes.Orange,
                                Margin = new Avalonia.Thickness(20, 20, 20, 10)
                            },
                            new TextBlock
                            {
                                Text = message,
                                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                                Margin = new Avalonia.Thickness(20, 0, 20, 20)
                            }
                        }
                    }
                };

                await messageBox.ShowDialog(window);
            }
        });
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var window = GetMainWindow();
            if (window == null)
                return false;

            var result = false;
            Window? confirmDialog = null;
            
            confirmDialog = new Window
            {
                Title = title,
                Width = 400,
                Height = 200,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Children =
                    {
                        new TextBlock
                        {
                            Text = message,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                            Margin = new Avalonia.Thickness(0, 0, 0, 20)
                        },
                        new StackPanel
                        {
                            Orientation = Avalonia.Layout.Orientation.Horizontal,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                            Children =
                            {
                                new Button
                                {
                                    Content = "Yes",
                                    Width = 80,
                                    Margin = new Avalonia.Thickness(0, 0, 10, 0)
                                },
                                new Button
                                {
                                    Content = "No",
                                    Width = 80
                                }
                            }
                        }
                    }
                }
            };

            // Get buttons and attach event handlers
            var buttons = ((StackPanel)((StackPanel)confirmDialog.Content).Children[1]).Children;
            ((Button)buttons[0]).Click += (s, e) => { result = true; confirmDialog.Close(); };
            ((Button)buttons[1]).Click += (s, e) => { result = false; confirmDialog.Close(); };

            await confirmDialog.ShowDialog(window);
            return result;
        });
    }

    public async Task<string?> ShowInputDialogAsync(string title, string prompt, string? defaultValue = null)
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var window = GetMainWindow();
            if (window == null)
                return null;

            string? result = null;
            var textBox = new TextBox
            {
                Text = defaultValue ?? string.Empty,
                Margin = new Avalonia.Thickness(0, 10, 0, 20)
            };

            Window? inputDialog = null;
            
            inputDialog = new Window
            {
                Title = title,
                Width = 400,
                Height = 180,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Children =
                    {
                        new TextBlock { Text = prompt, Margin = new Avalonia.Thickness(0, 0, 0, 10) },
                        textBox,
                        new StackPanel
                        {
                            Orientation = Avalonia.Layout.Orientation.Horizontal,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                            Children =
                            {
                                new Button
                                {
                                    Content = "OK",
                                    Width = 80,
                                    Margin = new Avalonia.Thickness(0, 0, 10, 0)
                                },
                                new Button
                                {
                                    Content = "Cancel",
                                    Width = 80
                                }
                            }
                        }
                    }
                }
            };

            // Get buttons and attach event handlers
            var buttons = ((StackPanel)((StackPanel)inputDialog.Content).Children[2]).Children;
            ((Button)buttons[0]).Click += (s, e) => { result = textBox.Text; inputDialog.Close(); };
            ((Button)buttons[1]).Click += (s, e) => { result = null; inputDialog.Close(); };

            await inputDialog.ShowDialog(window);
            return result;
        });
    }
}
