using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public partial class GameObject : ObservableObject
    {
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private int _zindex;
        private Bitmap? _sprite;
        private Rect? _sourceRect;
        private bool _isActive = true;
        private IBrush? _fillColor;

        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public int Zindex
        {
            get => _zindex;
            set => SetProperty(ref _zindex, value);
        }

        public Bitmap? Sprite
        {
            get => _sprite;
            set => SetProperty(ref _sprite, value);
        }

        public Rect? SourceRect
        {
            get => _sourceRect;
            set => SetProperty(ref _sourceRect, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public IBrush? FillColor
        {
            get => _fillColor;
            set => SetProperty(ref _fillColor, value);
        }


        //deltatime es el tiempo desde el ultimo frame
        public virtual void Update(double deltaTime)
        {

        }
    }
}
