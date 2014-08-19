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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Cloudmp3
{

	public partial class MainWindow : Window
	{
		private ObservableCollection<string> songList;
		private ObservableCollection<string> cloudSongList;
		private IMp3Player localPlayer;
		private AzureAccess blobAccess;

		private BitmapImage _playImage = new BitmapImage(new Uri("Images/Play.png", UriKind.Relative));
		private BitmapImage _pauseImage = new BitmapImage(new Uri("Images/Pause.png", UriKind.Relative));

		private int CurrentSongIndex { get; set; }

		private bool _isPaused;

		public bool isPaused
		{
			get { return _isPaused; }
			set
			{
				_isPaused = value;
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
				isPaused = false;
				blobAccess = new AzureAccess();
				localPlayer = new StreamMp3Player();
				cloudSongList = blobAccess.GetCloudSongs();
				SongsListBox.ItemsSource = cloudSongList;
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
			Play.DataContext = isPaused;
		}

		private void PlayButtonSwap()
		{
			((Image)(Play.Content)).Source = (isPaused) ? _pauseImage : _playImage;
		}

		private void Play_Click(object sender, RoutedEventArgs e)
		{
			if (!isPaused)
			{
				if (SongsListBox.SelectedIndex == -1)
				{
					isPaused = true;
					SongsListBox.SelectedIndex = ++SongsListBox.SelectedIndex;
					CurrentSongIndex = SongsListBox.SelectedIndex;
					localPlayer.Play((string)SongsListBox.SelectedItem + blobAccess.GetSaS());
				}
				else
				{
					isPaused = true;
					CurrentSongIndex = SongsListBox.SelectedIndex;
					localPlayer.Play((string)SongsListBox.SelectedItem + blobAccess.GetSaS());
				}
			}
			else
			{
				isPaused = false;
				localPlayer.Pause();
			} 
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			isPaused = false;
			localPlayer.Stop();
		}

		private void Pause_Click(object sender, RoutedEventArgs e)
		{
			isPaused = true;
			localPlayer.Pause();
		}

		private void Next_Click(object sender, RoutedEventArgs e)
		{
			isPaused = false;
			SongsListBox.SelectedIndex = (CurrentSongIndex == SongsListBox.Items.Count - 1) ? 0 : ++SongsListBox.SelectedIndex;
			CurrentSongIndex = SongsListBox.SelectedIndex;
			localPlayer.Play((string)SongsListBox.SelectedItem + blobAccess.GetSaS());
		}

		private void Previous_Click(object sender, RoutedEventArgs e)
		{
			isPaused = false;
			SongsListBox.SelectedIndex = (CurrentSongIndex <= 0) ?SongsListBox.Items.Count - 1 : --SongsListBox.SelectedIndex;
			CurrentSongIndex = SongsListBox.SelectedIndex;
			localPlayer.Play((string)SongsListBox.SelectedItem + blobAccess.GetSaS());
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
			isPaused = false;
			CurrentSongIndex = SongsListBox.SelectedIndex;
			localPlayer.Play((string)SongsListBox.SelectedItem + blobAccess.GetSaS());
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
						SongsListBox.ItemsSource = cloudSongList;
					}));
				});
			}
		}

		private void DownloadFile()
		{
			if (SongsListBox.SelectedIndex != -1)
			{
				string path = (string)SongsListBox.SelectedItem;
				Task.Factory.StartNew(() =>
				{
					blobAccess.DownloadSong(Path.GetFileName(path));
					Dispatcher.BeginInvoke(new Action(delegate()
					{
						songList = new ObservableCollection<string>(Directory.GetFiles("C:/Users/Public/Music/CloudMp3", "*.mp3"));
						SongsListBox.ItemsSource = songList;
					}));

				});
			}
		}
	}
	
		// This is the base code for grabbing songs corresponding to a specific user by User ID
	// It may still need a few tweaks.
	//private static void GetUserSongs(string UserID)
	//{
	//    int U_Id = int.Parse(UserID);

	//    using (CloudMp3SQLContext context = new CloudMp3SQLContext())
	//    {
	//        var Songs = from s in context.Songs
	//            where s.S_OwnerID == U_Id;
	//    }
	//}	

    //This code for the logic of the login screen where the username and password are store in an array (which can be 
    //changed to any other method of choice) and with events for button click and the textbox login the user in thus turning the login in screen invisible.

    // This is also a button event which allows for a new user to be stored in the array.

    //public int times = 0;
    //public string[] usernames = new string[10];
    //public string[] passwords = new string[10];
    //private void CorrectCredentials(object sender, RoutedEventArgs e)
    //{

    //    if (pass.Text == "JAM" && usn.Text == "SAM")
    //    {
    //        plausibleInput.Visibility = Visibility.Visible;
    //        Sta.Visibility = Visibility.Hidden;
    //    }
    //    for (int i = 0; i < 10; )
    //    {
    //        if (usn.Text == usernames[i] && pass.Text == passwords[i])
    //        {
    //            plausibleInput.Visibility = Visibility.Visible;
    //            Sta.Visibility = Visibility.Hidden;
    //            i = 10;
    //        }
    //        else
    //        {
    //            usn.Text = "";
    //            pass.Text = "";
    //            if(usn.Text != "" && pass.Text != "" || usn.Text != usernames[i] && pass.Text != passwords[i])
    //            {
    //                MessageBox.Show("Incorrect Credentials");
    //            }
    //            i = 10;

    //        }
    //        i++;
    //    }
    //}

    //private void Enterexi(object sender, KeyEventArgs e)
    //{
    //    if (e.Key == Key.Return)
    //    {
    //        this.CorrectCredentials(sender, e);
    //    }
    //}

    //private void Newuser(object sender, RoutedEventArgs e)
    //{

    //    usernames[times] = usn.Text;
    //    passwords[times] = pass.Text;

    //    times++;
    //    if (times > usernames.Length && times > passwords.Length)
    //    {
    //        MessageBox.Show("Max account nums capped.");
    //    }
    //}
        
}
