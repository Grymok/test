using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MailClient
{
    public class IsPrim
    {
         public static bool isPrime(int Number)
        {
            if (Number <= 1)
                return false;
            if (Number == 2)
                return true;
            if (Number % 2 == 0)
                return false;
            double sRoot = Math.Sqrt(Number * 1.0);
            for (int i = 3; i <= sRoot; i += 2)
            {
                if (Number % i == 0)
                    return false;
            }
            return true;
        }
    }
}
