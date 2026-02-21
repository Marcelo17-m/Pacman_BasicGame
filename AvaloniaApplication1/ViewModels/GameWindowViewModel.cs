using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Models.Ghosts;
using AvaloniaApplication1.Models.Ghosts.Behavior;
using AvaloniaApplication1.Models.Ghosts.Ghosts;
using AvaloniaApplication1.Models.Services;
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
        private int _score;

        [ObservableProperty]
        private string _elapsedTime = "0.0s";

        [ObservableProperty]
        private int _currentFPS;

        [ObservableProperty]
        private int _lives = 3;

        [ObservableProperty]
        private Bitmap? _mapImage;

        [ObservableProperty]
        private string _gameOverText = "";

        [ObservableProperty]
        private string _playerName = "";

        [ObservableProperty]
        private string _saveErrorMessage = "";

        [ObservableProperty]
        private bool _isGameOverVisible = false;

        // coleccion de viewmodels para los objetos del juego
        public ObservableCollection<GameObjectViewModel> GameObjects { get; } = new();

        SoundManager SoundManager = new();
        private GameEngine _gameEngine;
        private DispatcherTimer _gameLoopTimer;
        private Pacman? _pacman;
        private Ghost? _ghostBlinky;
        private Ghost? _ghostPinky;
        private Ghost? _ghostInky;
        private Ghost? _ghostClyde;

        // Mapeo Model -> ViewModel
        private readonly Dictionary<GameObject, GameObjectViewModel> _viewModelMap = new();
        public GameWindowViewModel(MainWindowViewModel main)
        {
            _main = main;
            _gameEngine = new GameEngine();

            _gameEngine.PacmanDied += OnPacmanDied;
            _gameEngine.GameOver += OnGameOver;

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
            Score = _gameEngine.Score;
            ElapsedTime = $"{_gameEngine.TotalTime:F1}s";
            CurrentFPS = _gameEngine.CurrentFPS;
            Lives = _gameEngine.Lives;

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
            var ghostSprite = _gameEngine.SpriteManager.LoadSprite("GhostsViews.png");

            if (pacmanSprite == null || ghostSprite == null)
            {
                Console.WriteLine("Failed Sprite");
                return;
            }

            MapImage = _gameEngine.SpriteManager.LoadSprite("PacmanMap1.png");
            if (MapImage == null)
            {
                return;
            }

            var startPos = _gameEngine.Map.TileToWorld(29, 2); 
            //centrar el pacman en la celda que es 16x16 y el pacman 14x14 
            //double centeredX = startPos.x + (16 -14.5) / 2; 
            //double centeredY = startPos.y + (16 - 14.5) / 2;
            IGhostBehavior ghostBehavior = new BlinkyBehavior();
            _pacman = new Pacman(startPos.x + 0.75, startPos.y + 0.75, pacmanSprite, _gameEngine.Map, spriteSize: 16);//pacman
            _pacman.SetNextDirection(Pacman.PacmanDirection.Right);
            _gameEngine.AddGameObject(_pacman);

            var ghostHomePos = _gameEngine.Map.TileToWorld(10, 12);
            var ghostHomePos2 = _gameEngine.Map.TileToWorld(11, 13);
            var ghostHomePos3 = _gameEngine.Map.TileToWorld(11, 14);
            var ghostHomePos4 = _gameEngine.Map.TileToWorld(10, 15);
            _ghostBlinky = new Ghost(ghostHomePos.x +0.5, ghostHomePos.y +0.5, type: GhostType.Blinky, behavior: new BlinkyBehavior(), ghostSprite, _gameEngine.Map);
            _gameEngine.AddGameObject(_ghostBlinky);
            _ghostPinky = new Ghost(ghostHomePos2.x +0.5, ghostHomePos2.y +0.5, type: GhostType.Pinky, behavior: new PinkyBehavior(), ghostSprite, _gameEngine.Map);
            _gameEngine.AddGameObject(_ghostPinky);
            _ghostInky = new Ghost(ghostHomePos3.x + 0.5, ghostHomePos3.y + 0.5, type: GhostType.Inky, behavior: new InkyBehavior(), ghostSprite, _gameEngine.Map);
            _gameEngine.AddGameObject(_ghostInky);
            _ghostClyde = new Ghost(ghostHomePos4.x + 0.5, ghostHomePos4.y + 0.5, type: GhostType.Clyde, behavior: new ClydeBehavior(), ghostSprite, _gameEngine.Map);
            _gameEngine.AddGameObject(_ghostClyde);

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

        private void OnPacmanDied()
        {
            //ponerle un sonido de muerte
            string pacmanDyingAudio = "PacmanDeathSound";
            SoundManager.PlaySound(pacmanDyingAudio);
        }

        private void OnGameOver()
        {
            _gameLoopTimer.Stop();
            GameOverText = "GAME OVER!!!";

            IsGameOverVisible = true;
            string gameOverAudio = "GameOverSound";
            SoundManager.PlaySound(gameOverAudio);

            //limpiarlo
            PlayerName = "";
            SaveErrorMessage = "";
        }

        [RelayCommand]
        private void Back()
        {
            _gameLoopTimer.Stop();
            _main.GoBackToMenu();
        }

        [RelayCommand]
        private void SaveData()
        {
            // validar que tenga 3 letras
            if (string.IsNullOrWhiteSpace(PlayerName))
            {
                SaveErrorMessage = "Please enter 3 initials";
                return;
            }

            //Convertir a mayuscula y dejar 3 letras
            string initials = PlayerName.ToUpper().Trim();

            if (initials.Length != 3)
            {
                SaveErrorMessage = "Must be exact three letters only";
                return;
            }

            if (!initials.All(char.IsLetter))
            {
                SaveErrorMessage = "Only letters allowed";
                return;
            }

            var newScore = new Score
            {
                Name = initials,
                Points = _gameEngine.Score // tomando el del scoreViewModel puede ser null
            };

            var scores = ScoreService.LoadScores();

            scores?.Add(newScore);

            //Save Data
            ScoreService.SaveScore(scores);
            SaveErrorMessage = "Score save";

            _main.GoBackToMenu();
        }

    }
}
