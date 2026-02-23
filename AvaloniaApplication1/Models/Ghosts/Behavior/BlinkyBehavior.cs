using AvaloniaApplication1.Models.Ghosts.Ghosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models.Ghosts.Behavior
{
    public class BlinkyBehavior : IGhostBehavior
    {
        public GhostDirection DecideNextDirection(Ghost ghost, Pacman pacman, GameMap map, List<Ghost>? allGhosts = null)
        {
            //posicion del pacman
            double targetX = pacman.X;
            double targetY = pacman.Y;

            return GetBestDirectionToTarget(ghost, targetX, targetY, map);
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

        //para que no se gire de la nada si no que diga por su camino y gire en algun lado
        private bool IsOpposite(GhostDirection dir1, GhostDirection dir2)
        {
            return (dir1 == GhostDirection.Up &&  dir2 == GhostDirection.Down) ||
                   (dir1 == GhostDirection.Down && dir2 == GhostDirection.Up) ||
                   (dir1 == GhostDirection.Left && dir2 == GhostDirection.Right) ||
                   (dir1 == GhostDirection.Right && dir2 == GhostDirection.Left);
        }
    }
}
