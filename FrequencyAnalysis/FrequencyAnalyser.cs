using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrequencyAnalysis {
    public static class FrequencyAnalyser {
        public static int FindShiftAmount(string text) {
            text = text.ToUpper();

            // Store each characters occurences in the text.
            Dictionary<char, int> occurences = new Dictionary<char, int> {
                { 'A', 0 }, { 'B', 0 }, { 'C', 0 }, { 'D', 0 }, { 'E', 0 },
                { 'F', 0 }, { 'G', 0 }, { 'H', 0 }, { 'I', 0 }, { 'J', 0 },
                { 'K', 0 }, { 'L', 0 }, { 'M', 0 }, { 'N', 0 }, { 'O', 0 },
                { 'P', 0 }, { 'Q', 0 }, { 'R', 0 }, { 'S', 0 }, { 'T', 0 },
                { 'U', 0 }, { 'V', 0 }, { 'W', 0 }, { 'X', 0 }, { 'Y', 0 }, { 'Z', 0 }
            };

            // Count the characters occured in the text.
            foreach (char character in text) {
                if (!occurences.ContainsKey(character))
                    continue;

                occurences[character]++;
            }

            // Calculate the probabilities of each character in the given text.
            Dictionary<char, double> currentProbabilities = new Dictionary<char, double>();
            Dictionary<char, double> originalProbabilities = new Dictionary<char, double>();
            foreach (KeyValuePair<char, int> occurence in occurences) {
                currentProbabilities[occurence.Key] = (double)occurence.Value / text.Length;
                originalProbabilities[occurence.Key] = Utilities.GetProbability(occurence.Key);
            }

            // Find out how many shift must be made to match the original probabilities.
            int currentShift = 0;
            Dictionary<int, double> shiftResults = new Dictionary<int, double>();
            while (currentShift < 26) {
                // Calculate average error for each shift.
                double error = 0;
                foreach (KeyValuePair<char, double> originalProbability in originalProbabilities) {
                    int valueOfChar = Utilities.GetValueOfChar(originalProbability.Key);
                    char shiftedCharacter = Utilities.GetCharFromValue((valueOfChar + currentShift) % 26);

                    error += Math.Abs(originalProbability.Value - currentProbabilities[shiftedCharacter]);
                }

                // Take average of the calculated error.
                error /= 26;
                
                // If error is small enough, store it.
                if (error < 0.01)
                    shiftResults.Add(currentShift, error);

                currentShift++;
            }

            // If no suitable shift amount is found, return -1 to signify it.
            if (shiftResults.Count == 0) {
                return -1;
            } else {
                // return the shift that has the lowest error.
                shiftResults = shiftResults.OrderBy(s => s.Value).ToDictionary(k => k.Key, v => v.Value);
                return shiftResults.First().Key;
            }
        }
    }
}
