using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public abstract class BaseSPN {
        protected const int RoundCount = 4;
        protected const int BlockLength = 16;

        private ISubstitution _substitution;
        private IPermutation _permutation;
        private IKeyScheduler _keyScheduler;

        internal BaseSPN(ISubstitution substitution, IPermutation permutation, IKeyScheduler keyScheduler) {
            _substitution = substitution;
            _permutation = permutation;
            _keyScheduler = keyScheduler;
        }

        public int Encode(int input) {
            IEnumerable<int> roundKeys = _keyScheduler.GetRoundKeys(RoundCount);
            for (int i = 0; i < RoundCount; i++) {
                // Add Round Key
                input = AddRoundKey(input, roundKeys.ElementAt(i));

                // SBox Substitution
                byte[] inputBytes = GetBytes(input);
                for (int j = 0; j < 4; j++) {
                    inputBytes[j] = _substitution.Substitute(inputBytes[j]);
                }
                input = GetInt(inputBytes);

                // Permutation Layer Except Last Round
                if (i != RoundCount - 1) {
                    input = _permutation.Permutate(input);
                }
            }

            // Add Last Round Key
            input = AddRoundKey(input, roundKeys.Last());
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
