using System;

using System.IO;
using System.Data.SQLite;


namespace Kavitesh.Database
{
    class DBEngine
    {
        #region Properties

        private string _dbFile = null;


        public string DBFile
        {
            get { return _dbFile; }
        }

        private SQLiteConnection _SqlConnection = null;


        public SQLiteConnection SqlConnection
        {
            get { return _SqlConnection; }
        }


        #endregion

        public void CreateFileDB(string Directory, string DBFileName)
        {
            if(_dbFile == null || _dbFile.Length == 0)
            {
                _dbFile = Directory + "\\" + DBFileName;
                SQLiteConnection.CreateFile(_dbFile);
            }
            else
            {
                string error = "DB: " + _dbFile + " already exist. Create a new instance of engine.";
                throw new Exception(error);
            }
        }


        public void CreateFileDB(string DBFilePath)
        {
            if(_dbFile == null || _dbFile.Length == 0)
            {
                _dbFile = DBFilePath;
                SQLiteConnection.CreateFile(_dbFile);
            }
            else
            {
                string error = "DB: " + _dbFile + " already exist. Create a new instance of engine.";
                throw new Exception(error);
            }
        }


        public void DeleteFileDB(string DBFilePath)
        {
            if(_dbFile != null && _dbFile.Length > 0)
            {
                this.DisconnectDB();
                File.Delete(_dbFile);
                _dbFile = null;
            }
        }


        public Boolean CreateInMemoryDB()
        {
            if(_SqlConnection == null)
            {
                // http://www.connectionstrings.com/sqlite/ 
                /* 
                 * http://www.sqlite.org/inmemorydb.html
                 * When this is done, no disk file is opened. Instead, a new database is created purely in memory. 
                 * The database ceases to exist as soon as the database connection is closed. 
                 * Every :memory: database is distinct from every other. So, opening two database 
                 * connections each with the filename ":memory:" will create two independent in-memory databases.
                */
                // Eg: Data Source=c:\mydb.db;Version=3;
                // Data Source=:memory:;Version=3;New=True;
                //string ConnectionString = "Data Source="+ _dbFile +";Version=3;";

                string ConnectionString = "Data Source=:memory:;Version=3;New=True;";
                _SqlConnection = new SQLiteConnection(ConnectionString);
                _SqlConnection.Open();
                return true;
            }
            else
                return false;
        }


        public void DeleteInMemoryDB()
        {
            // Just disconnect it and we don't need to delete any memory file.
            this.DisconnectDB();
        }


        public void DisconnectDB()
        {
            // This will delete the in memory DB after closing it.
            // For other file type DB, we need to delete the file using System IO API after closing it.
            if(_SqlConnection != null)
            {
                _SqlConnection.Close();
                /*
                 * http://stackoverflow.com/questions/8511901/system-data-sqlite-close-not-releasing-database-file
                 * http://www.sqlite.org/inmemorydb.html
                 * What happens when you call SQLiteConnection.Close() is that (along with a number of checks and other things) 
                 * the SQLiteConnectionHandle that points to the SQLite database instance is disposed. This is done through a 
                 * call to SQLiteConnectionHandle.Dispose(), however this doesn't actually release the pointer until the 
                 * CLR's Garbage Collector performs some garbage collection. Since SQLiteConnectionHandle overrides the 
                 * CriticalHandle.ReleaseHandle() function to call sqlite3_close_interop() (through another function) 
                 * this does not close the database.
                 * From my point of view this is a very bad way to do things since the programmer is not actually 
                 * certain when the database gets closed, but that is the way it has been done so I guess we have 
                 * to live with it for now, or commit a few changes to System.Data.SQLite. Any volunteers are 
                 * welcome to do so, unfortunately I am out of time to do so before next year.
                 * 
                 * TL;DR The solution is to force a GC after your call to SQLiteConnection.Close() and before your call to File.Delete().
                 */
                GC.Collect();
                _SqlConnection = null;
            }
        }


        public void CreateTable(string TableQueryString)
        {
            if(_SqlConnection != null)
            {
                using(SQLiteCommand command = new SQLiteCommand(TableQueryString, _SqlConnection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        public void DeleteTable(string TableName)
        {
            if(_SqlConnection != null)
            {
                using(SQLiteCommand command = new SQLiteCommand(_SqlConnection))
                {
                    command.CommandText = "DROP TABLE IF EXISTS " + TableName;
                    command.ExecuteNonQuery();
                }
            }
        }


        public DBEngine()
        {

        }


    }
}
