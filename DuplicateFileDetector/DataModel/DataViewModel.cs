using System.Collections.ObjectModel;

namespace Kavitesh.DataModel
{
    public class DataViewModel
    {
        public string DirectoryName { get; set; }

        public string TimeToScan { get; set; }

        public ObservableCollection<FileDetail> DuplicateFileList { get; set; }

        public DataViewModel()
        {
            this.DuplicateFileList = new ObservableCollection<FileDetail>();
        }

        public void AddFileEntryToDataModel(FileDetail FileInfo)
        {
            this.DuplicateFileList.Add(FileInfo);
        }

        public void Clear()
        {
            this.DirectoryName = null;
            this.TimeToScan = null;
            this.DuplicateFileList.Clear();
        }

    }
}
