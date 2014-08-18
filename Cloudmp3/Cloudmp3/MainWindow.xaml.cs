using Cloudmp3.AzureBlobClasses;
using Cloudmp3.Mp3Players;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Cloudmp3
{
    public partial class MainWindow : Window
    {
        ObservableCollection<string> songList;
        ObservableCollection<string> cloudSongList;
        IMp3Player localPlayer;
        AzureAccess blobAccess;

        private int CurrentSongIndex { get; set; }
        private string localMp3Directory = "C:/Users/Public/Music/CloudMp3";

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Setup();
                blobAccess = new AzureAccess();
                //localPlayer = new FileMp3Player();
                localPlayer = new StreamMp3Player();
                songList = new ObservableCollection<string>(Directory.GetFiles("C:/Users/Public/Music/CloudMp3", "*.mp3"));
                LocalSongListBox.ItemsSource = songList;
                cloudSongList = blobAccess.GetCloudSongs();
                CloudSongsBox.ItemsSource = cloudSongList;
                CurrentSongIndex = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine( e.InnerException.Message);
            } 
        }

        private void Setup()
        {
            if (!Directory.Exists(localMp3Directory))
            {
                Directory.CreateDirectory(localMp3Directory);
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (CloudSongsBox.SelectedIndex == -1)
            {
                CloudSongsBox.SelectedIndex = ++CloudSongsBox.SelectedIndex;
                CurrentSongIndex = CloudSongsBox.SelectedIndex;
                localPlayer.Play((string)CloudSongsBox.SelectedItem + blobAccess.GetSaS());
            }
            else if (CurrentSongIndex == CloudSongsBox.SelectedIndex)
            {
                localPlayer.Play(null);
            }
            else
            {
                CurrentSongIndex = CloudSongsBox.SelectedIndex;
                localPlayer.Play((string)CloudSongsBox.SelectedItem + blobAccess.GetSaS());
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            localPlayer.Stop();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            localPlayer.Pause();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            CloudSongsBox.SelectedIndex = (CurrentSongIndex == CloudSongsBox.Items.Count - 1) ? 0 : ++CloudSongsBox.SelectedIndex;
            CurrentSongIndex = CloudSongsBox.SelectedIndex;
            localPlayer.Play((string)CloudSongsBox.SelectedItem + blobAccess.GetSaS());
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
           CloudSongsBox.SelectedIndex = (CurrentSongIndex <= 0) ?CloudSongsBox.Items.Count - 1 : --CloudSongsBox.SelectedIndex;
            CurrentSongIndex = CloudSongsBox.SelectedIndex;
            localPlayer.Play((string)CloudSongsBox.SelectedItem + blobAccess.GetSaS());
        }

        private void UpLoad_Click(object sender, RoutedEventArgs e)
        {
            UploadFile();
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            DownloadFile();
        }

        private void StreamButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void StreamStopButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Song_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CurrentSongIndex = CloudSongsBox.SelectedIndex;
            localPlayer.Play((string)CloudSongsBox.SelectedItem);
        }

        private void UploadFile() //Added using Microsoft.Win32
        {
            OpenFileDialog chooseFile = new OpenFileDialog();
            chooseFile.Filter = "Music Files (.mp3)|*.mp3|All Files (*.*)|*.*";
            chooseFile.FilterIndex = 1;
            chooseFile.ShowDialog();
            string file = chooseFile.FileName;

            if (!string.IsNullOrEmpty(file))
            {
                Task.Factory.StartNew(() =>
                {
                    blobAccess.UploadSong(file);
                    Dispatcher.BeginInvoke(new Action(delegate() 
                    {
                        cloudSongList = blobAccess.GetCloudSongs();
                        CloudSongsBox.ItemsSource = cloudSongList;
                    }));
                });
            }
        }

        private void DownloadFile()
        {
            if (CloudSongsBox.SelectedIndex != -1)
            {
                string path = (string)CloudSongsBox.SelectedItem;
                Task.Factory.StartNew(() =>
                {
                    blobAccess.DownloadSong(Path.GetFileName(path));
                    Dispatcher.BeginInvoke(new Action(delegate()
                    {
                        songList = new ObservableCollection<string>(Directory.GetFiles("C:/Users/Public/Music/CloudMp3", "*.mp3"));
                        LocalSongListBox.ItemsSource = songList;
                    }));

                });
            }
        }
    }
}
