using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaPlayer
{
    class MediaFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int FileNumber { get; set; }

        public MediaFile(){}

        public MediaFile(string _filePath)
        {
            FilePath = _filePath;

            string _fileName = _filePath.Substring(_filePath.LastIndexOf("\\") + 1);
            FileName = _fileName;
        }
    }
}
