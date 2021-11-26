using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MediaPlayer
{
    class CorePlayer
    {
        private const int BUFFER_LENGTH = 128;
        private StringBuilder _buffer;
        private string _command;

        [DllImport("winmm.dll")]
        private static extern int mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        public CorePlayer()
        {
            _buffer = new StringBuilder(BUFFER_LENGTH);
        }

        // Mở file media
        public void Open(string fileName, IntPtr handle)
        {
            _command = "open \"" + fileName + "\" type mpegvideo alias NPThanhMedia style child parent " + handle;
            mciSendString(_command, null, 0, IntPtr.Zero);
        }

        // Chạy file media
        public void Play(bool repeat)
        {
            _command = "play NPThanhMedia";
            if (repeat)
                _command += " repeat";
            mciSendString(_command, null, 0, IntPtr.Zero);
        }

        public void Play()
        {
            Play(false);
        }

        // Dừng file media
        public void Stop()
        {
            _command = "close NPThanhMedia";
            mciSendString(_command, null, 0, IntPtr.Zero);
        }
        // Tạm ngưng file media
        public void Pause()
        {
            _command = "pause NPThanhMedia";
            mciSendString(_command, null, 0, IntPtr.Zero);
        }

        // Chạy file media tại vị trí bất kì
        public void Seek(int miliSeconds)
        {
            if (this.Status.Equals("playing", StringComparison.OrdinalIgnoreCase))
            {
                _command = "play NPThanhMedia from " + miliSeconds;
                mciSendString(_command, null, 0, IntPtr.Zero);
                
            }
            else
            {
                _command = "seek NPThanhMedia to" + miliSeconds;
                mciSendString(_command, null, 0, IntPtr.Zero);
            }
        }

        // Lấy trạng thái của Player
        public string Status
        {
            get
            {
                _command = "status NPThanhMedia mode";
                mciSendString(_command, _buffer, BUFFER_LENGTH, IntPtr.Zero);
                return _buffer.ToString();
            }
        }

        // Lấy chiều dài của file media
        public int Length
        {
            get
            {
                _command = "status NPThanhMedia length";
                mciSendString(_command, _buffer, BUFFER_LENGTH, IntPtr.Zero);
                try
                {
                    return (int)Math.Floor(Convert.ToDouble(_buffer.ToString().Trim()));
                }
                catch 
                {

                    return 0;
                }
            }
        }

        // Lấy vị trí hiện tại
        public int CurrentMiliSeconds
        {
            get
            {
                _command = "status NPThanhMedia position";
                try
                {
                    mciSendString(_command, _buffer, BUFFER_LENGTH, IntPtr.Zero);
                    return (int)Math.Floor(Convert.ToDouble(_buffer.ToString().Trim()));
                }
                catch 
                {
                    
                    return 0;
                }
            }
            set
            {
                Seek(value);
            }
        }
    }
}
