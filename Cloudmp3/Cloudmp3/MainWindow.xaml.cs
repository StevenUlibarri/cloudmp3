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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cloudmp3
{

    public partial class MainWindow : Window
    {
        private IMp3Player _localPlayer;
        private ObservableCollection<Song> _songList;
        private ObservableCollection<Playlist> _playlistList;
        private AzureAccess _blobAccess;
        private SqlAccess _sqlAccess;

        private BitmapImage _playImage = new BitmapImage(new Uri("Images/Play.png", UriKind.Relative));
        private BitmapImage _pauseImage = new BitmapImage(new Uri("Images/Pause.png", UriKind.Relative));

        private int CurrentSongIndex { get; set; }
        private static int _userId;
        public static int UserId
        {
            get { return _userId; }
        }

        public string notificatioN { get; set; }

        private bool _loggedIn;
        public bool LoggedIn
        {
            get { return _loggedIn; }
            set
            {
                _loggedIn = value;
                LoginChange();
            }
        }

        private bool _isPlaying;
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
                LoggedIn = false;
                IsPlaying = false;
                _blobAccess = new AzureAccess();
                _localPlayer = new StreamMp3Player();
                _sqlAccess = new SqlAccess();
                PlayerGrid.DataContext = _localPlayer;
                CurrentSongIndex = -1;
                this.Loaded += new RoutedEventHandler(LoginPromt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException.Message);
            }
        }

        private void LoginPromt(object sender, RoutedEventArgs e)
        {
            LoginExecuted(null, null);
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
            ((Image) (PlayButton.Content)).Source = (IsPlaying) ? _pauseImage : _playImage;
        }

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

        private void Song_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsPlaying = true;
            _localPlayer.Stop();
            CurrentSongIndex = SongDataGrid.SelectedIndex;
            Song s = (Song) SongDataGrid.SelectedItem;
            _localPlayer.Play(s.S_Path + _blobAccess.GetSaS(), s.S_Length);
        }

        private void LoginChange()
        {
            if (!_loggedIn)
            {
                IsPlaying = false;
            }
            else
            {
                CurrentSongIndex = -1;
                Dispatcher.BeginInvoke(new Action(delegate()
                {
                    _songList = _sqlAccess.GetSongsForUser(_userId);
                    SongDataGrid.ItemsSource = _songList;
                    _playlistList = _sqlAccess.GetPlaylistsForUser(_userId);
                    PlaylistBox.ItemsSource = _playlistList;
                }));
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
            log.Top = this.Top + 50;
            log.Left = this.Left + 50;
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
            if (e != null)
            {
                e.Handled = true;
            }
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
            _userId = 0;
            SongDataGrid.ItemsSource = null;
            LoggedIn = false;
            e.Handled = true;
            NotificationsLabel.Content = "You have logged out. Good Bye";
            // doesnt work need to fix!!!!!!!!!!!!!!!!!!
            ProgressBar p = new ProgressBar();
            p.Foreground = new SolidColorBrush(Colors.Maroon);
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
            if (_loggedIn && SongDataGrid.SelectedIndex != -1)
            {
                e.CanExecute = true;
            }
            e.Handled = true;
        }

        private void DownloadSongExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Song s = (Song)SongDataGrid.SelectedItem;
            string path = s.S_Path;
            Task.Factory.StartNew(() =>
            {
                _blobAccess.DownloadSong(Path.GetFileName(path));
            });
            e.Handled = true;
        }

        private void PlayCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_loggedIn)
            {
                e.CanExecute = true;
            }
            e.Handled = true;
        }

        private void PlayExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!IsPlaying)
            {
                if (SongDataGrid.SelectedIndex == -1)
                {
                    IsPlaying = true;
                    SongDataGrid.SelectedIndex = ++SongDataGrid.SelectedIndex;
                    CurrentSongIndex = SongDataGrid.SelectedIndex;
                    Song s = (Song)SongDataGrid.SelectedItem;
                    _localPlayer.Play(s.S_Path + _blobAccess.GetSaS(), s.S_Length);
                }
                else
                {
                    IsPlaying = true;
                    if (CurrentSongIndex != SongDataGrid.SelectedIndex)
                    {
                        _localPlayer.Stop();
                    }
                    CurrentSongIndex = SongDataGrid.SelectedIndex;
                    Song s = (Song)SongDataGrid.SelectedItem;
                    _localPlayer.Play(s.S_Path + _blobAccess.GetSaS(), s.S_Length);
                }
            }
            else
            {
                IsPlaying = false;
                _localPlayer.Pause();
            } 
            e.Handled = true;
        }

        private void StopCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_loggedIn)
            {
                e.CanExecute = true;
            }
            e.Handled = true;
        }

        private void StopExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            IsPlaying = false;
            _localPlayer.Stop();
            e.Handled = true;
        }

        private void NextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_loggedIn)
            {
                e.CanExecute = true;
            }
            e.Handled = true;
        }

        private void NextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            IsPlaying = true;
            _localPlayer.Stop();
            SongDataGrid.SelectedIndex = (CurrentSongIndex == SongDataGrid.Items.Count - 1) ? 0 : ++SongDataGrid.SelectedIndex;
            CurrentSongIndex = SongDataGrid.SelectedIndex;
            Song s = (Song)SongDataGrid.SelectedItem;
            _localPlayer.Play(s.S_Path + _blobAccess.GetSaS(), s.S_Length);
            e.Handled = true;
        }

        private void PrevCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_loggedIn)
            {
                e.CanExecute = true;
            }
            e.Handled = true;
        }

        private void PrevExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            IsPlaying = true;
            _localPlayer.Stop();
            SongDataGrid.SelectedIndex = (CurrentSongIndex <= 0) ? SongDataGrid.Items.Count - 1 : --SongDataGrid.SelectedIndex;
            CurrentSongIndex = SongDataGrid.SelectedIndex;
            Song s = (Song)SongDataGrid.SelectedItem;
            _localPlayer.Play(s.S_Path + _blobAccess.GetSaS(), s.S_Length);
            e.Handled = true;
        }

        ////Remove Song from Playlist
        //private void RemoveSongFromPlaylist_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SongDataGrid.SelectedItem != null && PlaylistBox.SelectedItem != null)
        //    {
        //        Song SelectedSong = (Song)SongDataGrid.SelectedItem;
        //        Playlist SelectedPlaylist = (Playlist)PlaylistBox.SelectedItem;
        //        _sqlAccess.RemoveSongFromPlaylist(SelectedSong.S_Id, SelectedPlaylist.P_Id);
        //        Dispatcher.BeginInvoke(new Action(delegate()
        //        {
        //            _songList = _sqlAccess.GetSongsForUser(_userId);
        //            SongDataGrid.ItemsSource = _songList;
        //            _playlistList = _sqlAccess.GetPlaylistsForUser(_userId);
        //            PlaylistBox.ItemsSource = _playlistList;
        //        }));
        //    }
        //}

        //Rename Playlist Methods
        //Open Rename Playlist Window
        private void RenamePlaylist_Click(object sender, RoutedEventArgs e)
        {
            RenamePlaylist renameList = new RenamePlaylist(this);
            renameList.Show();
        }
        //End Rename Playlist Methods

        //Remove Playlist
        private void RemovePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistBox.SelectedItem != null)
            {
                Playlist SelectedPlaylist = (Playlist)PlaylistBox.SelectedItem;
                _sqlAccess.RemovePlaylist(SelectedPlaylist.P_Id, _userId);
               Dispatcher.BeginInvoke(new Action(delegate()
                {
                    _songList = _sqlAccess.GetSongsForUser(_userId);
                    SongDataGrid.ItemsSource = _songList;
                    _playlistList = _sqlAccess.GetPlaylistsForUser(_userId);
                    PlaylistBox.ItemsSource = _playlistList;
                }));
            }
        }

        //Add new Playlist Methods
        //Open Window to create new Playlist
        private void AddPlaylistPopup_Click(object sender, RoutedEventArgs e)
        {
            AddPlaylist addPlaylist = new AddPlaylist(this);
            addPlaylist.Show();
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
                Dispatcher.BeginInvoke(new Action(delegate()
                {
                    _songList = _sqlAccess.GetSongsForUser(_userId);
                    SongDataGrid.ItemsSource = _songList;
                    _playlistList = _sqlAccess.GetPlaylistsForUser(_userId);
                    PlaylistBox.ItemsSource = _playlistList;
                }));
                AddSongToPlaylistPopup.IsOpen = false;
            }
        }

        //Cancel adding the song to another playlist and close
        private void CloseAddSongToPlaylistPopup_Click(object sender, RoutedEventArgs e)
        {
            AddSongToPlaylistPopup.IsOpen = false;
            ChoosePlaylist.SelectedIndex = -1;
        }

        private void Collection_Click(object sender, RoutedEventArgs e)
        {
            SongDataGrid.ItemsSource = _songList;
        }

        private void PlaylistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.SelectedItem != null)
            {
                Playlist p = (Playlist)lb.SelectedItem;
                SongDataGrid.ItemsSource = _sqlAccess.GetSongsInPlaylist(p.P_Id);
            }
            //using (var context = new CloudMp3SQLContext())
            //{
            //}
            
        }
        //End Add Song to Playlist methods
	}
}
