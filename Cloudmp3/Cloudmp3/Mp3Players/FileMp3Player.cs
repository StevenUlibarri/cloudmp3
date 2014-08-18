using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudmp3.Mp3Players
{
    public class FileMp3Player : IMp3Player
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

        public FileMp3Player()
        {
            PlayerState = Mp3PlayerState.Stopped;
        }

        public void Play(string path)
        {
            if (PlayerState == Mp3PlayerState.Paused && path == null)
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
                play(path);
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
            _waveOutDevice.Stop();
            _mp3Reader.Dispose();
            _mp3Reader = null;
            _waveOutDevice.Dispose();
            _waveOutDevice = null;
        }

        private void play(string path)
        {
            PlayerState = Mp3PlayerState.Playing;
            _waveOutDevice = new WaveOut();
            _mp3Reader = new MediaFoundationReader(path);
            _waveOutDevice.Init(_mp3Reader);
            _waveOutDevice.Play();
        }
    }
}
