using Avalonia.Rendering;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Models.Ghosts;
using AvaloniaApplication1.Models.Ghosts.Ghosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Engine
{
    public class GameEngine
    {

        // 1s/60fp = 0.016 *1000 = 16ms
        public const int TargetFPS = 60;

        public const int TargetFrameMS = 1000 / TargetFPS;

        public double TotalTime { get; set; }

        public int CurrentFPS { get; set; }

        public List<GameObject> GameObjects { get; private set; } = new List<GameObject>();

        public SpriteManager SpriteManager { get; private set; } = new SpriteManager();

        public SoundManager SoundManager { get; private set; } = new SoundManager();
        
        public GameMap? Map { get; set; }

        public event Action? PacmanDied;
        public event Action? GameOver;
        public event Action? GameWin;

        private int _frameCount;

        private double _fpsTimer;

        private DateTime _lastUpdateTime;
        private bool _isFrightenModeActive = false;
        private int _frightenFramesRemaining = 0;
        private const int _frightenFramesDuration = 400; // 10 segundos promedio digamos a 40fps

        public int Score { get; private set; }

        public int Lives { get; private set; } = 3;

        private int _amountOfPelletsEaten = 0;
        private const int _pelletPoints = 10; // es constante osea no se cambia
        private const int _powepelletPoints = 50;
        private const int _ghostPoints = 200;
        private const double _cherrySpawnInterval = 30.0; // cada 30 segundos aparece
        private double _cherrySpawnTimer = 0;// timer para cherry
        private Cherry? _currentCherry = null; // para saber si existe
        private readonly (int row, int col)[] _cherryPositions =
        [
            (1, 2), //Esquina superior izquierda
            (29, 2), //Equina inferior izquierda
            (2, 26), //Esquina superior derecha
            (29, 26) //Esquina inferior derecha
        ];

        private Random _random = new Random();

        public GameEngine()
        {
            _lastUpdateTime = DateTime.Now;
        }

        // LLamado en cada Tick
        public void Update()
        {
            //hora actual - hora anterior = diff
            DateTime now = DateTime.Now;
            double diff = (now - _lastUpdateTime).TotalSeconds;
            _lastUpdateTime = now;

            TotalTime += diff;

            _frameCount++;

            _fpsTimer += diff;

            if (_fpsTimer >= 1.0)
            {
                CurrentFPS = _frameCount;
                _frameCount = 0;
                _fpsTimer = 0;
            }

            _cherrySpawnTimer += diff;
            if ( _cherrySpawnTimer >= _cherrySpawnInterval)
            {
                SpawnCherry();
                _cherrySpawnTimer = 0;
            }
           
            //revisar si esta asustado el fantasma
            if (_isFrightenModeActive)
            {
                _frightenFramesRemaining--;

                if (_frightenFramesRemaining <= 0)
                {
                    UnFrightenGhosts();
                    _isFrightenModeActive = false;
                }
            }

            //actualizar la IA del fantasma
            var pacman = GameObjects.OfType<Pacman>().FirstOrDefault();
            var ghosts = GameObjects.OfType<Ghost>().ToList();

            if (pacman != null && ghosts.Count > 0)
            {
                foreach (var ghost in ghosts)
                {
                    ghost.DecideNextMove(pacman, ghosts);
                }
            }

            for (int i = GameObjects.Count - 1; i >= 0; i--)
            {
                var obj = GameObjects[i];

                if (!obj.IsActive)
                {
                    GameObjects.RemoveAt(i);
                    continue;
                }

                obj.Update(diff);

            }

            CheckPelletCollision();
            CheckGhostColission();
            CheckCherryColission();
        }

        public void AddGameObject(GameObject obj)
        {
            GameObjects.Add(obj);
        }

        public void RemoveGameObject(GameObject obj)
        {
            GameObjects.Remove(obj); 
        }

        public void Reset()
        {

        }

        public void CheckPelletCollision()
        {
            if (Map == null) return;

            var pacman = GameObjects.OfType<Pacman>().FirstOrDefault();
            if (pacman == null) return;

            if (pacman.State != Pacman.PacmanState.Normal &&
                pacman.State != Pacman.PacmanState.Invincible)
            {
                return;
            }

            //obtener celda del pacman
            var (row, col) = Map.WorldToTile(pacman.X + pacman.Width /2,
                pacman.Y + pacman.Height /2);

            //buscar las pildoras de la celda
            var pelletsToEat = GameObjects.OfType<Pellet>()
                .Where(p => p.IsActive)
                .ToList();

            foreach (var pellet in pelletsToEat)
            {
                //obtener la celda
                var (pelletRow, pelletCol) = Map.WorldToTile(
                pellet.X + pellet.Width / 2,
                pellet.Y + pellet.Height / 2
                );

                // si estan en la misma celda
                if (pelletRow == row && pelletCol == col)
                {
                    _amountOfPelletsEaten++;
                    SoundManager.PlaySound("pacman_chomp");
                    pellet.IsActive = false;

                    if (pellet.IsEnergizer)
                    {
                        Score += _powepelletPoints;
                        FrightenGhosts();
                    }
                    else
                    {
                        Score += _pelletPoints;
                        //se come una normal
                    }

                    if(_amountOfPelletsEaten >= 20)
                    {
                        GameWin?.Invoke();
                    }

                    Map.SetTile(row, col, MapTileType.Empty);
                }
            }
        }

        public void CheckGhostColission()
        {
            if (Map == null) return;

            var pacman = GameObjects.OfType<Pacman>().FirstOrDefault();
            if (pacman == null) return;

            if (pacman.State != Pacman.PacmanState.Normal)
            {
                return;
            }

            //obtener celda del pacman
            var (pacmanRow, pacmanCol) = Map.WorldToTile(pacman.X + pacman.Width / 2,
                pacman.Y + pacman.Height / 2);

            var ghosts = GameObjects.OfType<Ghost>()
                .Where(g => g.IsActive && !g.IsDead)
                .ToList();

            foreach (var ghost in ghosts)
            {
                // obtener la celda donde esta
                var (ghostRow, ghostCol) = Map.WorldToTile(
                    ghost.X + ghost.Width / 2,
                    ghost.Y + ghost.Height / 2);

                if (ghostRow == pacmanRow && ghostCol == pacmanCol)
                {
                    if (ghost.IsEatable)
                    {
                        // el pacman come al fantasma
                        ghost.IsDead = true;
                        Score += _ghostPoints;
                    }
                    else
                    {
                        Lives--;
                        pacman.Die(); // quitarle una vida agregar horita
                        PacmanDied?.Invoke();

                        if (Lives <= 0)
                        {
                            GameOver?.Invoke();
                        }
                        else
                        {
                            ResetGhosts();
                        }
                        return;
                    }
                }
            }
        }
        public void CheckCherryColission()
        {
            if (Map == null)
            {
                return;
            }

            var pacman = GameObjects.OfType<Pacman>().FirstOrDefault();
            if (pacman == null)
            {
                return; 
            }

            if (pacman.State != Pacman.PacmanState.Normal &&
                pacman.State != Pacman.PacmanState.Invincible)
            {
                return;
            }

            var cherry = GameObjects.OfType<Cherry>().FirstOrDefault(c => c.IsActive);
            if (cherry == null)
            {
                return;
            }

            var (pacmanRow, pacmanCol) = Map.WorldToTile(
                pacman.X + pacman.Width / 2,
                pacman.Y + pacman.Height / 2);

            var (cherryRow, cherryCol) = Map.WorldToTile(
            cherry.X + cherry.Width / 2,
            cherry.Y + cherry.Height / 2);

            if (pacmanRow == cherryRow && pacmanCol == cherryCol)
            {
                Score += cherry.BonusPoint;
                cherry.IsActive = false;
                _currentCherry = null;
                if (Lives < 3)
                {
                    Lives++;
                }
                //desaparece la cherry actual para que pueda aparecer otra
            }
        }

        private void SpawnCherry()
        {
            if (Map == null)
            {
                return; 
            }

            //solo una a la vez
            if (_currentCherry != null && _currentCherry.IsActive) 
            {
                return;
            }

            //elegir entre las cuatro posiciones
            var randomPosition = _cherryPositions[_random.Next(_cherryPositions.Length)];
            var (x, y) = Map.TileToWorld(randomPosition.row, randomPosition.col);

            var cherrySprite = SpriteManager.LoadSprite("CherrySprite.png");
            _currentCherry = new Cherry(x, y, cherrySprite);

            AddGameObject(_currentCherry);
         }
        private void ResetGhosts()
        {
            var ghosts = GameObjects.OfType<Ghost>().ToList();

            foreach (var ghost in ghosts)
            {
                ghost.X = ghost.SpawnX;
                ghost.Y = ghost.SpawnY;
                ghost.IsEatable = false;
                ghost.IsDead = false;
                ghost.Speed = 60.0;
                ghost.Direction = GhostDirection.Up;
            }

            _isFrightenModeActive = false;
            _frightenFramesRemaining = 0;
        }

        private void FrightenGhosts()
        {
            var ghosts = GameObjects.OfType<Ghost>().ToList();

            foreach (var ghost in ghosts)
            {
                if (!ghost.IsDead)
                {
                    ghost.Frighten();
                }
            }
            //activa el tiempo que va a durar
            _isFrightenModeActive = true;
            _frightenFramesRemaining = _frightenFramesDuration;
        }

        public void UnFrightenGhosts()
        {
            var ghosts = GameObjects.OfType<Ghost>().ToList();
            
            foreach (var ghost in ghosts)
            {
                if (!ghost.IsDead)
                {
                    ghost.UnFrighten();
                }
            }
        }
        public void CreatePellets()
        {
            if (Map == null) return;

            var pointPoisitions = Map.GetTilesOfType(MapTileType.Point);
            foreach (var (row, col) in pointPoisitions)
            {
                var (x, y) = Map.TileToWorld(row, col);
                var pellet = new Pellet(x, y, isEnergizer: false);
                AddGameObject(pellet);
            }

            var powerPositions = Map.GetTilesOfType(MapTileType.PowerPoint);
            foreach (var (row, col) in powerPositions)
            {
                var (x, y) = Map.TileToWorld(row, col);
                var pellet = new Pellet(x, y, isEnergizer: true);
                AddGameObject(pellet);
            }
        }

    }
}
