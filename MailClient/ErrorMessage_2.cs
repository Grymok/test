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
using System.Windows.Shapes;

namespace MailClient
{
   public class ErrorMessage
    {
        public static void errormessage()
        {
            string messageBoxText = "Du skal indtaste et tal...!";
            string caption = "Fejl Error Fehler Kosa";
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBox.Show(messageBoxText, caption, button, icon);
         }
    }
}
