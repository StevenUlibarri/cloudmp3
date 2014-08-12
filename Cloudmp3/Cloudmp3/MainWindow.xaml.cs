using Cloudmp3.AzureBlobClasses;
using Microsoft.Win32;
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

namespace Cloudmp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            //testing blob upload
            new BlobClass().testBlobUpload();
        }

        private void CopyFile() //Added using Microsoft.Win32
        {
            OpenFileDialog ChooseFile = new OpenFileDialog();
            ChooseFile.Filter = "Music Files (.mp3)|*.mp3|All Files (*.*)|*.*";
            ChooseFile.FilterIndex = 1;
            ChooseFile.ShowDialog();
            String File = ChooseFile.FileName;
            String Destination = @"C:\Test\" + ChooseFile.SafeFileName;
            System.IO.File.Copy(@File, @Destination);
        }
    }
}
