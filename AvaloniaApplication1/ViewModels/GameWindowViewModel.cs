using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.ViewModels
{
    public partial class GameWindowViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _main;

        [ObservableProperty]
        private int _currentFPS;

        [ObservableProperty]
        private string _elapsedTime;

        private GameEngine _gameEngine;
        private DispatcherTimer _gameLoopTimer;
        private Pacman? _pacman;

        public ObservableCollection<GameObject> GameObjects { get; private set; } = new ObservableCollection<GameObject>();
        public GameWindowViewModel(MainWindowViewModel main)
        {
            _main = main;
            _gameEngine = new GameEngine();
            _gameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(GameEngine.TargetFrameMS) // 60 FPS == 16,67 ms
            };

            _elapsedTime = "0.0s";

            _gameLoopTimer.Tick += GameLoopTimerTick;

            StartGame();

            StartGameLoop();
        }

        private void GameLoopTimerTick(object? sender, EventArgs e)
        {
            _gameEngine.Update();
            ElapsedTime = $"{_gameEngine.TotalTime:F1}s";
            CurrentFPS = _gameEngine.CurrentFPS;
            
            // Actualizar la colección observable con los objetos del juego
            GameObjects.Clear();
            foreach (var obj in _gameEngine.GameObjects)
            {
                GameObjects.Add(obj);
            }
        } 

        public void StartGameLoop()
        {
            _gameLoopTimer.Start();
        }

        public void StartGame()
        {
            _gameEngine.Map = GameMap.CreateFromLayout(PacmanMaps.ClassicMap, tileSize: 16);

            var pacmanSprite = _gameEngine.SpriteManager.LoadSprite("PacmanViewsFinal.png");
            if (pacmanSprite == null)
            {
                Console.WriteLine("Failed Sprite");
                return;
            }

            var startPos = _gameEngine.Map.TileToWorld(1, 15);
            //centrar el pacman en la celda que es 16x16 y el pacman 14x14 
            //double centeredX = startPos.x + (16 -14) / 2; 
            //double centeredY = startPos.y + (16 - 14) / 2;
            _pacman = new Pacman(startPos.x + 1, startPos.y + 1, pacmanSprite, _gameEngine.Map, spriteSize: 16);
            _pacman.SetNextDirection(Pacman.PacmanDirection.Right);
            _gameEngine.AddGameObject(_pacman);

            _gameEngine.CreatePellets(); //crear los puntos y power ups

            GameObjects.Clear();
            foreach (var obj in _gameEngine.GameObjects)
            {
                GameObjects.Add(obj);
            }
        }
        
        public void HandleKeyPress(Avalonia.Input.Key key)
        {
            if (_pacman == null) return;
            
            switch (key)
            {
                case Avalonia.Input.Key.Up:
                case Avalonia.Input.Key.W:
                    _pacman.SetNextDirection(Pacman.PacmanDirection.Up);
                    break;
                case Avalonia.Input.Key.Down:
                case Avalonia.Input.Key.S:
                    _pacman.SetNextDirection(Pacman.PacmanDirection.Down);
                    break;
                case Avalonia.Input.Key.Left:
                case Avalonia.Input.Key.A:
                    _pacman.SetNextDirection(Pacman.PacmanDirection.Left);
                    break;
                case Avalonia.Input.Key.Right:
                case Avalonia.Input.Key.D:
                    _pacman.SetNextDirection(Pacman.PacmanDirection.Right);
                    break;
            }
        }


        [RelayCommand]
        private void Back()
        {
            _main.GoBackToMenu();
        }

    }
}
