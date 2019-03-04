using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VigenereCipher {
    public class Variant : BaseVigenere {
        protected override int CalculateEncodeValue(int plainTextValue, int keywordValue) {
            int value = plainTextValue - keywordValue;
            while (value < 0)
                value += 26;

            return value % 26;
        }

        protected override int CalculateDecodeValue(int cipherTextValue, int keywordValue) {
            return (cipherTextValue + keywordValue) % 26;
        }
    }
}
