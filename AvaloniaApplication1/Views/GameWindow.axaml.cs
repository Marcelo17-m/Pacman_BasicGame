using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels;
using System;
using System.Runtime.CompilerServices;

namespace AvaloniaApplication1;

public partial class GameWindow : UserControl
{
    private GameWindowViewModel? _viewModel;
    
    public GameWindow()
    {
        InitializeComponent();
        this.Loaded += GameWindow_Loaded;
    }

    private void GameWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        _viewModel = DataContext as GameWindowViewModel;
        var canvas = this.FindControl<Canvas>("GameCanvas");
        if (canvas != null)
        {
            canvas.KeyDown += Canvas_KeyDown;
        }
    }

    private void Canvas_KeyDown(object? sender, KeyEventArgs e)
    {
        _viewModel?.HandleKeyPress(e.Key);
    }
}