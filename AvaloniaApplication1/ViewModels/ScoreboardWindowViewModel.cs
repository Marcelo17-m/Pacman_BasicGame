using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Models.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.ViewModels
{
    public partial class ScoreboardWindowViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _main;
        public ObservableCollection<Score> HighScores { get; } = new();

        public ScoreboardWindowViewModel(MainWindowViewModel main)
        {
            _main = main;
            LoadHighScores();
        }

        public void LoadHighScores()
        {
            var allScores = ScoreService.LoadScores();

            var topScores = allScores?.OrderByDescending(s => s.Points)
                .Take(5)
                .ToList();

            //actualizar la lista
            HighScores.Clear();
            foreach (var score in topScores)
            {
                HighScores.Add(score);
            }
            
        }

        [RelayCommand]
        private void Back()
        {
            _main.GoBackToMenu();
        }
    }
}
