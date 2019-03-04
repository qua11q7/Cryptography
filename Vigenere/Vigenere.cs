using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VigenereCipher {
    public class Vigenere : BaseVigenere {
        protected override int CalculateEncodeValue(int plainTextValue, int keywordValue) {
            return (plainTextValue + keywordValue) % 26;
        }

        protected override int CalculateDecodeValue(int cipherTextValue, int keywordValue) {
            int value = cipherTextValue - keywordValue;
            while (value < 0)
                value += 26;

            return value % 26;
        }
    }
}
