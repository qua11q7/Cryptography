using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public abstract class BaseSubstitutionBox : ISubstitution {
        public byte InputLength { get; }
        public Dictionary<byte, byte> Mapping { get; protected set; }
        public Dictionary<byte, byte> InverseMapping { get; private set; }

        public BaseSubstitutionBox(byte inputLength) {
            InputLength = inputLength;

            InitializeMapping();
            ReverseMapping();
        }

        public byte Substitute(byte input) {
            return Mapping[input];
        }

        public byte InverseSubstitute(byte input) {
            return InverseMapping[input];
        }

        private void ReverseMapping() {
            InverseMapping = new Dictionary<byte, byte>();
            foreach (KeyValuePair<byte, byte> pair in Mapping) {
                InverseMapping.Add(pair.Value, pair.Key);
            }
        }

        protected abstract void InitializeMapping();
    }

    public class SubstitutionBox1 : BaseSubstitutionBox {
        public SubstitutionBox1() : base(4) { }

        protected override void InitializeMapping() {
            Mapping = new Dictionary<byte, byte>() {
                { 0, 1 },  { 1, 9 },   { 2, 6 },   { 3, 13 },
                { 4, 7 },  { 5, 3 },   { 6, 5 },   { 7, 15 },
                { 8, 2 },  { 9, 12 },  { 10, 14 }, { 11, 10 },
                { 12, 4 }, { 13, 11 }, { 14, 8 },  { 15, 0 }
            };
        }
    }

    public class SubstitutionBox2 : BaseSubstitutionBox {
        public SubstitutionBox2() : base(4) { }

        protected override void InitializeMapping() {
            Mapping = new Dictionary<byte, byte>() {
                { 0, 0 },   { 1, 1 },  { 2, 9 },   { 3, 6 },
                { 4, 13 },  { 5, 7 },  { 6, 3 },   { 7, 5 },
                { 8, 15 },  { 9, 2 },  { 10, 12 }, { 11, 14 },
                { 12, 10 }, { 13, 4 }, { 14, 11 }, { 15, 8 }
            };
        }
    }

    public class SubstitutionBox3 : BaseSubstitutionBox {
        public SubstitutionBox3() : base(4) { }

        protected override void InitializeMapping() {
            Mapping = new Dictionary<byte, byte>() {
                { 0, 14 }, { 1, 4 },  { 2, 13 }, { 3, 1 },
                { 4, 2 },  { 5, 15 }, { 6, 11 }, { 7, 8 },
                { 8, 3 },  { 9, 10 }, { 10, 6 }, { 11, 12 },
                { 12, 5 }, { 13, 9 }, { 14, 0 }, { 15, 7 }
            };
        }
    }
}
