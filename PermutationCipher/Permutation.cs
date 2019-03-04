using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermutationCipher {
    public class Permutation {
        public string Encode(string plainText, string keyword) {
            IEnumerable<int> keywordValues = Utilities.GetValueArray(keyword);
            
            return Encode(plainText, keywordValues);
        }

        public string Encode(string plainText, IEnumerable<int> keywordValues) {
            Dictionary<int, int> permutation = GetPermutation(keywordValues);

            return GetPermutatedResult(plainText, permutation);
        }

        public string Decode(string cipherText, string keyword) {
            IEnumerable<int> keywordValues = Utilities.GetValueArray(keyword);

            return Decode(cipherText, keywordValues);
        }

        public string Decode(string cipherText, IEnumerable<int> keywordValues) {
            Dictionary<int, int> permutation = ReversePermutation(GetPermutation(keywordValues));

            return GetPermutatedResult(cipherText, permutation);
        }

        private string GetPermutatedResult(string text, Dictionary<int, int> permutation) {
            StringBuilder builder = new StringBuilder();

            int keywordLength = permutation.Count();
            for (int i = 0; i < text.Length; i += keywordLength) {
                char[] permutatedText = new char[keywordLength];
                for (int j = 0; j < keywordLength; j++) {
                    if (i + j >= text.Length) {
                        permutatedText = permutatedText.Where(c => c != 0).ToArray();
                        break;
                    }

                    permutatedText[permutation[j]] = text[i + j];
                }

                builder.Append(new string(permutatedText));
            }

            return builder.ToString();
        }

        private Dictionary<int, int> GetPermutation(IEnumerable<int> keywordValues) {
            Dictionary<int, int> permutation = new Dictionary<int, int>();
            int keywordLength = keywordValues.Count();
            for (int i = 0; i < keywordLength; i++) {
                permutation.Add(i, keywordValues.ElementAt(i));
            }

            permutation = permutation.OrderBy(p => p.Value).ToDictionary(k => k.Key, v => v.Value);
            List<int> keys = permutation.Keys.ToList();
            int lastIndex = 0;
            foreach (int key in keys) {
                permutation[key] = lastIndex++;
            }

            return permutation;
        }

        private Dictionary<int, int> ReversePermutation(Dictionary<int, int> permutation) {
            Dictionary<int, int> reversedPermutation = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> p in permutation) {
                reversedPermutation.Add(p.Value, p.Key);
            }

            return reversedPermutation;
        }
    }
}
