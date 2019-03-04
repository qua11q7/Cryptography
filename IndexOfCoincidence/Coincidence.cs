using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexOfCoincidence {
    public static class Coincidence {
        public static double CalculateIndex(string text) {
            text = text.ToUpper();

            Dictionary<char, int> occurences = new Dictionary<char, int> {
                { 'A', 0 }, { 'B', 0 }, { 'C', 0 }, { 'D', 0 }, { 'E', 0 },
                { 'F', 0 }, { 'G', 0 }, { 'H', 0 }, { 'I', 0 }, { 'J', 0 },
                { 'K', 0 }, { 'L', 0 }, { 'M', 0 }, { 'N', 0 }, { 'O', 0 },
                { 'P', 0 }, { 'Q', 0 }, { 'R', 0 }, { 'S', 0 }, { 'T', 0 },
                { 'U', 0 }, { 'V', 0 }, { 'W', 0 }, { 'X', 0 }, { 'Y', 0 }, { 'Z', 0 }
            };
            foreach (char character in text) {
                if (!occurences.ContainsKey(character))
                    occurences[character] = 0;

                occurences[character]++;
            }

            double index = 0;
            int totalLength = occurences.Sum(o => o.Value);
            foreach (KeyValuePair<char, int> occurence in occurences) {
                double prob = (double)occurence.Value / totalLength;
                index += prob * prob;
            }

            return index;
        }
    }
}
