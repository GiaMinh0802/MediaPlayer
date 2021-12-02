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
        LinkedList<LinkedList<MediaFile>> playlist = new LinkedList<LinkedList<MediaFile>>();
        LinkedListNode<MediaFile> iCurFile;
        LinkedListNode<LinkedList<MediaFile>> iCurPlaylist;
        LinkedList<string> nameplaylist = new LinkedList<string>();
        LinkedListNode<string> iCurNamePlaylist;
        string name;
        public MediaPlayer()
        {
            InitializeComponent();
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string[] album = Directory.GetDirectories(fbd.SelectedPath); 
                string[] pl = null;
                int i = 0;
                foreach (string item in album)
                {
                    name = item.Substring(item.LastIndexOf("\\") + 1);
                    nameplaylist.AddLast(name);
                    name = "";
                    LinkedList<MediaFile> lstFile = new LinkedList<MediaFile>();
                    pl = null;
                    pl = Directory.GetFiles(item);
                    foreach (string a in pl)
                    {
                        MediaFile file = new MediaFile(a);
                        file.FileNumber = i;
                        lstFile.AddLast(file);
                        i++;
                    }  
                    playlist.AddLast(lstFile);   
                    i = 0;
                }
                iCurPlaylist = playlist.First;
                iCurFile = iCurPlaylist.Value.First;
                iCurNamePlaylist = nameplaylist.First;
                label2.Text = iCurNamePlaylist.Value;
                LoadPlayList(iCurPlaylist.Value);
                Processor();
                btnPlay.Text = "Pause";
            }
        }
        private void LoadPlayList(LinkedList<MediaFile> playlst)
        {
            lstPlayList.DataSource = null;
            lstPlayList.DataSource = playlst.ToList();
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
            else
            {
                if (iCurPlaylist.Previous != null)
                {
                    iCurPlaylist = iCurPlaylist.Previous;
                    iCurFile = iCurPlaylist.Value.First;
                    iCurNamePlaylist = iCurNamePlaylist.Previous;
                    label2.Text = iCurNamePlaylist.Value;
                    LoadPlayList(iCurPlaylist.Value);
                    Processor();
                }
            }    
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (iCurFile.Next != null)
            {
                iCurFile = iCurFile.Next;
                Processor();
            }
            else
            {
                if (iCurPlaylist.Next != null)
                {
                    iCurPlaylist = iCurPlaylist.Next;
                    iCurFile = iCurPlaylist.Value.First;
                    iCurNamePlaylist = iCurNamePlaylist.Next;
                    label2.Text = iCurNamePlaylist.Value;
                    LoadPlayList(iCurPlaylist.Value);
                    Processor();
                }
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
        private void PrevPlaylist_Click(object sender, EventArgs e)
        {
            if (iCurPlaylist.Previous != null)
            {
                iCurPlaylist = iCurPlaylist.Previous;
                iCurFile = iCurPlaylist.Value.First;
                iCurNamePlaylist = iCurNamePlaylist.Previous;
                label2.Text = iCurNamePlaylist.Value;
                LoadPlayList(iCurPlaylist.Value);
                Processor();
            }
        }

        private void NextPlaylist_Click(object sender, EventArgs e)
        {
            if (iCurPlaylist.Next != null)
            {
                iCurPlaylist = iCurPlaylist.Next;
                iCurFile = iCurPlaylist.Value.First;
                iCurNamePlaylist = iCurNamePlaylist.Next;
                label2.Text = iCurNamePlaylist.Value;
                LoadPlayList(iCurPlaylist.Value);
                Processor();
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            int position = _player.CurrentMiliSeconds;
            if (position == _player.Length && iCurFile.Next != null)
            {
                position = 0;
                iCurFile = iCurFile.Next;
                Processor();
            }
            else if (position == _player.Length && iCurFile.Next == null && iCurPlaylist.Next != null)
            {
                position = 0;
                iCurPlaylist = iCurPlaylist.Next;
                iCurFile = iCurPlaylist.Value.First;
                iCurNamePlaylist = iCurNamePlaylist.Next;
                label2.Text = iCurNamePlaylist.Value;
                LoadPlayList(iCurPlaylist.Value);
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