using Avalonia.Platform;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Engine
{
    public class SoundManager
    {
        private WaveOutEvent? _outputDevice;
        private WaveFileReader? _audioFile;
        private bool _shouldLoop;

        public void PlaySound(string nameSong, bool isLooping = false)
        {
            StopSound();
            _shouldLoop = isLooping;

            try
            {
                var stream = AssetLoader.Open(
                    new Uri($"avares://AvaloniaApplication1/Assets/Media/{nameSong}.wav")
                    );

                // uso Naudio para asi hacer sonar esto me lo robe de su libreria
                _audioFile = new WaveFileReader(stream);
                _outputDevice = new WaveOutEvent();

                _outputDevice.Init(_audioFile);

                //utilizo el evento que tienen para suscribir que se repita si se para siga
                _outputDevice.PlaybackStopped += OnPlaybackStooped;

                _outputDevice.Play();
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
            }
        }

        private void OnPlaybackStooped(object sender, StoppedEventArgs e)
        {
            //si se acaba el audio se repite
            if (_shouldLoop && _audioFile != null && _outputDevice != null)
            {
                _audioFile.Position = 0;
                _outputDevice.Play();
            }
        }

        public void StopSound()
        {
            //elimina toda cosa que halla quedado del sonido anterior.
            _shouldLoop = false;
            if (_outputDevice != null)
            {
                _outputDevice.Stop();
                _outputDevice.Dispose();
                _outputDevice = null;
            }

            if (_audioFile != null)
            {
                _audioFile.Dispose();
                _audioFile = null;
            }
        }
    }
}
