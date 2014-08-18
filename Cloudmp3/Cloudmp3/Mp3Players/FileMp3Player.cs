using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudmp3.Mp3Players
{
    public class FileMp3Player
    {
        public enum Mp3PlayerState
        {
            Playing,
            Stopped,
            Paused
        }

        private MediaFoundationReader _mp3Reader;
        private IWavePlayer _waveOutDevice;

        public Mp3PlayerState PlayerState { get; private set; }
        public int CurrentSongIndex { get; set; }

        public FileMp3Player()
        {
            CurrentSongIndex = -1;
            PlayerState = Mp3PlayerState.Stopped;
        }

        public void Play(string path, int selectedIndex)
        {
            if (PlayerState == Mp3PlayerState.Paused && CurrentSongIndex == selectedIndex)
            {
                PlayerState = Mp3PlayerState.Playing;
                _waveOutDevice.Play();
            }
            else
            {
                if (PlayerState != Mp3PlayerState.Stopped)
                {
                    clearPlayer();
                }
                if (selectedIndex == -1 && PlayerState == Mp3PlayerState.Stopped)
                {
                    CurrentSongIndex = 0;
                }
                else
                {
                    CurrentSongIndex = selectedIndex;
                }
                play(path, selectedIndex);
            }
        }

        public void Stop()
        {
            if (PlayerState != Mp3PlayerState.Stopped)
            {
                PlayerState = Mp3PlayerState.Stopped;
                clearPlayer();
            }
        }

        public void Pause()
        {
            if (PlayerState == Mp3PlayerState.Playing)
            {
                PlayerState = Mp3PlayerState.Paused;
                _waveOutDevice.Pause();
            }
        }

        private void clearPlayer()
        {
            CurrentSongIndex = -1;
            _waveOutDevice.Stop();
            _mp3Reader.Dispose();
            _mp3Reader = null;
            _waveOutDevice.Dispose();
            _waveOutDevice = null;
        }

        private void play(string path, int index)
        {
            PlayerState = Mp3PlayerState.Playing;
            _waveOutDevice = new WaveOut();
            _mp3Reader = new MediaFoundationReader(path);
            _waveOutDevice.Init(_mp3Reader);
            _waveOutDevice.Play();
        }
    }
}
