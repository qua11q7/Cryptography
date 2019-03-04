using Helpers;
using IndexOfCoincidence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VigenereCipher.Analysis {
    public static class VigenereAnalyser {
        public static int FindBlockSizeWithCoincidence(string cipherText, int maximumBlockSize = Constants.MAX_BLOCK_SIZE) {
            // Try different block sizes starting from 2 to maximumBlockSize.
            int currentBlockSize = 2;
            // Store average error calculated between the found coincidence and English language's coincidence in a dictionary.
            Dictionary<int, double> errorsPerBlockSize = new Dictionary<int, double>();
            while (currentBlockSize <= maximumBlockSize) {
                double error = 0;
                // Divide cipher text according to the block size.
                IEnumerable<string> dividedCipherTexts = DivideCipherText(cipherText, currentBlockSize);

                // For each divided cipher text, calculate coincidence and obtain error.
                for (int i = 0; i < currentBlockSize; i++) {
                    double coincidence = Coincidence.CalculateIndex(dividedCipherTexts.ElementAt(i));
                    error += Math.Abs(coincidence - Constants.EnglishIndexOfCoincidence);

                    // Check after every 5 calculation of index of coincidence on a divided cipher text to see whether 
                    // current block size is yielding good results. If error is too large, try next block size.
                    if (i % 5 == 0) {
                        double currentError = error / currentBlockSize;
                        if (currentError > 0.01) {
                            break;
                        }
                    }
                }

                // Take average of the calculated error sum.
                error /= currentBlockSize;

                // If error is less than %0.5 (if average coincidence is between 0.060 and 0.070), store as a candidate block size.
                if (error < 0.005) {
                    errorsPerBlockSize.Add(currentBlockSize, error);
                }

                // Increment the block size and try again.
                currentBlockSize++;
            }
            
            if (errorsPerBlockSize.Count == 0) {
                // If no suitable block size has been found, return -1 to signify the analysis has failed.
                return -1;
            } else {
                // Get the block size that resulted in the lowest error and return it.
                errorsPerBlockSize = errorsPerBlockSize.OrderBy(k => k.Value).ToDictionary(k => k.Key, v => v.Value);
                return errorsPerBlockSize.First().Key;
            }
        }

        // Divide given text to blockSize many text.
        public static List<string> DivideCipherText(string cipherText, int blockSize) {
            List<string> dividedCipherTexts = new List<string>();
            StringBuilder builder = new StringBuilder();
            for (int startIndex = 0; startIndex < blockSize; startIndex++) {
                builder.Clear();
                for (int i = startIndex; i < cipherText.Length; i += blockSize) {
                    builder.Append(cipherText[i]);
                }

                dividedCipherTexts.Add(builder.ToString());
            }

            return dividedCipherTexts;
        }
    }
}
