using Cloudmp3.AzureBlobClasses;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cloudmp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    //Test Commit
    public partial class MainWindow : Window
    {
        public enum PlayerState
        {
            Playing,
            Stopped,
            Paused
        }

        Thread streamingThread;

        IWavePlayer waveOutDevice;
        Mp3FileReader mp3FileReader;
        PlayerState mp3PlayerState;
        int currentlyPlayingSongIndex;
        //string selectedSongPath;

        private Stream ms;
        private bool streaming = false;

        ObservableCollection<string> songList;
        ObservableCollection<string> cloudSongList;

        public MainWindow()
        {
            InitializeComponent();
            mp3PlayerState = PlayerState.Stopped;
            currentlyPlayingSongIndex = -1;

            songList = new ObservableCollection<string>(Directory.GetFiles("C:/Users/Steven Ulibarri/Music/Ben Prunty Music - FTL","*.mp3"));
            //songList = new ObservableCollection<string>(new BlobClass().getCloudSongs());
            SongListBox.ItemsSource = songList;

            cloudSongList = new ObservableCollection<string>(new BlobClass().getCloudSongs());
            CloudSongsBox.ItemsSource = cloudSongList;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (mp3PlayerState == PlayerState.Paused && currentlyPlayingSongIndex == SongListBox.SelectedIndex)
            {
                mp3PlayerState = PlayerState.Playing;
                waveOutDevice.Play();
            }
            else
            {
                if (mp3PlayerState != PlayerState.Stopped)
                {
                    clearPlayer();
                }
                if (SongListBox.SelectedIndex == -1 && mp3PlayerState == PlayerState.Stopped)
                {
                    currentlyPlayingSongIndex = 0;
                }
                else
                {
                    currentlyPlayingSongIndex = SongListBox.SelectedIndex;
                }
                play();
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (mp3PlayerState != PlayerState.Stopped)
            {
                mp3PlayerState = PlayerState.Stopped;
                //currentlyPlayingSongIndex = null;
                clearPlayer();
            }
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (mp3PlayerState == PlayerState.Playing)
            {
                mp3PlayerState = PlayerState.Paused;
                waveOutDevice.Pause();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (mp3PlayerState != PlayerState.Stopped)
            {
                clearPlayer();
            }
            if (currentlyPlayingSongIndex == -1 || currentlyPlayingSongIndex == SongListBox.Items.Count -1)
            {
                currentlyPlayingSongIndex = 0;
            }
            else
            {
                currentlyPlayingSongIndex++;
            }
            play();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (mp3PlayerState != PlayerState.Stopped)
            {
                clearPlayer();
            }
            if (currentlyPlayingSongIndex == -1 || currentlyPlayingSongIndex == 0)
            {
                currentlyPlayingSongIndex = SongListBox.Items.Count - 1;
            }
            else
            {
                currentlyPlayingSongIndex--;
            }
            play();
        }
        
        private void UpLoad_Click(object sender, RoutedEventArgs e)
        {
            UploadFile();
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            if (CloudSongsBox.SelectedIndex != -1)
            {
                DownloadFile();
            }
        }

        private void StreamButton_Click(object sender, RoutedEventArgs e)
        {

            if (CloudSongsBox.SelectedIndex != -1)
            {
                if (streamingThread != null)
                {
                    streaming = false;
                    streamingThread.Abort();
                    ms.Dispose();
                }

                string streamPath = (string)CloudSongsBox.SelectedItem;

                streamingThread = new Thread(delegate(object o)
                {
                    streaming = true;
                    ms = new MemoryStream();
                    PlayMp3FromUrl(streamPath);
                });
                streamingThread.Start();
            }
        }

        private void StreamStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (streamingThread != null)
            {
                streaming = false;
                ms.Dispose();
                streamingThread.Abort();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            cloudSongList = new ObservableCollection<string>(new BlobClass().getCloudSongs());
            CloudSongsBox.ItemsSource = cloudSongList;
        }

        private void clearPlayer()
        {
            waveOutDevice.Stop();
            mp3FileReader.Dispose();
            mp3FileReader = null;
            waveOutDevice.Dispose();
            waveOutDevice = null;
        }

        private void play()
        {
            mp3PlayerState = PlayerState.Playing;
            //selectedSongPath = (string)SongListBox.SelectedItem;
            //new Thread(delegate(object o)
            //{
            //    PlayMp3FromUrl(selectedSongPath);
            //}).Start();
            waveOutDevice = new WaveOut();
            SongListBox.SelectedIndex = currentlyPlayingSongIndex;
            mp3FileReader = new Mp3FileReader((string)SongListBox.SelectedItem);
            waveOutDevice.Init(mp3FileReader);
            waveOutDevice.Play();
        }

        private void UploadFile() //Added using Microsoft.Win32
        {
            OpenFileDialog ChooseFile = new OpenFileDialog();
            ChooseFile.Filter = "Music Files (.mp3)|*.mp3|All Files (*.*)|*.*";
            ChooseFile.FilterIndex = 1;
            ChooseFile.ShowDialog();
            String File = ChooseFile.FileName;

            new Thread(delegate(object o)
            {
                new BlobClass().uploadSong(File);
            }).Start();
        }

        private void DownloadFile()
        {
            string path = (string)CloudSongsBox.SelectedItem;
            new Thread(delegate(object o)
            {
                new BlobClass().downloadSong(path);
            }).Start();
        }

        public void PlayMp3FromUrl(string url)
        {
            new Thread(delegate(object o)
            {
                
                var response = WebRequest.Create(url).GetResponse();
                
                using (var stream = response.GetResponseStream())
                {
                    byte[] buffer = new byte[65536]; // 64KB chunks
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0 && streaming == true)
                    {
                        var pos = ms.Position;
                        ms.Position = ms.Length;
                        ms.Write(buffer, 0, read);
                        ms.Position = pos;
                    }
                }
            }).Start();

            // Pre-buffering some data to allow NAudio to start playing
            while (ms.Length < 65536 * 10)
                Thread.Sleep(1000);

            ms.Position = 0;
            using (WaveStream blockAlignedStream = new BlockAlignReductionStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(ms))))
            {
                using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                {
                    waveOut.Init(blockAlignedStream);
                    waveOut.Play();
                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }

        //public void PlayMp3FromUrl(string url)
        //{
        //    using (ms = new MemoryStream())
        //    {
        //        using (Stream stream = WebRequest.Create(url)
        //            .GetResponse().GetResponseStream())
        //        {
        //            byte[] buffer = new byte[32768];
        //            int read;
        //            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                ms.Write(buffer, 0, read);
        //            }
        //        }

        //        ms.Position = 0;
        //        using (WaveStream blockAlignedStream =
        //            new BlockAlignReductionStream(
        //                WaveFormatConversionStream.CreatePcmStream(
        //                    new Mp3FileReader(ms))))
        //        {
        //            using (WaveOut waveOutDevice = new WaveOut(WaveCallbackInfo.FunctionCallback()))
        //            {
        //                waveoutdevice.init(blockalignedstream);
        //                waveoutdevice.play();
        //                while (waveoutdevice.playbackstate == playbackstate.playing)
        //                {
        //                    system.threading.thread.sleep(100);
        //                }
        //            }
        //        }
        //    }
        //}
      
    }
}
