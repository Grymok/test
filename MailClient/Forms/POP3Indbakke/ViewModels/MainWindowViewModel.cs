using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Extra.Mail;
using System.Windows.Input;
using MailClient.Forms.POP3Indbakke.NS.MVVM;
using MailClient.Properties;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Windows.Threading;
using Microsoft.Win32;

namespace MailClient.Forms.POP3Indbakke.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            popClient.QueryPopInfoCompleted += PopClient_QueryPopInfoCompleted;
            popClient.MailPopped += PopClient_MailPopped;
            popClient.MailPopCompleted += PopClient_MailPopCompleted;
            popClient.ChatCommandLog += PopClient_ChatCommandLog;
            popClient.ChatResponseLog += PopClient_ChatResponseLog;

            Application.Current.Exit += Current_Exit;
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            popClient.Dispose();
        }

        private string title = "Pop Client Demo";

        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.FirePropertyChanged("Title");
                }
            }
        }

        private string mailStatsText;

        public string MailStatsText
        {
            get
            {
                return this.mailStatsText;
            }

            set
            {
                if (this.mailStatsText != value)
                {
                    this.mailStatsText = value;
                    this.FirePropertyChanged("MailStatsText");
                }
            }
        }

        private string fetchStatusText;

        public string FetchStatusText
        {
            get
            {
                return this.fetchStatusText;
            }

            set
            {
                if (this.fetchStatusText != value)
                {
                    this.fetchStatusText = value;
                    this.FirePropertyChanged("FetchStatusText");
                }
            }
        }

        private Visibility loginGridVisibility;

        public Visibility LoginGridVisibility
        {
            get
            {
                return this.loginGridVisibility;
            }

            set
            {
                if (this.loginGridVisibility != value)
                {
                    this.loginGridVisibility = value;
                    this.FirePropertyChanged("LoginGridVisibility");
                }
            }
        }

        private double mailGridOpacity = 0.33;

        public double MailGridOpacity
        {
            get
            {
                return this.mailGridOpacity;
            }

            set
            {
                if (this.mailGridOpacity != value)
                {
                    this.mailGridOpacity = value;
                    this.FirePropertyChanged("MailGridOpacity");
                }
            }
        }

        private bool isToolbarAvailable;

        public bool IsToolbarAvailable
        {
            get
            {
                return this.isToolbarAvailable;
            }

            set
            {
                if (this.isToolbarAvailable != value)
                {
                    this.isToolbarAvailable = value;
                    this.FirePropertyChanged("IsToolbarAvailable");
                }
            }
        }

        private string popPassword;

        public string PopPassword
        {
            get
            {
                return this.popPassword;
            }

            set
            {
                if (this.popPassword != value)
                {
                    this.popPassword = value;
                    this.FirePropertyChanged("PopPassword");
                }
            }
        }

        private PopClient popClient = new PopClient();

        private ICommand fetchMailCommand;

        public ICommand FetchMailCommand
        {
            get
            {
                return fetchMailCommand ?? (fetchMailCommand = new DelegateCommand(FetchMail));
            }
        }

        private Dispatcher mainDispatcher;

        public void FetchMail()
        {
            Settings.Default.Save();
            this.LoginGridVisibility = Visibility.Collapsed;
            this.MailGridOpacity = 1.0;
            this.IsToolbarAvailable = true;
            this.Refresh();
        }

        private ICommand exitCommand;

        public ICommand ExitCommand
        {
            get
            {
                return exitCommand ?? (exitCommand = new DelegateCommand(Exit));
            }
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        private ICommand cancelCommand;

        public ICommand CancelCommand
        {
            get
            {
                return cancelCommand ?? (cancelCommand = new DelegateCommand(Cancel, CanCancel));
            }
        }

        public bool CanCancel()
        {
            return popClient.IsWorking();
        }

        public void Cancel()
        {
            popClient.Cancel();
        }

        private ICommand refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                return refreshCommand ?? (refreshCommand = new DelegateCommand(Refresh, CanRefresh));
            }
        }

        public bool CanRefresh()
        {
            return !popClient.IsWorking();
        }

        public void Refresh()
        {
            popClient.Host = Settings.Default.Host;
            popClient.Port = Settings.Default.Port;
            popClient.Username = Settings.Default.Username;
            popClient.Password = this.PopPassword;
            popClient.EnableSsl = Settings.Default.EnableSsl;
            popClient.DeleteMailAfterPop = false; // For demo safety!
            popClient.Timeout = Settings.Default.Timeout;

            try
            {
                mainDispatcher = Dispatcher.CurrentDispatcher;
                FetchStatusText = "Fetching...";
                mails.Clear();
                logs.Clear();
                popClient.PopMail();
            }
            catch (InvalidOperationException ex)
            {
                FetchStatusText = String.Format("Connection error - {0}", ex.Message);
            }
            catch (PopClientException ex)
            {
                FetchStatusText = String.Format("POP3 error - {0}", ex.Message);
            }
        }

        public ICommand saveFileCommand;

        public ICommand SaveFileCommand
        {
            get
            {
                return saveFileCommand ?? (saveFileCommand = new DelegateCommand<Attachment>(SaveFile));
            }
        }

        public void SaveFile(Attachment attachment)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                FileName = attachment.Name
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                attachment.Save(dialog.FileName);
            }
        }

        private ICommand showChatLogCommand;

        public ICommand ShowChatLogCommand
        {
            get
            {
                return showChatLogCommand ?? (showChatLogCommand = new DelegateCommand(ShowChatLog, CanShowChatLog));
            }
        }

        private LogWindow logWindow;

        public bool CanShowChatLog()
        {
            return logWindow == null;
        }

        public void ShowChatLog()
        {
            logWindow = new LogWindow() { DataContext = logs, Owner = Application.Current.MainWindow };
            logWindow.Show();
            logWindow.Closed += (s, e) => logWindow = null;
        }

        private ObservableCollection<MailPoppedEventArgs> mails = new ObservableCollection<MailPoppedEventArgs>();

        public ObservableCollection<MailPoppedEventArgs> Mails
        {
            get
            {
                return mails;
            }
        }

        class LogInfo
        {
            public string Line { get; set; }

            public bool Response { get; set; }
        }

        private ObservableCollection<LogInfo> logs = new ObservableCollection<LogInfo>();

        void PopClient_ChatResponseLog(object sender, PopClientLogEventArgs e)
        {
            mainDispatcher.Invoke((Action)(() => logs.Add(new LogInfo() { Line = e.Line, Response = true })), null);
        }

        void PopClient_ChatCommandLog(object sender, PopClientLogEventArgs e)
        {
            mainDispatcher.Invoke((Action)(() => logs.Add(new LogInfo() { Line = e.Line })), null);
        }

        void PopClient_MailPopCompleted(object sender, MailPopCompletedEventArgs e)
        {
            if (e.Aborted)
            {
                PopClientException popex = e.Exception as PopClientException;
                if (popex == null)
                {
                    FetchStatusText = "Aborted!";
                }
                else
                {
                    FetchStatusText = popex.PopClientUserCancelled ? "User cancelled!" : String.Format("POP3 error - {0}", popex.Message);
                }
            }
            else
            {
                FetchStatusText = "Done!";
            }

            CommandManager.InvalidateRequerySuggested();
        }

        void PopClient_MailPopped(object sender, MailPoppedEventArgs e)
        {
            mainDispatcher.Invoke((Action)(() => mails.Add(e)), null);
        }

        void PopClient_QueryPopInfoCompleted(object sender, MailPopInfoFetchedEventArgs e)
        {
            MailStatsText = String.Format("{0} mails, Size = {1}", e.Count, e.Size);
        }
    }
}



