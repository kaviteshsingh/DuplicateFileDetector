using System;

using System.IO;
using System.Data.SQLite;

using Kavitesh.DataModel;

namespace Kavitesh.Database
{

    // http://blog.tigrangasparian.com/2012/02/09/getting-started-with-sqlite-in-c-part-one/
    // http://zetcode.com/db/sqlitecsharp/
    // http://www.tutorialspoint.com/sqlite/sqlite_syntax.htm

    class DBQueries
    {
        /*
        SELECT name, id, MD5Hash, COUNT(MD5Hash)
        FROM FileDB
        GROUP BY name, MD5Hash 
        HAVING (COUNT(MD5Hash) > 1)
        ORDER BY COUNT(MD5Hash) DESC
         */

        /*
SELECT s.Name, s.Size, s.Hash, s.lastmodtime, s.directory
    FROM FileDB s
        INNER JOIN (SELECT Hash
                        FROM FileDB
                        GROUP BY Hash
                        HAVING COUNT(*) > 1) q
            ON s.Hash = q.Hash
            ORDER BY s.Hash DESC
         * 
          SELECT s.id, s.Name, s.Size, s.Hash, s.Fullname
    FROM FileDB s
        INNER JOIN (SELECT Hash
                        FROM FileDB
                        GROUP BY Hash
                        HAVING COUNT(*) > 1) q
            ON s.Hash = q.Hash
            ORDER BY s.Hash DESC
         */





        // dont use @ as it adds newline char at the end. 
        public static string FileTableCreateQuery = "CREATE TABLE FileDB(Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                                                    "Name TEXT, FullName TEXT, Hash TEXT, Size INTEGER, FileExt TEXT, Directory TEXT, " +
                                                    "LastModTime INTEGER);";

        public static string DuplicateEntriesQuery = "SELECT s.id, s.Name, s.Size, s.Hash, s.Fullname " +
                                                        "FROM FileDB s " +
                                                        "INNER JOIN (SELECT Hash FROM FileDB GROUP BY Hash HAVING COUNT(*) > 1) q " +
                                                        "ON s.Hash = q.Hash " +
                                                        "ORDER BY s.Hash DESC";


        static public int CreateFileEntry(DBEngine dbEngine, FileInfo fileInfo, string MD5Hash)
        {
            string PreparedEntry = "INSERT INTO FileDB (Name, FullName, Hash, Size, FileExt, Directory, LastModTime) " +
                "VALUES (@Name, @FullName, @Hash, @Size, @FileExt, @Directory, @LastModTime);";

            using(SQLiteCommand command = new SQLiteCommand(dbEngine.SqlConnection))
            {
                command.CommandText = PreparedEntry;
                command.Prepare();

                command.Parameters.AddWithValue("@Name", fileInfo.Name);
                command.Parameters.AddWithValue("@FullName", fileInfo.FullName);
                command.Parameters.AddWithValue("@Hash", MD5Hash);
                command.Parameters.AddWithValue("@Size", fileInfo.Length);
                command.Parameters.AddWithValue("@FileExt", fileInfo.Extension);
                command.Parameters.AddWithValue("@Directory", fileInfo.DirectoryName);
                command.Parameters.AddWithValue("@LastModTime", fileInfo.LastWriteTime.Ticks);

                command.ExecuteNonQuery();
            }
            return 0;
        }


        static public int FindDuplicateEntries(DBEngine dbEngine, DataViewModel DataModel)
        {
            using(SQLiteCommand command = new SQLiteCommand(dbEngine.SqlConnection))
            {
                command.CommandText = DuplicateEntriesQuery;

                using(SQLiteDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        FileDetail fileDetail = new FileDetail();
                        fileDetail.Hash = (string)reader["Hash"]; ;
                        fileDetail.FileName = (string)reader["Name"];
                        fileDetail.FullFilePath = (string)reader["FullName"];
                        fileDetail.Size = (Int64)reader["Size"];
                        DataModel.AddFileEntryToDataModel(fileDetail);
                    }
                }
                return 0;
            }
        }


        static public void ExampleInsert(DBEngine dbEngine)
        {
            using(SQLiteCommand cmd = new SQLiteCommand(dbEngine.SqlConnection))
            {
                cmd.CommandText = "DROP TABLE IF EXISTS Cars";
                cmd.ExecuteNonQuery();
                cmd.CommandText = @"CREATE TABLE Cars(Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                    Name TEXT, Price INT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO Cars (Name, Price) VALUES('Audi',52642)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Cars (Name, Price) VALUES('Mercedes',57127)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Cars (Name, Price) VALUES('Skoda',9000)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Cars (Name, Price) VALUES('Volvo',29000)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Cars (Name, Price) VALUES('Bentley',350000)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Cars (Name, Price) VALUES('Citroen',21000)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Cars (Name, Price) VALUES('Hummer',41400)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO Cars (Name, Price) VALUES('Volkswagen',21600)";
                cmd.ExecuteNonQuery();
            }
        }


    }
}
