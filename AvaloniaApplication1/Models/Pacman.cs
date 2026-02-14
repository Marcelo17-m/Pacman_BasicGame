using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    internal class Pacman : GameObject
    {
        private readonly GameMap? _map;
        public enum PacmanDirection
        {
            Up, Down, Left, Right
        }

        public PacmanDirection Direction { get; set; } = PacmanDirection.Up;
        public PacmanDirection? NextDirection { get; set; } = null;

        public double Speed { get; set; } = 60.0; //basicamente los fps

        private readonly int _spriteSize;

        public Pacman(double x, double y, Bitmap? sprite = null, GameMap? map = null, int spriteSize = 16)
        {
            X = x;
            Y = y;
            _map = map;
            _spriteSize = spriteSize;
            Width = spriteSize - 1.5;
            Height = spriteSize - 1.5; // esto es para hacerlo mas pequeño de la celda del mapa.
            Zindex = 6;
            Sprite = sprite;

            UpdateSpriteRect();
        }

        private void UpdateSpriteRect()
        {
            if (Sprite == null)
            {
                SourceRect = null;
                return; 
            }
            
            int spriteX = 16;
            int spriteY = 0; // Si los sprites están en múltiples filas, ajusta este valor

            switch (Direction)
            {
                case PacmanDirection.Right:
                    spriteY = 0 * _spriteSize; // Primer frame
                    break;

                case PacmanDirection.Left:
                    spriteY = 1 * _spriteSize; // Segundo frame
                    break;

                case PacmanDirection.Up:
                    spriteY = 2 * _spriteSize; // Tercer frame
                    break;

                case PacmanDirection.Down:
                    spriteY = 3 * _spriteSize; // Cuarto frame
                    break;
                default:
                    spriteY = 1; 
                    break;
            }

            SourceRect = new Avalonia.Rect(spriteX, spriteY, _spriteSize, _spriteSize);
        }

        public void SetNextDirection(PacmanDirection newDirection)
        {
            NextDirection = newDirection;
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (_map == null) return;

            // Mover el Pacman según su dirección y velocidad   
            double moveDistance = Speed * deltaTime;

            // esto es para saber si se puede mover hacia algun lado
            if (NextDirection.HasValue)
            {
                double testX = X;
                double testY = Y;

                switch (NextDirection.Value)
                {
                    case PacmanDirection.Up:
                        testY -= moveDistance;
                        break;
                    case PacmanDirection.Down:
                        testY += moveDistance;
                        break;
                    case PacmanDirection.Left:
                        testX -= moveDistance;
                        break;
                    case PacmanDirection.Right:
                        testX += moveDistance;
                        break;
                }

                if (_map.CanMoveTo(testX, testY, Width, Height))
                {
                    // alinearlo pq falla
                    AllignToGrid(NextDirection.Value);

                    //ahora si girar
                    Direction = NextDirection.Value;
                    NextDirection = null;
                    UpdateSpriteRect();
                }
            }

            // esto ya es para que se mueva normal
            double newX = X;
            double newY = Y;

            switch (Direction)
            {
                case PacmanDirection.Up:
                    newY -= moveDistance;
                    break;
                case PacmanDirection.Down:
                    newY += moveDistance;
                    break;
                case PacmanDirection.Left:
                    newX -= moveDistance;
                    break;
                case PacmanDirection.Right:
                    newX += moveDistance;
                    break;
            }

            if (_map.CanMoveTo(newX, newY, Width, Height))
            {
                X = newX;
                Y = newY;
            }
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
    }
}
