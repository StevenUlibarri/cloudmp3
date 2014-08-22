using Cloudmp3.Mp3Players;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cloudmp3.UserControls
{
    /// <summary>
    /// Interaction logic for Mp3Player.xaml
    /// </summary>
    public partial class Mp3Player : UserControl
    {
        public Song CurrentSong { get; set; }
        public IMp3Player Player { get; set; }

        public Mp3Player()
        {
            InitializeComponent();         
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
