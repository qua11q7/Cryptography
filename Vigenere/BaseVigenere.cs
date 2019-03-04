using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace VigenereCipher {
    public abstract class BaseVigenere {
        public string Encode(string plainText, string keyword) {
            // Convert text values to corresponding integer values.
            IEnumerable<int> textValues = Utilities.GetValueArray(plainText);
            IEnumerable<int> keywordValues = Utilities.GetValueArray(keyword);
            
            // Obtain cipher values from appling an operation between text values and keyword value.
            // Different ciphers apply different operations.
            // Vigenere: C = (P + K) % 26
            // Beaufort: C = (K - P) % 26
            // Variant:  C = (P - K) % 26
            int[] cipherValues = new int[plainText.Length];
            for (int i = 0; i < plainText.Length; i++) {
                cipherValues[i] = CalculateEncodeValue(textValues.ElementAt(i), keywordValues.ElementAt(i % keyword.Length));
            }
            
            // Convert cipher values to text.
            return Utilities.GetString(cipherValues);
        }

        public string Decode(string cipherText, string keyword) {
            // Convert text values to corresponding integer values.
            IEnumerable<int> textValues = Utilities.GetValueArray(cipherText);
            IEnumerable<int> keywordValues = Utilities.GetValueArray(keyword);

            // Obtain plain text values from appling an operation between cipher values and keyword value.
            // Different ciphers apply different operations.
            // Vigenere: P = (C - K) % 26
            // Beaufort: P = (K - C) % 26
            // Variant:  P = (C + K) % 26
            int[] plainTextValues = new int[cipherText.Length];
            for (int i = 0; i < cipherText.Length; i++) {
                plainTextValues[i] = CalculateDecodeValue(textValues.ElementAt(i), keywordValues.ElementAt(i % keyword.Length));
            }

            // Convert plain text values to text.
            return Utilities.GetString(plainTextValues);
        }

        // These abstract functions are implemented in Vigenere, Beaufort and Variant classes.
        protected abstract int CalculateEncodeValue(int plainTextValue, int keywordValue);
        protected abstract int CalculateDecodeValue(int cipherTextValue, int keywordValue);
    }
}
