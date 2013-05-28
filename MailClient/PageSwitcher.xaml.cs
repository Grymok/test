
using MailClient.Forms;
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
   /* Dette er hovede side, men faktisk får du den aldrig at se, da den kun vil blive brugt til at åbne og lukke det andre vinduer inde i denne.
    */
    public partial class PageSwitcher : Window
    {
        public PageSwitcher()
        {//her starter du programmen, og fortæller at inde i dette vindue, vil du åbne et andet vindue, som hedder MyMaiMenu
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch(new MyMainMenu());
        }

        public void Navigate(UserControl nextPage)
        {
            this.Content = nextPage;
        }
        //her laver vi et interface vi tilføje til alle de vindeur som vi ønsker at åbne.
        public void Navigate(UserControl nextPage, object state)
        {
            this.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable! "
                  + nextPage.Name.ToString());
        }
    }
}
