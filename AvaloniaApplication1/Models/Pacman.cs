using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public class Pacman : GameObject
    {
        private readonly GameMap? _map;
        public enum PacmanDirection
        {
            Up, Down, Left, Right
        }

        public enum PacmanState
        {
            Normal, Dying, Dead, Invincible
        }

        public PacmanDirection Direction { get; set; } = PacmanDirection.Up;
        public PacmanDirection? NextDirection { get; set; } = null;
        public PacmanState State { get; set; } = PacmanState.Normal;

        public double Speed { get; set; } = 60.0; //basicamente los fps

        private int _animationFrame = 0; // talvez añadir a gameObject
        private int _animationCounter = 0;
        private int _spriteSize { get; set; }
        private double _stateTimer = 0; //time para controlar estados

        // posicion inicial para respawnear
        public double SpawnX { get; set; }
        public double SpawnY { get; set; }

        public Pacman(double x, double y, Bitmap? sprite = null, GameMap? map = null, int spriteSize = 16)
        {
            X = x;
            Y = y;
            SpawnX = x;
            SpawnY = y;
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
                SourceRect = new Avalonia.Rect(0, 0, 0, 0);
                return; 
            }

            int spriteX = _animationFrame * _spriteSize;
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

            _animationCounter++;
            if (_animationCounter >= 6) // cada 6 frames
            {
                _animationCounter = 0;
                _animationFrame = (_animationFrame == 0) ? 1 : 0; // Alterna entre 0 y 1
                UpdateSpriteRect();
            }

            switch (State)
            {
                case PacmanState.Dying:
                    UpdateDyingState(deltaTime);
                    return;
                case PacmanState.Dead:
                    UpdateDeadState(deltaTime);
                    return;
                case PacmanState.Invincible:
                    UpdateInvincibleState(deltaTime);
                    break;
            }

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

        private void UpdateDyingState(double deltatime)
        {
            _stateTimer += deltatime;

            if (_stateTimer >= 2.0)
            {
                State = PacmanState.Dead;
                _stateTimer = 0;
                // desaparecerlo momentanamente
                _spriteSize = 0;
                Width = 0;
                Height = 0;
            }
        }

        private void UpdateDeadState(double deltatime)
        {
            _stateTimer += deltatime;

            if ( _stateTimer >= 3.0)
            {
                Respawn();
            }
        }

        private void UpdateInvincibleState(double deltatime)
        {
            _stateTimer += deltatime;

            if (_stateTimer >= 2.0)
            {
                State = PacmanState.Normal;
                _stateTimer = 0;
            }
        }

        public void Die()
        {
            if (State == PacmanState.Dead || State == PacmanState.Dying)
            {
                return;
            }

            State = PacmanState.Dying;
            _stateTimer = 0;
            Speed = 0;
        }

        public void Respawn()
        {
            X = SpawnX;
            Y = SpawnY;
            Direction = PacmanDirection.Right;
            NextDirection = null;
            Speed = 60.0;
            State = PacmanState.Invincible;
            _stateTimer = 0;
            _spriteSize = 16;
            Width = _spriteSize - 1.5;
            Height = _spriteSize - 1.5;

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
                    // Si va a girar vertical, alinear en X
                    X = tileCenterX - Width / 2;
                    break;

                case PacmanDirection.Left:
                case PacmanDirection.Right:
                    // Si va a girar horizontal, alinear en Y
                    Y = tileCenterY - Height / 2;
                    break;
            }
        }
    }
}
