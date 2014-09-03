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
    /// Interaction logic for AddPlaylist.xaml
    /// </summary>
    public partial class AddPlaylist : Window
    {
        private MainWindow _mainWindow;
        private SqlAccess _sqlAccess = new SqlAccess();
        private ObservableCollection<Song> _songList;
        private ObservableCollection<Playlist> _playlistList;
        private int _userId;

        public AddPlaylist(MainWindow window)
        {
            InitializeComponent();
            this.DataContext = this;
            _mainWindow = window;
        }

        //Add the new Playlist and close popup
        private void AddList_Click(object sender, RoutedEventArgs e)
        {
            _userId = MainWindow.UserId;

            string NewPlaylistName = PlaylistNameBox.Text;

            if (!string.IsNullOrWhiteSpace(NewPlaylistName))
            {
                Playlist NewPlaylist = new Playlist();
                NewPlaylist.P_Name = NewPlaylistName;
                _sqlAccess.AddPlaylist(NewPlaylist, _userId);
                Dispatcher.BeginInvoke(new Action(delegate()
                {
                    _songList = _sqlAccess.GetSongsForUser(_userId);
                    _mainWindow.SongDataGrid.ItemsSource = _songList;
                    _playlistList = _sqlAccess.GetPlaylistsForUser(_userId);
                    _mainWindow.PlaylistBox.ItemsSource = _playlistList;
                }));
            }
            this.Close();
        }

        //Close the popup and cancel adding the playlist
        private void ClosePlaylistPopup_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
