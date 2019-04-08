using DifferentialCyrptanalysis;
using SPNCipher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2 {
    class Program {
        const int MASTER_KEY = 31327;
        // K0 : 31327 : 0111 1010 0101 1111
        // K1 : 48431 : 1011 1101 0010 1111
        // K2 : 56983 : 1101 1110 1001 0111
        // K3 : 61259 : 1110 1111 0100 1011
        // K4 : 63397 : 1111 0111 1010 0101
        // K5 : 64466 : 1111 1011 1101 0010

        static void Main(string[] args) {
            ISPNCipher spn1 = new SPNCipher1(MASTER_KEY);
            ISPNCipher spn2 = new SPNCipher2(MASTER_KEY);
            ISPNCipher spn3 = new SPNCipher3(MASTER_KEY);

            AnalyseSPN(spn1);
            AnalyseSPN(spn2);
            AnalyseSPN(spn3);

            Console.ReadKey();
        }

        static void AnalyseSPN(ISPNCipher spn) {
            Console.WriteLine("\tDIFFERENTIAL ANALYSIS OF " + spn.Name.ToUpper());
            DifferentialAnalyser analyser = new DifferentialAnalyser(spn);

            Console.WriteLine("\r\n\tDifference Distribution Table of " + spn.Name);
            Console.WriteLine(analyser.Table);

            DifferentialCharacteristic characteristic = analyser.CreateCharacteristic();
            Console.WriteLine("\r\n\tFound Differential Characteristic: ");
            Console.WriteLine("\t" + characteristic);

            int partialKey = analyser.Analyse();
            Console.WriteLine("\r\n\tFound Partial Key: ");
            WriteInBinary(partialKey);

            Console.WriteLine("\r\n\tDIFFERENTIAL ANALYSIS OF " + spn.Name.ToUpper() + " HAS COMPLETED\r\n\r\n");
        }

        static void WriteInBinary(int number) {
            string binary = Convert.ToString(number, 2);
            if (binary.Length < 16) {
                int zeroCount = 16 - binary.Length;
                for (int i = 0; i < zeroCount; i++) {
                    binary = "0" + binary;
                }
            }

            string spacedBinary = "";
            for (int i = 0; i < binary.Length; i++) {
                if (i != 0 && i % 4 == 0) {
                    spacedBinary += " ";
                }

                spacedBinary += binary[i];
            }

            Console.WriteLine("\t" + spacedBinary);
        }
    }
}
