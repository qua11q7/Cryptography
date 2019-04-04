using SPNCipher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2 {
    class Program {
        const int MASTER_KEY = 31327;   // 0111 1010 0101 1111
        static void Main(string[] args) {
            BaseSPN spn1 = new SPNCipher1(MASTER_KEY);
            BaseSPN spn2 = new SPNCipher2(MASTER_KEY);
            BaseSPN spn3 = new SPNCipher3(MASTER_KEY);

            int encoded1 = spn1.Encode(40042);
            int encoded2 = spn2.Encode(40042);
            int encoded3 = spn3.Encode(40042);

            Console.ReadKey();
        }
    }
}
