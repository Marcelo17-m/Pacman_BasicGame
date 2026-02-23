using AvaloniaApplication1.Models.Ghosts.Ghosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models.Ghosts.Behavior
{
    public class InkyBehavior : IGhostBehavior
    {
        // se distrae cada cierto tiempo
        private readonly Random _random = new Random();
        private int _decisionCounter = 0;
        private const int _randomDecisionUse = 240; //cada unos 6 segundos mas o menos si va como a 40fps
        public GhostDirection DecideNextDirection(
            Ghost ghost,
            Pacman pacman,
            GameMap map,
            List<Ghost>? allGhost = null)
        {
            _decisionCounter++;

            if (_decisionCounter >= _randomDecisionUse)
            {
                _decisionCounter = 0;

                if (_random.Next(0, 100) < 33) // 33% de probabilidad
                {
                    return GetRandomDirection(ghost, map);
                }
            }
            var blinky = allGhost?.FirstOrDefault(g => g.Type == GhostType.Blinky);

            // si no encuentra a blinky actua como el
            if (blinky == null)
            {
                return new BlinkyBehavior().DecideNextDirection(ghost, pacman, map);
            }

            //calculamos la posicion del pacman de primeras
            double midX = pacman.X;
            double midY = pacman.Y;

            switch (pacman.Direction)
            {
                case Pacman.PacmanDirection.Up:
                    midX -= 2 * 16;
                    break;
                case Pacman.PacmanDirection.Down:
                    midX += 2 * 16;
                    break;
                case Pacman.PacmanDirection.Left:
                    midY -= 2 * 16;
                    break;
                case Pacman.PacmanDirection.Right:
                    midY += 2 * 16;
                    break;
            }
            //target va a ser el reflejo de blinky con respecto al punto
            //medio que sacamos arriba
            //mid va a estar 2 tiles adelante de pacman.
            double targetX = midX + (midX - blinky.X);
            double targetY = midY + (midY - blinky.Y);
            //el target va a estar al otro lado de blinky estilo espejo
            //

            return GetBestDirectionToTarget(ghost, targetX, targetY, map);
        }

        private GhostDirection GetRandomDirection(Ghost ghost, GameMap map)
        {
            var possibleDirections = new List<GhostDirection>();

            foreach (GhostDirection dir in Enum.GetValues<GhostDirection>())
            {
                if (IsOpposite(dir, ghost.Direction)) continue;

                double testX = ghost.X;
                double testY = ghost.Y;

                switch (dir)
                {
                    case GhostDirection.Up:
                        testY -= 16;
                        break;
                    case GhostDirection.Down:
                        testY += 16;
                        break;
                    case GhostDirection.Left:
                        testX -= 16;
                        break;
                    case GhostDirection.Right:
                        testX += 16;
                        break;
                }

                if (map.CanMoveTo(testX, testY, ghost.Width, ghost.Height))
                {
                    possibleDirections.Add(dir);
                }
            }
            // elegir una direccion al azar del as posibles
            if (possibleDirections.Count > 0)
            {
                return possibleDirections[_random.Next(possibleDirections.Count)];
                //si ahy tres direcciones digamos
                //el random va a escoger uno aleatorio para ir
            }
            return ghost.Direction;
        }

        private GhostDirection GetBestDirectionToTarget(Ghost ghost, double targetX, double targetY, GameMap map)
        {
            var possibleDirections = new List<(GhostDirection dir, double distance)>();

            // probar cada direccion y calcular la distancia al objetivo
            foreach (GhostDirection dir in Enum.GetValues<GhostDirection>())
            {
                // No retrocede (
                if (IsOpposite(dir, ghost.Direction))
                {
                    continue;
                }

                double testX = ghost.X;
                double testY = ghost.Y;

                switch (dir)
                {
                    case GhostDirection.Up:
                        testY -= 16;
                        break;
                    case GhostDirection.Down:
                        testY += 16;
                        break;
                    case GhostDirection.Left:
                        testX -= 16;
                        break;
                    case GhostDirection.Right:
                        testX += 16;
                        break;
                }

                if (map.CanMoveTo(testX, testY, ghost.Width, ghost.Height))
                {
                    //formula para calcular que tan lejos quedaria el fantasma del pacman
                    // si se mueve a esa direccion
                    double dx = targetX - testX;
                    double dy = targetY - testY;

                    double distanceSquarted = dx * dx + dy * dy;
                    //calcula cual es menor

                    possibleDirections.Add((dir, distanceSquarted));
                }
            }

            // elige la direccion que se acerca mas al objetivo
            if (possibleDirections.Count > 0)
            {
                // las ordena para tener la primera mas cercana
                return possibleDirections.OrderBy(d => d.distance).First().dir;
            }

            return ghost.Direction; // seguir la misma direccion si no hay mas opciones
        }

        private bool IsOpposite(GhostDirection dir1, GhostDirection dir2)
        {
            return (dir1 == GhostDirection.Up && dir2 == GhostDirection.Down) ||
                   (dir1 == GhostDirection.Down && dir2 == GhostDirection.Up) ||
                   (dir1 == GhostDirection.Left && dir2 == GhostDirection.Right) ||
                   (dir1 == GhostDirection.Right && dir2 == GhostDirection.Left);
        }
    }
}
