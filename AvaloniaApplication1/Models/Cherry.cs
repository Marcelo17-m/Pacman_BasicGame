using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public class Cherry : GameObject
    {
        public double LifeTimer { get; set; } = 0;
        public const double MaxLifeTime = 20.0;
        public int BonusPoint { get; set; } = 500;

        public Cherry (double x, double y, Bitmap? sprite = null, int spriteSize = 16)
        {
            X = x;
            Y = y;
            Width = spriteSize;
            Height = spriteSize;
            Zindex = 4; // mismo de los ghost
            Sprite = sprite;
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            LifeTimer += deltaTime;

            if (LifeTimer > MaxLifeTime)
            {
                IsActive = false;
            }
        }
    }
}
