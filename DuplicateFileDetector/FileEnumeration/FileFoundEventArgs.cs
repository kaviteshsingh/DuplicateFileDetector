using System;
using System.IO;


namespace Kavitesh.IO
{
    public class FileFoundEventArgs : EventArgs
    {
        public FileInfo fileInfo { get; set; }

        public FileFoundEventArgs(FileInfo inFileInfo)
        {
            fileInfo = inFileInfo;
        }
    }
}
