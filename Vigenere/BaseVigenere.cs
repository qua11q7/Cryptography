using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace VigenereCipher {
    public abstract class BaseVigenere {
        public string Encode(string plainText, string keyword) {
            IEnumerable<int> textValues = Utilities.GetValueArray(plainText);
            IEnumerable<int> keywordValues = Utilities.GetValueArray(keyword);
            
            int[] cipherValues = new int[plainText.Length];
            for (int i = 0; i < plainText.Length; i++) {
                cipherValues[i] = CalculateEncodeValue(textValues.ElementAt(i), keywordValues.ElementAt(i % keyword.Length));
            }
            
            return Utilities.GetString(cipherValues);
        }

        public string Decode(string cipherText, string keyword) {
            IEnumerable<int> textValues = Utilities.GetValueArray(cipherText);
            IEnumerable<int> keywordValues = Utilities.GetValueArray(keyword);

            int[] plainTextValues = new int[cipherText.Length];

            for (int i = 0; i < cipherText.Length; i++) {
                plainTextValues[i] = CalculateDecodeValue(textValues.ElementAt(i), keywordValues.ElementAt(i % keyword.Length));
            }

            return Utilities.GetString(plainTextValues);
        }

        protected abstract int CalculateEncodeValue(int plainTextValue, int keywordValue);
        protected abstract int CalculateDecodeValue(int cipherTextValue, int keywordValue);
    }
}
