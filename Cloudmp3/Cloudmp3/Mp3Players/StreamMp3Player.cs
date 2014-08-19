using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudmp3.Mp3Players
{
    public class StreamMp3Player : IMp3Player
    {
        public enum Mp3PlayerState
        {
            Playing,
            Stopped,
            Paused,
        }

        private Thread _downLoadThread;
        private Thread _playThread;
        private MemoryStream _playerStream;
        private WebResponse _webResponse;
        private WaveOut _waveOut;
        private Mp3PlayerState _playerState;

        public StreamMp3Player ()
	    {
            _playerState = Mp3PlayerState.Stopped;
        }

        public void Play(string path = null)
        {
            if (_playerState == Mp3PlayerState.Paused && path == null)
            {
                _playerState = Mp3PlayerState.Playing;
                _waveOut.Play();
            }
            else if(path != null)
            {
                if (_playerState != Mp3PlayerState.Stopped)
                {
                    ClearPlayer();
                }
                _playerState = Mp3PlayerState.Playing;
                PlaySong(path);
            }
        }

        public void Stop()
        {
            if (_playerState != Mp3PlayerState.Stopped)
            {
                _playerState = Mp3PlayerState.Stopped;
                ClearPlayer();
            }
        }

        public void Pause()
        {
            if (_playerState == Mp3PlayerState.Playing)
            {
                _playerState = Mp3PlayerState.Paused;
                _waveOut.Pause();
            }
        }

        private void PlaySong(string path)
        {
            _playerStream = new MemoryStream();
            _waveOut = new WaveOut();

            _playThread = new Thread(delegate(object o)
            {
                _downLoadThread = new Thread(delegate(object p)
                {
                    _webResponse = WebRequest.Create(path).GetResponse();
                    using (var stream = _webResponse.GetResponseStream())
                    {
                        try
                        {
                            byte[] buffer = new byte[65536];
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                var pos = _playerStream.Position;
                                _playerStream.Position = _playerStream.Length;
                                _playerStream.Write(buffer, 0, read);
                                _playerStream.Position = pos;
                            }
                        }
                        catch (Exception e)
                        {
                            
                        }
                    }

                });
                _downLoadThread.Start();

                try
                {
                    while (_playerStream.Length < 65536 * 10)
                    {
                        Thread.Sleep(1000);
                    }

                    _playerStream.Position = 0;
                    using (WaveStream blockAlignedStream = new BlockAlignReductionStream(
                        WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(_playerStream))))
                    {
                        using (_waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                        {
                            _waveOut.Init(blockAlignedStream);
                            _waveOut.Play();
                            while (_waveOut != null)
                            {
                                Thread.Sleep(100);
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }   
            });
            _playThread.Start();
        }

        private void ClearPlayer()
        {
            if (_waveOut != null)
            {
                _waveOut.Stop();
                _waveOut = null;
            }
            if (_webResponse != null)
            {
                _webResponse.Close();
                _webResponse = null;
            }
            if (_playerStream != null)
            {
                _playerStream.Dispose();
                _playerStream = null;
            }

        }
    }
}
