﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Cloudmp3.Progresin
{
    /// <summary>
    /// Interaction logic for ProgresControls.xaml
    /// </summary>
    public partial class ProgresControls : UserControl
    {
        BackgroundWorker backgroundWorker1 = new BackgroundWorker();

        public ProgresControls()
        {
            InitializeComponent();
            //backgroundWorker1.WorkerReportsProgress = true;

            //backgroundWorker1.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
            //backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            //backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            //backgroundWorker1.RunWorkerAsync();
        }


        public void changLAB(string message)
        {
            proce.Content = message;
        }

        //public void start()
        //{
        //    backgroundWorker1.RunWorkerAsync();
        //}

        //private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    for (int i = 0; i <= 100; i++)
        //    {
        //        Thread.Sleep(90);
        //        backgroundWorker1.ReportProgress(i);
        //    }
        //    backgroundWorker1.ReportProgress(100);
        //}

        //private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    progbar.Value = e.ProgressPercentage;

        //}

        //private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    //Prog.Visibility = Visibility.Hidden;
        //}

        private void Hide_Click(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
