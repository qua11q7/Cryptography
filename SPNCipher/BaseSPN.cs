using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public abstract class BaseSPN : ISPNCipher {
        public int RoundCount { get; }
        public int SubstitutionBoxCount { get; }
        public int BlockLength { get; }

        public ISubstitution Substitution { get; }
        public IPermutation Permutation { get; }
        public IKeyScheduler KeyScheduler { get; }

        public string Name { get; protected set; }

        internal BaseSPN(ISubstitution substitution, IPermutation permutation, IKeyScheduler keyScheduler) {
            Substitution = substitution;
            Permutation = permutation;
            KeyScheduler = keyScheduler;

            RoundCount = 4;
            SubstitutionBoxCount = 4;
            BlockLength = 16;

            if (BlockLength / SubstitutionBoxCount != Substitution.InputLength)
                throw new Exception("ISubstitution is not compatible with SPN.");
        }

        public int Encode(int input) {
            IEnumerable<int> roundKeys = KeyScheduler.GetRoundKeys(RoundCount);
            for (int i = 0; i < RoundCount; i++) {
                // Add Round Key
                input = AddRoundKey(input, roundKeys.ElementAt(i));

                // SBox Substitution
                byte[] inputBytes = GetBytes(input);
                for (int j = 0; j < 4; j++) {
                    inputBytes[j] = Substitution.Substitute(inputBytes[j]);
                }
                input = GetInt(inputBytes);

                // Permutation Layer Except Last Round
                if (i != RoundCount - 1) {
                    input = Permutation.Permutate(input);
                }
            }

            // Add Last Round Key
            input = AddRoundKey(input, roundKeys.Last());
            return input;
        }

        public int Decode(int input) {
            IEnumerable<int> roundKeys = KeyScheduler.GetRoundKeys(RoundCount);

            input = AddRoundKey(input, roundKeys.Last());

            for (int i = roundKeys.Count() - 2; i >= 0; i--) {
                // Permutation Layer Except Last Round
                if (i != RoundCount - 1) {
                    input = Permutation.InversePermutate(input);
                }

                // SBox Substitution
                byte[] inputBytes = GetBytes(input);
                for (int j = 0; j < 4; j++) {
                    inputBytes[j] = Substitution.InverseSubstitute(inputBytes[j]);
                }
                input = GetInt(inputBytes);

                // Add Round Key
                input = AddRoundKey(input, roundKeys.ElementAt(i));
            }

            return input;
        }

        private int AddRoundKey(int input, int roundKey) {
            return (input ^ roundKey) & 65535;
        }

        private byte[] GetBytes(int input) {
            byte first4bit = (byte)((input & 61440) >> 12);
            byte second4bit = (byte)((input & 3840) >> 8);
            byte third4bit = (byte)((input & 240) >> 4);
            byte fourth4bit = (byte)(input & 15);

            return new byte[] { first4bit, second4bit, third4bit, fourth4bit };
        }

        private int GetInt(byte[] bytes) {
            if (bytes.Length != 4)
                throw new Exception();

            return bytes[0] << 12 | bytes[1] << 8 | bytes[2] << 4 | bytes[3];
        }
    }
}
