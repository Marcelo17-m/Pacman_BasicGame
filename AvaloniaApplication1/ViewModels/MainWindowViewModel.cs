using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using AvaloniaApplication1.Engine;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;

namespace AvaloniaApplication1.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        SoundManager SoundManager = new();
        [ObservableProperty]
        private ViewModelBase? _currentPage;

        [ObservableProperty]
        private bool _isMusicEnable = false;

        public MainWindowViewModel()
        {
            _currentPage = null;
        }

        [RelayCommand]
        public void GoGame()
        {
            CurrentPage = new GameWindowViewModel(this);
        }

        [RelayCommand]
        public void GoScoreboard()
        {
            CurrentPage = new ScoreboardWindowViewModel(this);
        }

        public void GoBackToMenu()
        {
            CurrentPage = null;
        }

        [RelayCommand]
        private void Exit()
        {
            Environment.Exit(0); // manera sencilla

            // if (Application.Current?.ApplicationLifetime
            // is IClassicDesktopStyleApplicationLifetime desktop)
            // {
            // desktop.Shutdown();
            // }
        }

        [RelayCommand]
        private void ToggleAudio()
        {
            string namesong = "PacManOriginalThemeTheCantinaBand";
            if (IsMusicEnable)
            {
                SoundManager.PlaySound(namesong, true);
            }
            else
            {
                SoundManager.StopSound();
            }
        }

    }
}
