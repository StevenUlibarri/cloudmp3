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
    /// Interaction logic for RenamePlaylist.xaml
    /// </summary>
    public partial class RenamePlaylist : Window
    {
        private MainWindow _mainWindow;
        private SqlAccess _sqlAccess = new SqlAccess();
        private ObservableCollection<Song> _songList;
        private ObservableCollection<Playlist> _playlistList;
        private int _userId;

        public RenamePlaylist(MainWindow window)
        {
            InitializeComponent();
            this.DataContext = this;
            _mainWindow = window;
        }

        //Rename the Playlist and Close
        private void RenameList_Click(object sender, RoutedEventArgs e)
        {
            _userId = MainWindow.UserId;

            string NewPlaylistName = PlaylistRenameBox.Text;

            if (!string.IsNullOrWhiteSpace(NewPlaylistName))
            {
                Playlist SelectedPlaylist = (Playlist)_mainWindow.PlaylistBox.SelectedItem;
                _sqlAccess.RenamePlaylist(SelectedPlaylist.P_Id, SelectedPlaylist.P_OwnerId, NewPlaylistName);
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

        //Cancel Rename and Close
        private void CloseRenamePopup_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //End Rename Playlist Methods

    }
}
