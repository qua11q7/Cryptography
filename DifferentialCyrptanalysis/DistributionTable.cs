using SPNCipher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DifferentialCyrptanalysis {
    public class DistributionTable {
        private readonly ISubstitution _substitution;

        public Dictionary<byte, Dictionary<byte, byte>> Table { get; }

        public Dictionary<byte, byte> this[byte xDifference] => Table[xDifference];
        public byte this[byte xDifference, byte yDifference] => Table[xDifference][yDifference];

        public DistributionTable(ISubstitution substitution) {
            _substitution = substitution;
            Table = new Dictionary<byte, Dictionary<byte, byte>>();

            CreateTable();
        }

        private void CreateTable() {
            Table.Clear();

            byte inputCount = (byte)Math.Pow(2, _substitution.InputLength);
            byte mask = (byte)((1 << _substitution.InputLength) - 1);
            for (byte x = 0; x < inputCount; x++) {
                for (byte diffX = 0; diffX < inputCount; diffX++) {
                    byte secondX = (byte)((x ^ diffX) & mask);
                    byte diffY = (byte)((_substitution.Mapping[x] ^ _substitution.Mapping[secondX]) & mask);

                    if (!Table.ContainsKey(diffX)) {
                        Dictionary<byte, byte> differenceTable = new Dictionary<byte, byte>();
                        for (byte i = 0; i < inputCount; i++) {
                            differenceTable.Add(i, 0);
                        }

                        Table.Add(diffX, differenceTable);
                    }
                    if (!Table[diffX].ContainsKey(diffY)) {
                        Table[diffX].Add(diffY, 0);
                    }

                    Table[diffX][diffY]++;
                }
            }
        }

        public string PrintTable() {
            byte inputCount = (byte)Math.Pow(2, _substitution.InputLength);
            string[] table = new string[inputCount + 2];

            string firstRow = "          ";
            string secondRow = "      __";
            for (int i = 0; i < inputCount; i++) {
                firstRow += i.ToString("X") + (i == inputCount - 1 ? "" : "   ");
                secondRow += "____";
            }
            table[0] = firstRow;
            table[1] = secondRow;

            int index = 2;
            foreach (KeyValuePair<byte, Dictionary<byte, byte>> tableItem in Table) {
                string row = "    " + tableItem.Key.ToString("X") + " |   ";
                foreach (KeyValuePair<byte, byte> difference in tableItem.Value) {
                    row += difference.Value + (difference.Key == inputCount - 1 ? "" : (difference.Value >= 10 ? "  " : "   "));
                }
                table[index++] = row;
            }

            string tableStr = table.Aggregate((a, b) => a + "\r\n" + b);
            return tableStr;
        }

        public override string ToString() {
            return PrintTable();
        }

        public static implicit operator string(DistributionTable dTable) {
            return dTable.PrintTable();
        }
    }
}
