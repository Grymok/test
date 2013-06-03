using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MailClient.Forms.Openpop
{
    public class ADD_NEW_DATA
    {
        public ADD_NEW_DATA()
        {
            // default constructor
        }

        public ADD_NEW_DATA(string C1, string C2, string C3)
        {
            Column_1 = C1;
            Column_2 = C2;
            Column_3 = C3;
        }

        public string Column_1 { get; set; }
        public string Column_2 { get; set; }
        public string Column_3 { get; set; }
    }
}

