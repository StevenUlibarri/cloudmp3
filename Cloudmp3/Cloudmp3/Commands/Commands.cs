using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cloudmp3.Commands
{
    public class Commands
    {
        public static RoutedUICommand Login = new RoutedUICommand("Login Command", "Login", typeof(Commands));
        public static RoutedUICommand Logout = new RoutedUICommand("Logout Command", "Logout", typeof(Commands));
        public static RoutedUICommand DownloadSong = new RoutedUICommand("Download Song Command", "DownloadSong", typeof(Commands));
        public static RoutedUICommand UploadSong = new RoutedUICommand("Upload Song Command", "UploadSong", typeof(Commands));
    }
}
