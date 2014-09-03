using Cloudmp3.AzureBlobClasses;
using Cloudmp3.DataAccessLayer;
using Cloudmp3.Mp3Players;
using Cloudmp3.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cloudmp3
{
    /// <summary>
    /// Interaction logic for AddSongToPlaylist.xaml
    /// </summary>
    public partial class AddSongToPlaylist : Window
    {
        private MainWindow _mainWindow;
        private SqlAccess _sqlAccess = new SqlAccess();
        private ObservableCollection<Song> _songList;
        private ObservableCollection<Playlist> _playlistList;
        public ObservableCollection<Playlist> PlaylistList
        {
            get { return _playlistList; }
        }

        private int _userId = MainWindow.UserId;

        public AddSongToPlaylist(MainWindow window)
        {
            InitializeComponent();
            this.DataContext = this;
            _mainWindow = window;
            _playlistList = _sqlAccess.GetPlaylistsForUser(_userId);
        }

        //Add the song to the selected playlist and close
        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (_mainWindow.SongDataGrid.SelectedItem != null && ChoosePlaylist.SelectedIndex != -1)
            {
                Song SelectedSong = (Song)_mainWindow.SongDataGrid.SelectedItem;
                Playlist SelectedPlaylist = (Playlist)ChoosePlaylist.SelectedItem;
                _sqlAccess.AddSongToPlaylist(SelectedSong.S_Id, SelectedPlaylist.P_Id);
                Dispatcher.BeginInvoke(new Action(delegate()
                {
                    _songList = _sqlAccess.GetSongsForUser(_userId);
                    _mainWindow.SongDataGrid.ItemsSource = _songList;
                    _playlistList = _sqlAccess.GetPlaylistsForUser(_userId);
                    _mainWindow.PlaylistBox.ItemsSource = _playlistList;
                }));
                this.Close();
            }
        }

        //Cancel adding the song to another playlist and close
        private void CloseAddSongToPlaylistPopup_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
