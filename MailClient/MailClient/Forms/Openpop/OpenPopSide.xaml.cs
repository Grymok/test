using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using OpenPop.Common.Logging;
using OpenPop.Mime;
using OpenPop.Mime.Decode;
using OpenPop.Mime.Header;
using OpenPop.Pop3;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;

namespace MailClient.Forms.Openpop
{
    /// <summary>
    /// Interaction logic for OpenPopSide.xaml
    /// </summary>
    public partial class OpenPopSide : UserControl
    {
        SqlDbHandler SqlDbHandler = new SqlDbHandler();
        SQLiteConnection sqliteCon;
       
       List<OpenPop.Mime.Message> list;
       
        private BackgroundWorker worker;
        
        public OpenPopSide()
        
        {
            
            InitializeComponent();
            sqliteCon = SqlDbHandler.sqliteCon;
            SqlDbHandler.CreateDB();
            updatDB();      
                          
            list = new List<OpenPop.Mime.Message>();


            worker = new BackgroundWorker();

            worker.WorkerReportsProgress = true;

            worker.DoWork += new DoWorkEventHandler(fetchAllMessages);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

            private static void fetchAllMessages(object sender, DoWorkEventArgs e)
        {
            int percentComplete;

            
            using (Pop3Client client = new Pop3Client())
            {
               
                client.Connect("pop.gmail.com", 995, true);

               
                client.Authenticate("programmering3@gmail.com", "programmering");

                
                int messageCount = client.GetMessageCount();

               
                List<OpenPop.Mime.Message> allMessages = new List<OpenPop.Mime.Message>(messageCount);

                
                for (int i = messageCount; i > 0; i--)
                {
                    allMessages.Add(client.GetMessage(i));
                    percentComplete = Convert.ToInt16((Convert.ToDouble(allMessages.Count) / Convert.ToDouble(messageCount)) * 100);
                    (sender as BackgroundWorker).ReportProgress(percentComplete);
                }

                e.Result = allMessages;
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgBarMailFetched.Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            list = (List<OpenPop.Mime.Message>)e.Result;
            try
            {
                SQLiteConnection sqliteCon = new SQLiteConnection(@"Data Source = messages.db;");
                SQLiteTransaction sqlTrans;
                sqliteCon.Open();



                SQLiteCommand commandInsert = new SQLiteCommand("INSERT OR IGNORE INTO messages (msgID, msgSender, msgSubject, msgBody) VALUES ( @msgID, @msgSender, @msgSubject, @msgBody)", sqliteCon);
               // SQLiteCommand commandCount = new SQLiteCommand("SELECT COUNT(*) FROM messages", sqliteConn);

                /*

                string crtMessagetblSQL = "CREATE TABLE IF NOT EXISTS [Messages] (" +
                            "[msgID] TEXT NOT NULL PRIMARY KEY," +
                            "[msgSender] TEXT NULL," +
                            "[msgSubject] TEXT NULL," +
                            "[msgBody] TEXT NULL)";
                           // "[msgIndex] INTEGER )";
                          
                using (SQLiteTransaction sqlTrans1 = sqliteCon.BeginTransaction())
                {
                    SQLiteCommand crtComm = new SQLiteCommand(crtMessagetblSQL, sqliteCon);

                    crtComm.ExecuteNonQuery();
                    crtComm.Dispose();

                    sqlTrans1.Commit(); // Commit changes into the DB
                } 
                */
                msgcounglb.Text = Convert.ToString(list.Count);

              //  Subjectlsbx.Items.Clear();
                //int i = Convert.ToInt32(commandCount.ExecuteScalar());
                foreach (OpenPop.Mime.Message message in list)
                {
                    if (message.Headers.MessageId != null)
                    {
                        commandInsert.Parameters.AddWithValue("@msgID", message.Headers.MessageId);
                        commandInsert.Parameters.AddWithValue("@msgSender", message.Headers.From.Address);
                        commandInsert.Parameters.AddWithValue("@msgSubject", message.Headers.Subject);
                        if (!message.MessagePart.IsMultiPart)
                        {
                            commandInsert.Parameters.AddWithValue("@msgBody", message.MessagePart.GetBodyAsText());
                        }
                        else
                        {
                            OpenPop.Mime.MessagePart plainText = message.FindFirstPlainTextVersion();
                            commandInsert.Parameters.AddWithValue("@msgBody", plainText.GetBodyAsText());
                        }

                       // SQLiteCommand cmdFindIndex = new SQLiteCommand("SELECT MAX(msgIndex) from messages", sqliteCon);
                       // object anObj = cmdFindIndex.ExecuteScalar();

                        //int maxIndex = (anObj == null ? -1 : Convert.ToInt32(anObj.ToString()));
                       
                       // maxIndex++;
                       // commandInsert.Parameters.AddWithValue("@msgIndex", maxIndex);
                        
                        sqlTrans = sqliteCon.BeginTransaction();
                       // SQLiteCommand crtComm = new SQLiteCommand(crtMessagetblSQL, sqliteCon);
                        int result = commandInsert.ExecuteNonQuery();
                        sqlTrans.Commit();
                        updatDB();

                       // Subjectlsbx.Items.Add(message.Headers.Subject.ToString());
                    }
                    
                }
                
            }
            catch (SQLiteException r)
            {
                MessageBox.Show(r.ToString());
            } // End catch





        }

            
        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MyMainMenu());
        }

        private void ReciveMailbtn_Click(object sender, RoutedEventArgs e)
        {
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }

        }

        private void Subjectname_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            SQLiteConnection sqliteCon = new SQLiteConnection(@"Data Source = messages.db;");
           
            sqliteCon.Open();
            List<Msg> dbmsg = new List<Msg>();
            SQLiteCommand cmdRead = new SQLiteCommand("SELECT * FROM Messages", sqliteCon);
            SQLiteDataReader reader = cmdRead.ExecuteReader();
            //try
            //{
            while (reader.Read())
            {
                string msgID = reader.GetString(0);
                string msgSender = reader.GetString(1);
                string msgSubject = reader.GetString(2);
                string msgBody = reader.GetString(3);
                dbmsg.Add(new Msg() { MsgID = msgID, MsgSender = msgSender, MsgSubject = msgSubject, MsgBody = msgBody });
            }
           
            int selectedItem = Subjectname.SelectedIndex;
            msgBodytbx.Text = dbmsg[selectedItem].MsgBody;

        }

        public void updatDB()
        {
            Subjectname.Items.Clear();
            try
            {

                SQLiteConnection sqliteCon = new SQLiteConnection(@"Data Source = messages.db;");

                sqliteCon.Open();
                List<Msg> dbmsg = new List<Msg>();
                SQLiteCommand cmdRead = new SQLiteCommand("SELECT * FROM Messages ", sqliteCon);
                SQLiteDataReader reader = cmdRead.ExecuteReader();
                //try
                //{
                while (reader.Read())
                {
                    string msgID = reader.GetString(0);
                    string msgSender = reader.GetString(1);
                    string msgSubject = reader.GetString(2);
                    string msgBody = reader.GetString(3);
                    dbmsg.Add(new Msg() { MsgID = msgID, MsgSender = msgSender, MsgSubject = msgSubject, MsgBody = msgBody });
                }



                foreach (Msg msg in dbmsg)
                {
                    ADD_NEW_DATA data = new ADD_NEW_DATA(msg.MsgSubject, msg.MsgID, "Data3");
                    Subjectname.Items.Add(data);
                    //Subjectname.Items.Add(msg.MsgSubject);

                    // sercetBox.Items.Add(msg.MsgID);
                }
                sqliteCon.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception");
                Debug.WriteLine(ex.Message);
            }

        }


       /* private void Subjectlsbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            {
              
                
              // msgBodytbx.Text = 
               // msgBodytbx.Text =  
                /*
                if (!list[selectedItem].MessagePart.IsMultiPart)
                {
                    msgBodytbx.Text = list[selectedItem].MessagePart.GetBodyAsText();
                }
                else
                {
                    OpenPop.Mime.MessagePart plainText = list[selectedItem].FindFirstPlainTextVersion();
                    msgBodytbx.Text = plainText.GetBodyAsText();
                }
                  
            }
    
        }
*/
        


       

        

        

        
       }
    
}

