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
    private Canvas? _gameCanvas;
    private GameWindowViewModel? _viewModel;
    private DispatcherTimer? _renderTimer;
    
    public GameWindow()
    {
        InitializeComponent();
        this.Loaded += GameWindow_Loaded;
        this.KeyDown += GameWindow_KeyDown;
    }
    
    private void GameWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.HandleKeyPress(e.Key);
        }
    }

    private void GameWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        _gameCanvas = this.FindControl<Canvas>("GameCanvas");
        _viewModel = DataContext as GameWindowViewModel;

        if (_gameCanvas != null && _viewModel != null)
        {
            
            // Manejar eventos de teclado
            _gameCanvas.KeyDown += GameCanvas_KeyDown;
            
            // Iniciar el timer de renderizado
            _renderTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _renderTimer.Tick += RenderTimer_Tick;
            _renderTimer.Start();
        }
    }
    
    private void GameCanvas_KeyDown(object? sender, KeyEventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.HandleKeyPress(e.Key);
        }
    }

    private void RenderTimer_Tick(object? sender, EventArgs e)
    {
        if (_gameCanvas == null || _viewModel == null)
        {
            return;
        }

        // limpiar los anteriores
        _gameCanvas.Children.Clear();

        foreach (var gameObject in _viewModel.GameObjects)
        {
            if (!gameObject.IsActive)
            {
                continue;
            }

            if (gameObject is Pellet pellet)
            {
                var ellipse = new Ellipse
                {
                    Width = pellet.Width,
                    Height = pellet.Height,
                    Fill = pellet.FillColor,
                    [Canvas.LeftProperty] = pellet.X,
                    [Canvas.TopProperty] = pellet.Y,
                    [Canvas.ZIndexProperty] = pellet.Zindex
                };
                _gameCanvas.Children.Add(ellipse);
                continue;
            }

            if (gameObject.Sprite == null)
            {
                continue;
            }

            // crear una imagen
            var image = new Image
            {
                Width = gameObject.Width,
                Height = gameObject.Height,
                Source = gameObject.Sprite,
                [Canvas.LeftProperty] = gameObject.X,
                [Canvas.TopProperty] = gameObject.Y,
                [Canvas.ZIndexProperty] = gameObject.Zindex
            };

            if (gameObject.SourceRect.HasValue)
            {
                var sourceRect = gameObject.SourceRect.Value;
                var croppedBitMap = new CroppedBitmap(
                    gameObject.Sprite,
                    new PixelRect(
                        (int)sourceRect.X,
                        (int)sourceRect.Y,
                        (int)sourceRect.Width, 
                        (int)sourceRect.Height
                    ) 
                );
                image.Source = croppedBitMap;
            }
            _gameCanvas.Children.Add( image );
        }
    }
}