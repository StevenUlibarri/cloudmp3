using Cloudmp3.AzureBlobClasses;
using Cloudmp3.DataAccessLayer;
using Cloudmp3.Mp3Players;
using Cloudmp3.Windows;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public string notificatioN { get; set; }

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
                CurrentSongIndex = -1;
                _songList = _sqlAccess.GetSongsForUser(_userId);
                SongDataGrid.ItemsSource = _songList;
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

        //private void UpLoad_Click(object sender, RoutedEventArgs e)
        //{
        //    UploadFile();
        //    NotifyUsrup();
        //}

        //private void Download_Click(object sender, RoutedEventArgs e)
        //{
        //    DownloadFile();
        //    NotifyUsrdown();
        //}
        private void NotifyUsrup()
        {
            if (_blobAccess.isCompleted.Equals(true))
            {
                notificatioN = "Song is being uploaded";
            }
        }
        private void NotifyUsrdown()
        {
            if (_blobAccess.isCompleted.Equals(true))
            {
                notificatioN = "Song is being downloaded";
            }
        }

		private void StreamButton_Click(object sender, RoutedEventArgs e)
		{
			
		}

		private void StreamStopButton_Click(object sender, RoutedEventArgs e)
		{

		}
        //private void Log_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!_loggedIn)
        //    {
        //        Login log = new Login();
        //        log.ShowDialog();

        //        if (log.UserName != null)
        //        {
        //            if (_sqlAccess.ValidateUserName(log.UserName, log.Password))
        //            {
        //                _userId = _sqlAccess.GetUserID(log.UserName);
        //                LoggedIn = true;
        //            }
        //            else
        //            {
        //                MessageBox.Show("Incorrect Username or Password.");
        //            }
        //        } 
        //    }
        //    else
        //    {
        //        _localPlayer.Stop();
        //        SongDataGrid.ItemsSource = null;
        //        LoggedIn = false;
        //    }
        //}

		private void Song_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			IsPlaying = true;
			CurrentSongIndex = SongDataGrid.SelectedIndex;
            Song s = (Song)SongDataGrid.SelectedItem;
            _localPlayer.Play(s.S_Path + _blobAccess.GetSaS());
		}

        //private void UploadFile() //Added using Microsoft.Win32
        //{
        //    OpenFileDialog chooseFile = new OpenFileDialog();
        //    chooseFile.Filter = "Music Files (.mp3)|*.mp3|All Files (*.*)|*.*";
        //    chooseFile.FilterIndex = 1;
        //    chooseFile.ShowDialog();
        //    string file = chooseFile.FileName;

        //    if (!string.IsNullOrEmpty(file))
        //    {
        //        Task.Factory.StartNew(() =>
        //        {
        //            _blobAccess.UploadSong(file, _userId);
        //            Dispatcher.BeginInvoke(new Action(delegate() 
        //            {
        //                _songList = _sqlAccess.GetSongsForUser(_userId);
        //                SongDataGrid.ItemsSource = _songList;
        //            }));
        //        });
        //    }
			//notifLabel.Content = "Upload Complete";
			//notifarea.Visibility = Visibility.Hidden;
		//}

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
                //Dispatcher.BeginInvoke(new Action(delegate()
                //{
                //    _songList = _sqlAccess.GetSongsForUser(_userId);
                //    SongDataGrid.ItemsSource = _songList;
                //}));
            }
        }

        private void LoginCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!_loggedIn)
            {
                e.CanExecute = true;
            }
            e.Handled = true;
        }

        private void LoginExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Login log = new Login();
            log.ShowDialog();

            if (log.UserName != null)
            {
                if (_sqlAccess.ValidateUserName(log.UserName, log.Password))
                {
                    _userId = _sqlAccess.GetUserID(log.UserName);
                    LoggedIn = true;
                    NotificationsLabel.Content = "You are login as " + log.UserName;
                }
                else
                {
                    MessageBox.Show("Incorrect Username or Password.");
                }
            }
            e.Handled = true;
        }

        private void LogoutCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_loggedIn)
            {
                e.CanExecute = true;
            }
            e.Handled = true;
        }

        private void LogoutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _localPlayer.Stop();
            SongDataGrid.ItemsSource = null;
            LoggedIn = false;
            e.Handled = true;
            NotificationsLabel.Content = "You have logged out. Good Bye";
        }

        private void UploadSongCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_loggedIn)
            {
                e.CanExecute = true;

            }
            e.Handled = true;
        }

        private void UploadSongExecuted(object sender, ExecutedRoutedEventArgs e)
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
                        NotificationsLabel.Content = "Upload Complete";
                    }));
                });
            }
            e.Handled = true;
        }

        private void DownloadSongCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void DownloadSongExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //download logic here
            e.Handled = true;
        }

        //Remove Song from Playlist
        private void RemoveSongFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (SongDataGrid.SelectedItem != null && PlaylistBox.SelectedItem != null)
            {
                Song SelectedSong = (Song)SongDataGrid.SelectedItem;
                Playlist SelectedPlaylist = (Playlist)PlaylistBox.SelectedItem;
                _sqlAccess.RemoveSongFromPlaylist(SelectedSong.S_Id, SelectedPlaylist.P_Id);
            }
        }

        //Rename Playlist Methods
        //Open Rename Playlist Popup
        private void RenamePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistBox.SelectedItem != null)
            {
                RenamePlaylistPopup.IsOpen = true;
            }
        }

        //Rename the Playlist and Close
        private void RenameList_Click(object sender, RoutedEventArgs e)
        {
            string NewPlaylistName = PlaylistRenameBox.Text;

            if (!string.IsNullOrWhiteSpace(NewPlaylistName))
            {
                Playlist SelectedPlaylist = (Playlist)PlaylistBox.SelectedItem;
                SelectedPlaylist.P_Name = NewPlaylistName;
                AddPlaylistPopup.IsOpen = false;
            }
            PlaylistRenameBox.Text = "";
        }

        //Cancel Rename and Close
        private void CloseRenamePopup_Click(object sender, RoutedEventArgs e)
        {
            RenamePlaylistPopup.IsOpen = false;
            PlaylistRenameBox.Text = "";
        }
        //End Rename Playlist Methods

        //Remove Playlist
        private void RemovePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistBox.SelectedItem != null)
            {
                Playlist SelectedPlaylist = (Playlist)PlaylistBox.SelectedItem;
                _sqlAccess.RemovePlaylist(SelectedPlaylist.P_Id);
            }
        }

        //Add new Playlist Methods
        //Open Popup to create new Playlist
        private void AddPlaylistPopup_Click(object sender, RoutedEventArgs e)
        {
            AddPlaylistPopup.IsOpen = true;
        }

        //Add the new Playlist and close popup
        private void AddList_Click(object sender, RoutedEventArgs e)
        {
            string NewPlaylistName = PlaylistNameBox.Text;

            if (!string.IsNullOrWhiteSpace(NewPlaylistName))
            {
                Playlist NewPlaylist = new Playlist();
                NewPlaylist.P_Name = NewPlaylistName;
                _sqlAccess.AddPlaylist(NewPlaylist);
                AddPlaylistPopup.IsOpen = false;
            }
            PlaylistNameBox.Text = "";
        }

        //Close the popup and cancel adding the playlist
        private void ClosePlaylistPopup_Click(object sender, RoutedEventArgs e)
        {
            AddPlaylistPopup.IsOpen = false;
            PlaylistNameBox.Text = "";
        }
        //End Add new Playlist Methods

        //Add Song to Playlist Methods
        //Open Add Song to Playlist Popup
        private void AddSongToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            AddSongToPlaylistPopup.IsOpen = true;
        }

        //Add the song to the selected playlist and close
        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (SongDataGrid.SelectedItem != null && ChoosePlaylist.SelectedIndex != -1)
            {
                Song SelectedSong = (Song)SongDataGrid.SelectedItem;
                Playlist SelectedPlaylist = (Playlist)ChoosePlaylist.SelectedItem;
                _sqlAccess.AddSongToPlaylist(SelectedSong.S_Id, SelectedPlaylist.P_Id);
            }
        }

        //Cancel adding the song to another playlist and close
        private void CloseAddSongToPlaylistPopup_Click(object sender, RoutedEventArgs e)
        {
            AddSongToPlaylistPopup.IsOpen = false;
            ChoosePlaylist.SelectedIndex = -1;
        }
        //End Add Song to Playlist methods
	}
}
