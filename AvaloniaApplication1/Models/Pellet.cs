using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public class Pellet : GameObject
    {
        public bool IsEnergizer { get; }
        public Pellet(double x, double y, bool isEnergizer = false) 
        { 
            X = x + 8 - (isEnergizer ? 4: 1.5); //centro de los 16px
            Y = y + 8 - (isEnergizer ? 4 : 1.5);
            IsEnergizer = isEnergizer;
            Width = isEnergizer ? 8 : 3; //dependiendo si es power up o no le da un tamaño
            Height = isEnergizer ? 8 : 3;
            Zindex = 3;
            IsActive = true;

            FillColor = isEnergizer ? Brushes.Yellow : Brushes.White;
        }

        public override void Update(double deltaTime)
        {
            // actualizarlo para ver si fue comida
        }

    }
}
