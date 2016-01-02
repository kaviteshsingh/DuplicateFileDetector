using System;
using System.ComponentModel;

namespace Kavitesh.DataModel
{
    public class FileDetail : INotifyPropertyChanged
    {

        private string _FileName;
        public string FileName
        {
            get { return this._FileName; }
            set
            {
                if(this._FileName != value)
                {
                    this._FileName = value;
                    this.NotifyPropertyChanged("FileName");
                }
            }
        }

        private string _FullFilePath;
        public string FullFilePath
        {
            get { return this._FullFilePath; }
            set
            {
                if(this._FullFilePath != value)
                {
                    this._FullFilePath = value;
                    this.NotifyPropertyChanged("FullFilePath");
                }
            }
        }

        private Int64 _Size;
        public Int64 Size
        {
            get { return this._Size; }
            set
            {
                if(this._Size != value)
                {
                    this._Size = value;
                    this.NotifyPropertyChanged("Size");
                }
            }
        }

        private string _Hash;
        public string Hash
        {
            get { return this._Hash; }
            set
            {
                if(this._Hash != value)
                {
                    this._Hash = value;
                    this.NotifyPropertyChanged("Hash");
                }
            }
        }

        public FileDetail()
        {

        }

        public FileDetail(string Hash, string FileName, string FullFilePath, Int64 Size)
        {
            this.FileName = FileName;
            this.FullFilePath = FullFilePath;
            this.Size = Size;
            this.Hash = Hash;
        }

        public FileDetail(FileDetail fileDetail)
        {
            if(fileDetail != null)
            {
                this.FullFilePath = fileDetail.FullFilePath;
                this.FileName = fileDetail.FileName;
                this.Size = fileDetail.Size;
                this.Hash = fileDetail.Hash;

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if(this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
