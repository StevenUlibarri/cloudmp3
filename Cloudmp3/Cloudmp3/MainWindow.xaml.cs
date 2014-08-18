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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    //Test Commit
    public partial class MainWindow : Window
    {
        ObservableCollection<string> songList;
        ObservableCollection<string> cloudSongList;
        FileMp3Player localPlayer;
        AzureAccess blobAccess;

        private string localMp3Directory = "C:/Users/Public/Music/CloudMp3";

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Setup();
                blobAccess = new AzureAccess();
                localPlayer = new FileMp3Player();
                songList = new ObservableCollection<string>(Directory.GetFiles("C:/Users/Public/Music/CloudMp3", "*.mp3"));
                LocalSongListBox.ItemsSource = songList;
                cloudSongList = blobAccess.GetCloudSongs();
                CloudSongsBox.ItemsSource = cloudSongList;
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
            if (LocalSongListBox.SelectedIndex == -1)
            {
                LocalSongListBox.SelectedIndex = ++LocalSongListBox.SelectedIndex;
            }
            localPlayer.Play((string)LocalSongListBox.SelectedItem, LocalSongListBox.SelectedIndex);
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
            LocalSongListBox.SelectedIndex = (localPlayer.CurrentSongIndex == LocalSongListBox.Items.Count - 1) ? 0 : ++LocalSongListBox.SelectedIndex; 
            localPlayer.Play((string)LocalSongListBox.SelectedItem, LocalSongListBox.SelectedIndex);
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            LocalSongListBox.SelectedIndex = (localPlayer.CurrentSongIndex <= 0) ? LocalSongListBox.Items.Count - 1 : --LocalSongListBox.SelectedIndex;
            localPlayer.Play((string)LocalSongListBox.SelectedItem, LocalSongListBox.SelectedIndex);
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
            localPlayer.Play((string)LocalSongListBox.SelectedItem, LocalSongListBox.SelectedIndex);
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
