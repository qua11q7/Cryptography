using SPNCipher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DifferentialCyrptanalysis {
    public class PlainTextGenerator {
        private static Random random = new Random();

        public int BlockLength { get; }

        public PlainTextGenerator(int blockLength) {
            BlockLength = blockLength;
        }

        public Dictionary<int, int> GetPlainTexts(int amount, int inputDifference) {
            Dictionary<int, int> plainTexts = new Dictionary<int, int>();
            int maxInput = (int)Math.Pow(2, BlockLength);
            int tries = 0;
            while (plainTexts.Count < amount && tries < 25) {
                int plainText = random.Next(0, maxInput);
                if (!plainTexts.ContainsKey(plainText)) {
                    plainTexts.Add(plainText, plainText ^ inputDifference);
                    tries = 0;
                } else {
                    tries++;
                }
            }

            return plainTexts;
        }
    }
}
