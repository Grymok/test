using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MailClient.Forms.Openpop
{
    class SqlDbHandler
    {
        public SQLiteConnection sqliteCon;

        public void CreateDB()
        {
            string dbConnString = @"Data Source=db.sqlite; Version=3;"; // Creating the connection string with filepath to the DB
            sqliteCon = new SQLiteConnection(dbConnString); // Creating new connection string instance.
            try
            {

                sqliteCon.Open(); // Open database


                // Defining the SQL Create table string 
                string crtMessagetblSQL = "CREATE TABLE IF NOT EXISTS [Messages] (" +
                    "[msgID] TEXT NOT NULL PRIMARY KEY," +
                    "[msgSender] TEXT NULL," +
                    "[msgSubject] TEXT NULL," +
                    "[msgBody] TEXT NULL" +
                    ")";

                using (SQLiteTransaction sqlTrans = sqliteCon.BeginTransaction())
                {
                    SQLiteCommand crtComm = new SQLiteCommand(crtMessagetblSQL, sqliteCon);

                    crtComm.ExecuteNonQuery();

                    //   crtComm.Dispose();

                    sqlTrans.Commit(); // Commit changes into the DB
                } // End using

                sqliteCon.Close(); // Closes DB connection

            } // End try

            catch (SQLiteException e)
            {
                //MessageBox.Show(e.ToString());
            } // End catch

        }
    }
}
