using System;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel;
using System.IO;

using Kavitesh.IO;
using Kavitesh.Database;
using Kavitesh.DataModel;


/*
Go to the TOOLS->Library Package Manager->Package Manager Console of the Visual studio.
Then run the command Install-Package System.Data.SQLite
*/

namespace DuplicateFileDetector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker workerThread;
        public UInt64 DirCount { get; set; }
        public UInt64 FileCount { get; set; }

        string _folderSelected = null;

        DBEngine _dbEngine = null;
        DirFileEnumeration _dirEnum = null;

        bool _scanInProgress = false;

        public DataViewModel DataModel { get; set; }

        DateTime start;
        DateTime end;


        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if(_dirEnum != null)
            {
                _dirEnum.StopEnumeration();
            }

            if(_dbEngine != null)
            {
                _dbEngine.DeleteInMemoryDB();
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "Duplicate File Detector \u00A9Kavitesh Singh 2016";

            //throw new NotImplementedException();
            Console.WriteLine("");
            workerThread = new BackgroundWorker();
            workerThread.WorkerReportsProgress = true;
            workerThread.DoWork += workerThread_DoWork;
            workerThread.RunWorkerCompleted += workerThread_RunWorkerCompleted;
            workerThread.ProgressChanged += workerThread_ProgressChanged;

            // Create InMemory DB.    
            _dbEngine = new DBEngine();
            _dbEngine.CreateInMemoryDB();

            _dirEnum = new DirFileEnumeration();
            _dirEnum.FileFound += DirEnum_FileFound;

            DataModel = new DataViewModel();
            ResultDataGrid.ItemsSource = DataModel.DuplicateFileList;
        }

        private void DirScanButton_Click(object sender, RoutedEventArgs e)
        {
            if(_scanInProgress == false)
            {
                DirCount = 0;
                FileCount = 0;
                CurrentFileLabel.Content = null;
                CurrentFileSizeLabel.Content = null;
                CurrentFileSizeLabel.Content = null;
                TotalFilesLabel.Content = null;
                TotalTimeLabel.Content = null;

                DataModel.Clear();

                _dbEngine.DeleteTable(@"FileDB");
                _dbEngine.CreateTable(DBQueries.FileTableCreateQuery);

                start = DateTime.Now;
                workerThread.RunWorkerAsync();
                DirScanButton.Content = "Stop Scan";
                _scanInProgress = true;
            }
            else
            {
                if(_dirEnum != null)
                {
                    _dirEnum.StopEnumeration();
                }

                DirScanButton.Content = "Begin Scan";

                // Disable the button here because if App is scanning a big file in the middle
                // we have to wait for it to finish and cannot terminate the thread right away.
                // If we dont disable this, user can click the begin scan again and this causes
                // crash. When we stop the scan, workerThread_RunWorkerCompleted will be called and
                // and we enable the button again.
                DirScanButton.IsEnabled = false;
            }
        }

        void workerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FileInfo fileInfo = e.UserState as FileInfo;
            TotalFilesLabel.Content = FileCount.ToString();
            CurrentFileLabel.Content = fileInfo.Name;
            CurrentFileSizeLabel.Content = (fileInfo.Length / 1000).ToString() + " KB";
        }

        void workerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            end = DateTime.Now;
            TotalFilesLabel.Content = FileCount.ToString();
            TotalTimeLabel.Content = (end - start).TotalSeconds.ToString() + " seconds.";

            DataModel.TimeToScan = (end - start).TotalSeconds.ToString();
            // When we stop the scan in between, we don't drop the table or clean it.
            // We print all the results we have populated so far. 
            DataModel.Clear();
            DBQueries.FindDuplicateEntries(this._dbEngine, this.DataModel);

            _scanInProgress = false;
            DirScanButton.IsEnabled = true;
        }

        void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            FileCount = 0;
            DirCount = 0;

            if(_folderSelected != null && _folderSelected.Length > 0)
            {
                _dirEnum.EnumerateFiles(_folderSelected);
            }
        }

        void DirEnum_FileFound(object sender, FileFoundEventArgs e)
        {
            FileCount++;
            workerThread.ReportProgress(1, e.fileInfo);
            string MD5Hash = Kavitesh.Hashing.MD5Hash.GetMD5HashFromFile(e.fileInfo.FullName);
            DBQueries.CreateFileEntry(_dbEngine, e.fileInfo, MD5Hash);
        }


        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                _folderSelected = dialog.SelectedPath;
                TextBlockFolderPath.Text = _folderSelected;
            }
        }

        private void DeleteItemsButton_Click(object sender, RoutedEventArgs e)
        {
            /*
             * Make a list of selected items instead of iterating and deleting them because 
             * it will cause exception 'Collection was modified; enumeration operation may not execute'
             * So we need to make a temp list and then remove it from list.
             * http://stackoverflow.com/questions/604831/collection-was-modified-enumeration-operation-may-not-execute
             * http://stackoverflow.com/questions/2024179/c-sharp-collection-was-modified-enumeration-operation-may-not-execute
            */

            List<FileDetail> selectedItems = new List<FileDetail>();

            foreach(var item in ResultDataGrid.SelectedItems)
            {
                selectedItems.Add(item as FileDetail);
            }

            foreach(var item in selectedItems)
            {
                try
                {
                    File.Delete(item.FullFilePath);
                }
                catch(Exception Ex)
                {
                    System.Windows.MessageBox.Show(Ex.Message);
                }
                DataModel.DuplicateFileList.Remove(item);

            }
        }

    }
}

/*

longWorkTextBox.Text = "Ready For Work!";
Action workAction = delegate
{
    BackgroundWorker worker = new BackgroundWorker();
    worker.DoWork += delegate
    {
        Console.WriteLine("Starting Slow Work");
        int i = int.MaxValue;
        while (i > 0)
        i--;
        Console.WriteLine("Ending Work Action");
    };
    worker.RunWorkerCompleted += delegate
    {
        longWorkTextBox.Text = "Work Complete";
    };
    worker.RunWorkerAsync();
 };
 longWorkTextBox.Dispatcher.BeginInvoke(DispatcherPriority.Background, workAction);

 
*/