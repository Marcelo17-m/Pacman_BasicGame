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
        private int _maxX = 785; //averiguar como sacarlo directamente del canvas
        private int _maxY = 435; // 36 mide lo de arriba para el min
        public enum PacmanDirection
        {
            Up, Down, Left, Right
        }

        public PacmanDirection Direction { get; set; } = PacmanDirection.Up;

        public double Speed { get; set; } = 60.0;

        private readonly int _spriteSize;

        public Pacman(double x, double y, Bitmap? sprite = null, int spriteSize = 15)
        {
            X = x;
            Y = y;
            _spriteSize = spriteSize;
            Width = spriteSize;
            Height = spriteSize;
            Zindex = 2;
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
            
            int spriteX = 15;
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
                    spriteY = 2 * _spriteSize +2; // Tercer frame
                    break;

                case PacmanDirection.Down:
                    spriteY = 3 * _spriteSize +2; // Cuarto frame
                    break;
                default:
                    spriteY = 1; 
                    break;
            }

            SourceRect = new Avalonia.Rect(spriteX, spriteY, _spriteSize, _spriteSize);
        }

        public void SetDirection(PacmanDirection newDirection)
        {
            Direction = newDirection; 
            UpdateSpriteRect();
        }


         
        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            
            // Mover el Pacman según su dirección y velocidad
            double moveDistance = Speed * deltaTime;
            
            switch (Direction)
            {
                case PacmanDirection.Up:
                    Y -= moveDistance;
                    break;
                case PacmanDirection.Down:
                    Y += moveDistance;
                    break;
                case PacmanDirection.Left:
                    X -= moveDistance;
                    break;
                case PacmanDirection.Right:
                    X += moveDistance;
                    break;
            }

            X = Math.Clamp(X, 0, _maxX);
            Y = Math.Clamp(Y, 40, _maxY);
            // ese 40 es para dejar espacio arriba
        }
    }
}
