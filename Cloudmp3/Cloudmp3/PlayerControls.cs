using NAudio.Wave;
using NAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using System.Media;
using WMPLib;

namespace MP3SelfDemo
{
    public class PlayerControls
    {

        IWavePlayer player = new WaveOut();
        List<Mp3FileReader> read = new List<Mp3FileReader>{
            new Mp3FileReader(@"C:\Users\smarks\Desktop\music\Chance The Rapper - 10Day Official Final\03 Nostalgia.mp3"),
            new Mp3FileReader(@"C:\Users\smarks\Desktop\music\Chance The Rapper - 10Day Official Final\11 Fuck You Tahm Bout.mp3")
        };

        int track = 0;

        public void Play()
        {
            player.Init(read[track]);
            player.Play();
        }

        public void Stop()
        {
            player.Stop();
            player.Dispose();


        }

        public void Pause()
        {
            player.Init(read[track]);
            player.Pause();
        }

        public void Forward()
        {
            if (player != null)
            {
                this.Stop();
            }
            player = new WaveOut();
            track += 1;
            if (track >= read.Count)
            {
                track = 0;
            }
            player.Init(read[track]);
            player.Play();

        }

        public void Back()
        {
            if (player != null)
            {
                this.Stop();
            }
            player = new WaveOut();
            track -= 1;
            if (track <= -1)
            {
                track = 0;
            }
            player.Init(read[track]);
            player.Play();
        }
    }
}