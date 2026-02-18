using Avalonia.Media.Imaging;
using AvaloniaApplication1.Models.Ghosts.Ghosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AvaloniaApplication1.Models.Pacman;

namespace AvaloniaApplication1.Models.Ghosts
{
    public class Ghost : GameObject
    {
        private readonly GameMap? _map;
        private readonly IGhostBehavior _behavior;
        private int _animationFrame = 0;

        public GhostDirection Direction { get; set; } = GhostDirection.Up;
        public GhostType Type { get; set; }
        public double Speed { get; set; } = 60.0; // los fps a los que corre
        public bool IsEatable { get; set; } = false; // cuando se pone azul comestible
        public bool IsDead { get; set; } = false;
        private readonly int _spriteSize;
        public Ghost(double x, double y, GhostType type, IGhostBehavior behavior, Bitmap? sprite = null, GameMap? map = null, int spriteSize = 16)
        {
            X = x;
            Y = y;
            Type = type;
            _behavior = behavior;
            _map = map;
            _spriteSize = spriteSize;
            Width = spriteSize - 1.5;
            Height = spriteSize - 1.5; // por si acaso para que no gire mal
            Zindex = 5; // uno menos que el pacman
            Sprite = sprite;

            UpdateSpriteRect();
        }

        private void UpdateSpriteRect()
        {
            if (Sprite == null)
            {
                SourceRect = new Avalonia.Rect(0, 0, 0, 0); //inicialzar en 0 no en null
                return;
            }

            int spriteX = 0;
            int spriteY = 0;

            if (IsDead)
            {
                spriteY = 16;

               spriteX = (_animationFrame % 2 == 0) ? 8 * _spriteSize : 9 * _spriteSize;
               // mismo tipo de animación pero esta vez una linea mas abajo
               SourceRect = new Avalonia.Rect(spriteX, spriteY, _spriteSize, _spriteSize);
               return;
            }

            if (IsEatable)
            {
                spriteY = 0;

                //animación sencilla
                spriteX = (_animationFrame % 2 == 0) ? 8 * _spriteSize : 9 * _spriteSize;
                //basicamente como se va a ir sumando ese animation frame lo que va a ser
                // es alternar entre 0 y 1 para cambiar el sprite al otro para generar
                //una animacion sencilla

                SourceRect = new Avalonia.Rect(spriteX, spriteY, _spriteSize, _spriteSize);
                return; // salirse para que no compare mas cosas pues ya todos serian lo mismo
            }

            if (Type == GhostType.Blinky)
            {

            }
            else if (Type == GhostType.Pinky)
            {
                spriteY = 16;
            }
            else if (Type == GhostType.Inky)
            {
                spriteY = 32;
            }
            else if (Type == GhostType.Clyde)
            {
                spriteY = 48;
            }



            switch (Direction)
            {
                case GhostDirection.Right:
                    spriteX = 0 * _spriteSize; // Primer frame
                    break;

                case GhostDirection.Left:
                    spriteX = 2 * _spriteSize; // Segundo frame
                    break;

                case GhostDirection.Up:
                    spriteX = 4 * _spriteSize; // Tercer frame
                    break;

                case GhostDirection.Down:
                    spriteX = 6 * _spriteSize; // Cuarto frame
                    break;
                default:
                    spriteX = 1;
                    break;
            }

            SourceRect = new Avalonia.Rect(spriteX, spriteY, _spriteSize, _spriteSize);
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (_map == null) return;

            _animationFrame++;

            // validacion para volver a la casa automaticamente
            if (IsDead)
            {
                MoveTowardsHome(deltaTime);
                return;
            }

            // El fantasma se mueve según su dirección y velocidad   
            double moveDistance = Speed * deltaTime;

            double newX = X;
            double newY = Y;

            switch (Direction)
            {
                case GhostDirection.Up:
                    newY -= moveDistance;
                    break;
                case GhostDirection.Down:
                    newY += moveDistance;
                    break;
                case GhostDirection.Left:
                    newX -= moveDistance;
                    break;
                case GhostDirection.Right:
                    newX += moveDistance;
                    break;
            }

            if (_map.CanMoveTo(newX, newY, Width, Height))
            {
                X = newX;
                Y = newY;
            }

            UpdateSpriteRect();

        }

        private void AllignToGrid(PacmanDirection newDirection)
        {
            if (_map == null) return;

            double centerX = X + Width / 2;
            double centerY = Y + Height / 2;

            var (row, col) = _map.WorldToTile(centerX, centerY);

            var (tileX, tileY) = _map.TileToWorld(row, col);

            double tileCenterX = tileX + _map.TileWidth / 2;
            double tileCenterY = tileY + _map.TileHeight / 2;

            switch (newDirection)
            {
                case PacmanDirection.Up:
                case PacmanDirection.Down:
                    X = tileCenterX - Width / 2;
                    break;

                case PacmanDirection.Left:
                case PacmanDirection.Right:
                    Y = tileCenterY - Height / 2;
                    break;
            }
        }

        public void DecideNextMove(Pacman pacman, List<Ghost>? allGhost = null)
        {
            if (_map == null || IsDead || IsEatable)
            {
                return;
            }

            var newDirection = _behavior.DecideNextDirection(this, pacman, _map, allGhost);

            if (CanTurnTo(newDirection))
            {
                Direction = newDirection;
            }
        }

        private bool CanTurnTo(GhostDirection direction)
        {
            if (_map == null)
            {
                return false; 
            }

            double testX = X;
            double testY = Y;
            double testDistance = Speed * 0.016; // un tick por delante como va 60fps

            switch (direction)
            {
                case GhostDirection.Up:
                    testY -= testDistance;
                    break;
                case GhostDirection.Down:
                    testY += testDistance;
                    break;
                case GhostDirection.Left:
                    testX -= testDistance;
                    break;
                case GhostDirection.Right:
                    testX += testDistance;
                    break;
            }

            return _map.CanMoveTo(testX, testY, Width, Height);
        }

        private void MoveTowardsHome(double deltatime)
        {
            // logica para volver a la casa
        }

        public void Frighten()
        {
            IsEatable = true;
            Speed = 30.0; // se vuelve comestible y aparte la velocidad se reduce
        }

        public void UnFrighten()
        {
            IsEatable = false;
            Speed = 60.0;
        }
    }
}
