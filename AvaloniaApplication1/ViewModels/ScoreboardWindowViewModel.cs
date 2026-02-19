using AvaloniaApplication1.Engine;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.ViewModels
{
    public partial class ScoreboardWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private int _score;

        [ObservableProperty]
        private string _elapsedTime = "0.0s";

        [ObservableProperty]
        private int _currentFPS;

        [ObservableProperty]
        private int _lives = 3;

        public void UpdateFromEngine(GameEngine engine)
        {
            Score = engine.Score;
            ElapsedTime = $"{engine.TotalTime:F1}s";
            CurrentFPS = engine.CurrentFPS;
            Lives = engine.Lives;
        }
    }
}
