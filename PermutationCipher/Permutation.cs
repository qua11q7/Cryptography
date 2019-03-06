using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermutationCipher {
    public class Permutation {
        public string Encode(string plainText, string keyword) {
            // Convert keyword string to its corresponding integer values.
            IEnumerable<int> keywordValues = Utilities.GetValueArray(keyword);
            
            // Encode plain text with keyword values.
            return Encode(plainText, keywordValues);
        }

        public string Encode(string plainText, IEnumerable<int> keywordValues) {
            // Convert keyword values to a valid permutation. 
            Dictionary<int, int> permutation = GetPermutation(keywordValues);

            // Apply permutation to plain text.
            return GetPermutatedResult(plainText, permutation);
        }

        public string Decode(string cipherText, string keyword) {
            // Convert keyword string to its corresponding integer values.
            IEnumerable<int> keywordValues = Utilities.GetValueArray(keyword);

            // Decode plain text with keyword values.
            return Decode(cipherText, keywordValues);
        }

        public string Decode(string cipherText, IEnumerable<int> keywordValues) {
            // Convert keyword values to a valid permutation. For decoding, permutation values must be reversed.
            Dictionary<int, int> permutation = ReversePermutation(GetPermutation(keywordValues));

            // Apply permutation to cipher text.
            return GetPermutatedResult(cipherText, permutation);
        }

        private string GetPermutatedResult(string text, Dictionary<int, int> permutation) {
            // Applies permutation to given text.
            StringBuilder builder = new StringBuilder();

            int keywordLength = permutation.Count();
            // Iterate through each keyword length characters.
            for (int i = 0; i < text.Length; i += keywordLength) {
                char[] permutatedText = new char[keywordLength];
                for (int j = 0; j < keywordLength; j++) {
                    // If we surpass the length of the original text, the permutation process is completed.
                    if (i + j >= text.Length) {
                        // if there are empty spaces in the permutatedText, remove them.
                        permutatedText = permutatedText.Where(c => c != 0).ToArray();
                        break;
                    }

                    // Put (i+j)th indexed character in the text to corresponding index in permutatedText.
                    permutatedText[permutation[j]] = text[i + j];
                }

                // Create a string lateral from permutated characters.
                builder.Append(new string(permutatedText));
            }

            return builder.ToString();
        }

        private Dictionary<int, int> GetPermutation(IEnumerable<int> keywordValues) {
            // Convert given keyword values to 0-indexed permutation dictionary:
            // Ex: keywordValues = { 2, 8, 15, 7, 4, 17 } => {{0, 0}, {1, 3}, {2, 4}, {3, 2}, {4, 1}, {5, 5}}

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
            // Reverses given permutation dictionary:
            // Ex: permutation: {{0, 0}, {1, 3}, {2, 4}, {3, 2}, {4, 1}, {5, 5}} => {{0, 0}, {1, 4}, {2, 3}, {3, 1}, {4, 2}, {5, 5}}
            Dictionary<int, int> reversedPermutation = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> p in permutation) {
                reversedPermutation.Add(p.Value, p.Key);
            }

            return reversedPermutation;
        }
    }
}
