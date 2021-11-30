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


namespace MediaPlayer
{
    public partial class MediaPlayer : Form
    {
        CorePlayer _player = new CorePlayer();
        LinkedList<MediaFile> lstFile = new LinkedList<MediaFile>();
        LinkedListNode<MediaFile> iCurFile;
        public MediaPlayer()
        {
            InitializeComponent();
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                lstFile.Clear();
                string[] list = Directory.GetFiles(fbd.SelectedPath);
                int i = 0;
                foreach (string item in list)
                {
                    MediaFile file = new MediaFile(item);
                    file.FileNumber = i;
                    lstFile.AddLast(file);
                    i++;
                }
                iCurFile = lstFile.First;
                LoadPlayList();
                Processor();
                btnPlay.Text = "Pause";
            }
        }
        private void LoadPlayList()
        {
            lstPlayList.DataSource = null;
            lstPlayList.DataSource = lstFile.ToList();
            lstPlayList.DisplayMember = "FileName";
        }
        private void Processor()
        {
            _player.Stop();
            MediaFile file = iCurFile.Value;
            _player.Open(file.FilePath, pnScreen.Handle);
            _player.Play();
            trackBar.Minimum = 0;
            trackBar.Maximum = _player.Length;
            timer.Start();
            this.Text = "Media Player" + " - " + iCurFile.Value.FileName.Replace(".mp3", "");
            lstPlayList.SelectedIndex = iCurFile.Value.FileNumber;
        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (_player.Status == "playing")
            {
                btnPlay.Text = "Play";
                _player.Pause();
                timer.Stop();
            }
            else if (_player.Status == "paused")
            {
                btnPlay.Text = "Pause";
                _player.Play();
                timer.Start();
            }
            else
            {
                Processor();
                btnPlay.Text = "Pause";
            }
        }
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (iCurFile.Previous != null)
            {
                iCurFile = iCurFile.Previous;
                Processor();
            }
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (iCurFile.Next != null)
            {
                iCurFile = iCurFile.Next;
                Processor();
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            _player.Stop();
            timer.Stop();
            this.Text = "Media Player";
            trackBar.Value = 0;
            btnPlay.Text = "Play";
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            int position = _player.CurrentMiliSeconds;
            if (position == _player.Length && iCurFile.Next != null)
            {
                iCurFile = iCurFile.Next;
                Processor();
            }
            trackBar.Value = position;
        }
        private void trackBar_MouseUp(object sender, MouseEventArgs e)
        {
            _player.CurrentMiliSeconds = trackBar.Value;
        }

        private void MediaPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            _player.Stop();
        }
    }
}