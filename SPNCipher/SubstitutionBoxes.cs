using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public class SubstitutionBox1 : ISubstitution {
        public Dictionary<byte, byte> Mapping { get; }

        public SubstitutionBox1() {
            Mapping = new Dictionary<byte, byte>() {
                { 0, 1 },  { 1, 9 },   { 2, 6 },   { 3, 13 },
                { 4, 7 },  { 5, 3 },   { 6, 5 },   { 7, 15 },
                { 8, 2 },  { 9, 12 },  { 10, 14 }, { 11, 10 },
                { 12, 4 }, { 13, 11 }, { 14, 8 },  { 15, 0 }
            };
        }

        public byte Substitute(byte input) {
            return Mapping[input];
        }
    }
    public class SubstitutionBox2 : ISubstitution {
        public Dictionary<byte, byte> Mapping { get; }

        public SubstitutionBox2() {
            Mapping = new Dictionary<byte, byte>() {
                { 0, 0 },   { 1, 1 },  { 2, 9 },   { 3, 6 },
                { 4, 13 },  { 5, 7 },  { 6, 3 },   { 7, 5 },
                { 8, 15 },  { 9, 2 },  { 10, 12 }, { 11, 14 },
                { 12, 10 }, { 13, 4 }, { 14, 11 }, { 15, 8 }
            };
        }

        public byte Substitute(byte input) {
            return Mapping[input];
        }
    }
}
