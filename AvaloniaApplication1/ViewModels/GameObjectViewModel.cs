using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaApplication1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.ViewModels
{
    public partial class GameObjectViewModel : ViewModelBase
    {
        [ObservableProperty]
        private double _x;

        [ObservableProperty]
        private double _y;

        [ObservableProperty]
        private double _width;

        [ObservableProperty]
        private double _height;

        [ObservableProperty]
        private int _zindex;

        [ObservableProperty]
        private Bitmap? _sprite;

        [ObservableProperty]
        private Rect? _sourceRect;

        [ObservableProperty]
        private bool _isActive;

        [ObservableProperty]
        private IBrush? _fillColor;

        public GameObject Model { get; }

        public GameObjectViewModel(GameObject model) 
        {
            Model = model;
            SyncFromModel();
        }

        public void SyncFromModel()
        {
            X = Model.X;
            Y = Model.Y;
            Width = Model.Width;
            Height = Model.Height;
            Zindex = Model.Zindex;
            Sprite = Model.Sprite;
            SourceRect = Model.SourceRect;
            IsActive = Model.IsActive;
            FillColor = Model.FillColor;
        }

    }
}
