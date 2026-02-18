using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
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

        //manejar el scoreboard que se muestre llamando su viewmodel
        [ObservableProperty]
        private ScoreboardWindowViewModel _scoreBoard = new();

        [ObservableProperty]
        private Bitmap? _mapImage;

        // coleccion de viewmodels para los objetos del juego
        public ObservableCollection<GameObjectViewModel> GameObjects { get; } = new();

        private GameEngine _gameEngine;
        private DispatcherTimer _gameLoopTimer;
        private Pacman? _pacman;

        // Mapeo Model -> ViewModel
        private readonly Dictionary<GameObject, GameObjectViewModel> _viewModelMap = new();
        public GameWindowViewModel(MainWindowViewModel main)
        {
            _main = main;
            _gameEngine = new GameEngine();
            _gameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(GameEngine.TargetFrameMS) // 60 FPS == 16,67 ms
            };

            _gameLoopTimer.Tick += GameLoopTimerTick;

            StartGame();
            StartGameLoop();
        }

        private void GameLoopTimerTick(object? sender, EventArgs e)
        {
            //actualizar el engine cada momento para hacer validaciones
            _gameEngine.Update();

            //sincronizar el scoreboard
            ScoreBoard.UpdateFromEngine(_gameEngine);

            //sincronizar si se agrega o elimina un objeto
            SyncGameObjects();
        }

        private void SyncGameObjects()
        {
            foreach (var gameObject in _gameEngine.GameObjects)
            {
                if (!_viewModelMap.ContainsKey(gameObject))
                {
                    var vm = new GameObjectViewModel(gameObject);
                    _viewModelMap[gameObject] = vm;
                    GameObjects.Add(vm);
                }
            }

            foreach (var vm in GameObjects)
            {
                vm.SyncFromModel();
            }

            for (int i = GameObjects.Count - 1; i >= 0; i--)
            {
                if (!GameObjects[i].IsActive)
                {
                    var vm = GameObjects[i];
                    _viewModelMap.Remove(vm.Model);
                    GameObjects.RemoveAt(i);
                }
            }
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

            MapImage = _gameEngine.SpriteManager.LoadSprite("PacmanMap1.png");
            if (MapImage == null)
            {
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

            SyncGameObjects();
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
        public void StartGameLoop()
        {
            _gameLoopTimer.Start();
        }

        [RelayCommand]
        private void Back()
        {
            _main.GoBackToMenu();
        }

    }
}
