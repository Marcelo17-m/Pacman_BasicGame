using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models.Ghosts.Ghosts
{
    public interface IGhostBehavior
    {
        GhostDirection DecideNextDirection(
            Ghost ghost,
            Pacman pacman,
            GameMap map,
            List<Ghost>? allGhosts = null);
    }
    
    public enum GhostDirection
    {
        Up, Down, Left, Right   
    }

    public enum GhostType
    {
        Blinky, // rojo
        Pinky, // rosa
        Inky, // azul
        Clyde, // naranja
    }
}
