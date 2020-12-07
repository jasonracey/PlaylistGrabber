﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlaylistGrabber
{
    public partial class PlaylistGrabber : Form
    {
        private const string FileSearchPattern = @"*.m3u";

        private readonly IDownloader downloader;

        public PlaylistGrabber(IDownloader downloader)
        {
            this.downloader = downloader ??
                throw new ArgumentNullException(nameof(downloader));

            InitializeComponent();
        }

        private void PlaylistGrabber_Load(object sender, EventArgs e)
        {
            SetStateIdle();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    AddPlaylistContentsToListBox(fileName);
                }
            }
            SetButtonState();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            SetStateIdle();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            SetStateBusy();

            var sourcePaths = listBox.Items.Cast<string>();

            timer.Interval = 500;
            timer.Start();

            Task.Run(() =>
            {
                this.downloader.DownloadFiles(sourcePaths);
            })
            .ContinueWith(task => timer.Stop(), TaskScheduler.FromCurrentSynchronizationContext())
            .ContinueWith(task => SetStateIdle(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void listBox_DragDrop(object sender, DragEventArgs e)
        {
            foreach (var path in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (Directory.Exists(path))
                {
                    var playlistFilePaths = Directory.GetFiles(path, FileSearchPattern, SearchOption.AllDirectories);
                    foreach (var playlistFilePath in playlistFilePaths)
                    {
                        AddPlaylistContentsToListBox(playlistFilePath);
                    }
                }
                else if (File.Exists(path))
                {
                    AddPlaylistContentsToListBox(path);
                }
            }
            SetButtonState();
        }

        private void listBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.downloader.State != null)
            {
                labelMessage.Text = this.downloader.State;
            }

            SetProgressLabel(this.downloader.DownloadedFiles, this.downloader.TotalFiles);

            progressBar.Maximum = this.downloader.TotalFiles;
            progressBar.Value = this.downloader.DownloadedFiles;
        }

        private void AddPlaylistContentsToListBox(string playlistFilePath)
        {
            foreach (var url in File.ReadAllLines(playlistFilePath)
                .Where(url => !string.IsNullOrWhiteSpace(url)))
            {
                Uri uri;
                try
                {
                    uri = new Uri(url);
                }
                catch (Exception)
                {
                    continue;
                }
                if (!listBox.Items.Contains(uri))
                {
                    listBox.Items.Add(uri);
                }
            }
        }

        private void SetButtonState()
        {
            var listBoxHasItems = listBox.Items.Count > 0;
            buttonStart.Enabled = listBoxHasItems;
            buttonClear.Enabled = listBoxHasItems;
        }

        private void SetProgressLabel(int downloadedFiles, int totalFiles)
        {
            labelProgress.Text = $@"{downloadedFiles}/{totalFiles}";
        }

        private void SetStateBusy()
        {
            buttonBrowse.Enabled = false;
            buttonClear.Enabled = false;
            buttonStart.Enabled = false;
            labelMessage.Text = @"Downloading files";
            listBox.Enabled = false;
        }

        private void SetStateIdle()
        {
            buttonBrowse.Enabled = true;
            buttonClear.Enabled = false;
            buttonStart.Enabled = false;
            labelMessage.Text = @"Drop playlists here";
            listBox.Enabled = true;
            listBox.Items.Clear();
            progressBar.Value = 0;
            SetProgressLabel(0, 0);
            timer.Enabled = false;
        }
    }
}
