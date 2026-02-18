using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public partial class GameObject 
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int Zindex { get; set; }
        public Bitmap? Sprite { get; set; }
        public Rect? SourceRect { get; set; }
        public bool IsActive { get; set; } = true;
        public IBrush? FillColor { get; set; }

        //deltatime es el tiempo desde el ultimo frame
        public virtual void Update(double deltaTime)
        {

        }
    }
}
