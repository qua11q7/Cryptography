using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public class Permutation1 : IPermutation {
        public Dictionary<int, int> Mapping { get; }

        public Permutation1() {
            Mapping = new Dictionary<int, int>() {
                { 0, 0 },  { 1, 4 },  { 2, 8 },   { 3, 12 },
                { 4, 1 },  { 5, 5 },  { 6, 9 },   { 7, 13 },
                { 8, 2 },  { 9, 6 },  { 10, 10 }, { 11, 14 },
                { 12, 3 }, { 13, 7 }, { 14, 11 }, { 15, 15 }
            };
        }

        public int Permutate(int input) {
            int output = 0;
            for (int i = 0; i < 16; i++) {
                output |= ((input >> i) & 1) << Mapping[i];
            }

            return output;
        }
    }
    public class Permutation2 : IPermutation {
        public Dictionary<int, int> Mapping { get; }

        public Permutation2() {
            Mapping = new Dictionary<int, int>() {
                { 0, 0 },   { 1, 1 },   { 2, 2 },   { 3, 3 },
                { 4, 4 },   { 5, 5 },   { 6, 6 },   { 7, 7 },
                { 8, 8 },   { 9, 9 },   { 10, 10 }, { 11, 11 },
                { 12, 12 }, { 13, 13 }, { 14, 14 }, { 15, 15 }
            };
        }

        public int Permutate(int input) {
            return input;
        }
    }
}
