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
using Extra.Mail;
using MailClient.Forms.POP3Indbakke.NS.MVVM;
using MailClient.Properties;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Windows.Threading;
using Microsoft.Win32;
using MailClient.Forms.POP3Indbakke.ViewModels;



namespace MailClient.Forms.SendMail
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Send_Mail : UserControl
    {
        public Send_Mail()
        {
            InitializeComponent();
        }

      

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Settings.Default.userpassword=PasswordBoxPOP1.Password ;
            LoginGrid.Visibility = Visibility.Collapsed;
            {

                
            }



        }

        private void sendmail_button_Click(object sender, RoutedEventArgs e)
        {
             try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient (Settings.Default.SmtpHost);

                    mail.From = new MailAddress(Settings.Default.Username);
                    mail.To.Add(sendto_textbox.Text);
                    mail.Subject = subject_textbox.Text;
                    mail.Body = message_textbox.Text;
                    
                    SmtpServer.Port = Settings.Default.SmtpPort;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(Settings.Default.Username, Settings.Default.userpassword);
                    SmtpServer.EnableSsl = Settings.Default.EnableSsl;

                    SmtpServer.Send(mail);
                    MessageBox.Show("mail Send");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MyMainMenu());
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MyMainMenu());
        }

        

        

        
    }
}
