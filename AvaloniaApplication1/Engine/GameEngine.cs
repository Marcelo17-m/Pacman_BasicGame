using AvaloniaApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        
        public GameMap? Map { get; set; }

        public event Action<int, int>? PelletEaten;

        private int _frameCount;

        private double _fpsTimer;

        private DateTime _lastUpdateTime;
        public int Score { get; private set; }

        private const int _pelletPoints = 10;

        private int _powepelletPoints = 50;

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
                    pellet.IsActive = false;

                    if (pellet.IsEnergizer)
                    {
                        Score += _powepelletPoints;
                        //luego agregar el modo matar fantasmas
                    }
                    else
                    {
                        Score += _pelletPoints;
                        //se come una normal
                    }

                    Map.SetTile(row, col, MapTileType.Empty);
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
