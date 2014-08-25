using Cloudmp3.AzureBlobClasses;
using Cloudmp3.DataAccessLayer;
using Cloudmp3.Mp3Players;
using Cloudmp3.Windows;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Cloudmp3
{

    public partial class MainWindow : Window
    {
        private IMp3Player _localPlayer;

        private ObservableCollection<Song> _songList;
        private AzureAccess _blobAccess;
        private SqlAccess _sqlAccess;

		private BitmapImage _playImage = new BitmapImage(new Uri("Images/Play.png", UriKind.Relative));
		private BitmapImage _pauseImage = new BitmapImage(new Uri("Images/Pause.png", UriKind.Relative));

		private int CurrentSongIndex { get; set; }

        private int _userId = 1;
        private bool _loggedIn;
        private bool _isPlaying;

        public bool LoggedIn
        {
            get { return _loggedIn; }
            set
            {
                _loggedIn = value;
                LoginChange();
            }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                PlayButtonSwap();
            }
        }
        private string localMp3Directory = "C:/Users/Public/Music/CloudMp3";

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Setup();
                
                LoggedIn = true;
                IsPlaying = false;
                _blobAccess = new AzureAccess();
                _localPlayer = new StreamMp3Player();
                _sqlAccess = new SqlAccess();
                PlayerGrid.DataContext = _localPlayer;
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

        private void PlayButtonSwap()
        {
            ((Image)(PlayButton.Content)).Source = (IsPlaying) ? _pauseImage : _playImage;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPlaying)
            {
                if (SongDataGrid.SelectedIndex == -1)
                {
                    IsPlaying = true;
                    SongDataGrid.SelectedIndex = ++SongDataGrid.SelectedIndex;
                    CurrentSongIndex = SongDataGrid.SelectedIndex;
                    Song s = (Song)SongDataGrid.SelectedItem;
                    _localPlayer.Play(s.S_Path + _blobAccess.GetSaS());
                }
                else
                {
                    IsPlaying = true;
                    CurrentSongIndex = SongDataGrid.SelectedIndex;
                    Song s = (Song)SongDataGrid.SelectedItem;
                    _localPlayer.Play(s.S_Path + _blobAccess.GetSaS());
                }
            }
            else
            {
                IsPlaying = false;
                _localPlayer.Pause();
            } 
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
            _localPlayer.Stop();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = true;
            _localPlayer.Pause();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = true;
            _localPlayer.Stop();
            SongDataGrid.SelectedIndex = (CurrentSongIndex == SongDataGrid.Items.Count - 1) ? 0 : ++SongDataGrid.SelectedIndex;
            CurrentSongIndex = SongDataGrid.SelectedIndex;
            Song s = (Song)SongDataGrid.SelectedItem;
            _localPlayer.Play(s.S_Path + _blobAccess.GetSaS());
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = true;
            _localPlayer.Stop();
            SongDataGrid.SelectedIndex = (CurrentSongIndex <= 0) ?SongDataGrid.Items.Count - 1 : --SongDataGrid.SelectedIndex;
            CurrentSongIndex = SongDataGrid.SelectedIndex;
            Song s = (Song)SongDataGrid.SelectedItem;
            _localPlayer.Play(s.S_Path + _blobAccess.GetSaS());
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

        private void Log_Click(object sender, RoutedEventArgs e)
        {
            if (!_loggedIn)
            {
                Login log = new Login();
                log.ShowDialog();

                if (log.UserName != null)
                {
                    if (_sqlAccess.ValidateUserName(log.UserName, log.Password))
                    {
                        _userId = _sqlAccess.GetUserID(log.UserName);
                        LoggedIn = true;
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Username or Password.");
                    }
                } 
            }
            else
            {
                _localPlayer.Stop();
                SongDataGrid.ItemsSource = null;
                LoggedIn = false;
            }
        }

		private void Song_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			IsPlaying = true;
			CurrentSongIndex = SongDataGrid.SelectedIndex;
            Song s = (Song)SongDataGrid.SelectedItem;
            _localPlayer.Play(s.S_Path + _blobAccess.GetSaS());
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
					_blobAccess.UploadSong(file, _userId);
					Dispatcher.BeginInvoke(new Action(delegate() 
					{
						_songList = _sqlAccess.GetSongsForUser(_userId);
						SongDataGrid.ItemsSource = _songList;
					}));
				});
			}
		}

		private void DownloadFile()
		{
			if (SongDataGrid.SelectedIndex != -1)
			{
                Song s = (Song)SongDataGrid.SelectedItem;
				string path = s.S_Path;
                Task.Factory.StartNew(() =>
                {
                    _blobAccess.DownloadSong(Path.GetFileName(path));
                });
			}
		}

        private void LoginChange()
        {
            if (!_loggedIn)
            {
                //UploadButton.Visibility = System.Windows.Visibility.Hidden;
                //DownLoadButton.Visibility = System.Windows.Visibility.Hidden;
                //SongDataGrid.Visibility = System.Windows.Visibility.Hidden;
                //PlayerGrid.Visibility = System.Windows.Visibility.Hidden;
                //ButtonPanel.Visibility = System.Windows.Visibility.Hidden;
                //LogButton.Header = "Login";
                //LoginStatusLabel.Content = "You are offline! log in to see your music!";
                //IsPlaying = false;
            }
            else
            {
                //UploadButton.Visibility = System.Windows.Visibility.Visible;
                //DownLoadButton.Visibility = System.Windows.Visibility.Visible;
                //SongDataGrid.Visibility = System.Windows.Visibility.Visible;
                //PlayerGrid.Visibility = System.Windows.Visibility.Visible;
                //ButtonPanel.Visibility = System.Windows.Visibility.Visible;
                //LogButton.Header = "Logout";
                //LoginStatusLabel.Content = "";
                //CurrentSongIndex = -1;
                Dispatcher.BeginInvoke(new Action(delegate()
                {
                    _songList = _sqlAccess.GetSongsForUser(_userId);
                    SongDataGrid.ItemsSource = _songList;
                }));
            }
        }

	}
}
