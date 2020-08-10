using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Threading;

namespace SftpDownloadUpload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Downloading started");
            label2.Text = "Downloading!";
            Thread.Sleep(1000);
            backgroundWorker1.RunWorkerAsync();  //This works perfectly
            Thread.Sleep(1000);

        }
        private void listFiles()
        {
            string host = "192.168.0.1";
            string username = "admin";
            string password = "admin";
            int count = 0;
            string remoteDirectory = @"c:\";

            using (SftpClient sftp = new SftpClient(host, username, password))
            {
                try
                {
                    sftp.Connect();

                    var files = sftp.ListDirectory(remoteDirectory);

                    foreach (var file in files)
                    {
                        count = count + 1;

                        if ((file.Name != ".") && (file.Name != ".."))
                        {

                            if ((file.LastWriteTime >= dateTimePicker1.Value) & (file.LastWriteTime <= dateTimePicker2.Value) & (file.Name.Contains("core")))
                            {
                                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), file.Name);
                                using (Stream fileStream = File.OpenWrite(path))
                                {

                                    sftp.DownloadFile(remoteDirectory + "/" + file.Name, fileStream);
                                }
                                //MessageBox.Show("working");

                            }

                        }
                        double percentage = (count / files.Count()) * 100;

                        backgroundWorker1.ReportProgress((int)percentage);

                        //Console.WriteLine(file.LastAccessTime);
                        //Console.WriteLine(file.FullName);
                        //MessageBox.Show(file.Name + " " + file.LastWriteTime + " " + file.LastAccessTime + " " + file.Attributes.Size);
                    }

                    sftp.Disconnect();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An exception has been caught " + e.ToString());
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            listFiles();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label2.Text = $"Downloaded {e.ProgressPercentage}%";
            label2.Update();
            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Update();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label2.Text = "Download Complete!";
            MessageBox.Show("Download Complete!");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
