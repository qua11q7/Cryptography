using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
    public static class Utilities {
        public static Random rnd = new Random();

        public static Dictionary<char, int> ValueOfChars = new Dictionary<char, int>() {
            { 'A', 0 },  { 'B', 1 },  { 'C', 2 },  { 'D', 3 },  { 'E', 4 },
            { 'F', 5 },  { 'G', 6 },  { 'H', 7 },  { 'I', 8 },  { 'İ', 8 }, { 'J', 9 },
            { 'K', 10 }, { 'L', 11 }, { 'M', 12 }, { 'N', 13 }, { 'O', 14 },
            { 'P', 15 }, { 'Q', 16 }, { 'R', 17 }, { 'S', 18 }, { 'T', 19 },
            { 'U', 20 }, { 'V', 21 }, { 'W', 22 }, { 'X', 23 }, { 'Y', 24 }, { 'Z', 25 }
        };

        public static Dictionary<int, char> CharOfValues = new Dictionary<int, char>() {
            { 0 , 'A' }, { 1 , 'B' }, { 2 , 'C' }, { 3 , 'D' }, { 4, 'E' },
            { 5 , 'F' }, { 6 , 'G' }, { 7 , 'H' }, { 8 , 'I' }, { 9, 'J' },
            { 10, 'K' }, { 11, 'L' }, { 12, 'M' }, { 13, 'N' }, { 14, 'O' },
            { 15, 'P' }, { 16, 'Q' }, { 17, 'R' }, { 18, 'S' }, { 19, 'T' },
            { 20, 'U' }, { 21, 'V' }, { 22, 'W' }, { 23, 'X' }, { 24, 'Y' }, { 25, 'Z' },
        };

        public static int GetValueOfChar(char character) {
            character = char.ToUpper(character);
            if (char.IsLetter(character)) {
                return ValueOfChars[character];
            }

            throw new Exception("Specified character doesn't have a value associated with it.");
        }

        public static char GetCharFromValue(int value) {
            if (value >= 0 && value <= 25) {
                return CharOfValues[value];
            }

            throw new Exception("Specified value doesn't have a character associated with it.");
        }

        public static List<int> GetValueArray(string text) {
            List<int> values = new List<int>();
            int i = 0;
            foreach (char character in text) {
                values.Add(GetValueOfChar(character));
                i++;
            }

            return values;
        }

        public static string GetString(IEnumerable<int> values) {
            int length = values.Count();
            char[] characters = new char[length];
            for (int i = 0; i < length; i++) {
                characters[i] = GetCharFromValue(values.ElementAt(i));
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(characters);

            return builder.ToString();
        }

        public static double GetProbability(char character) {
            character = char.ToUpper(character);
            if (char.IsLetter(character)) {
                switch (character) {
                    case 'A':
                        return Constants.AProbability;
                    case 'B':
                        return Constants.BProbability;
                    case 'C':
                        return Constants.CProbability;
                    case 'D':
                        return Constants.DProbability;
                    case 'E':
                        return Constants.EProbability;
                    case 'F':
                        return Constants.FProbability;
                    case 'G':
                        return Constants.GProbability;
                    case 'H':
                        return Constants.HProbability;
                    case 'I':
                        return Constants.IProbability;
                    case 'İ':
                        return Constants.IProbability;
                    case 'J':
                        return Constants.JProbability;
                    case 'K':
                        return Constants.KProbability;
                    case 'L':
                        return Constants.LProbability;
                    case 'M':
                        return Constants.MProbability;
                    case 'N':
                        return Constants.NProbability;
                    case 'O':
                        return Constants.OProbability;
                    case 'P':
                        return Constants.PProbability;
                    case 'Q':
                        return Constants.QProbability;
                    case 'R':
                        return Constants.RProbability;
                    case 'S':
                        return Constants.SProbability;
                    case 'T':
                        return Constants.TProbability;
                    case 'U':
                        return Constants.UProbability;
                    case 'V':
                        return Constants.VProbability;
                    case 'W':
                        return Constants.WProbability;
                    case 'X':
                        return Constants.XProbability;
                    case 'Y':
                        return Constants.YProbability;
                    case 'Z':
                        return Constants.ZProbability;
                    default:
                        break;
                }
            }

            throw new Exception("Specified character doesn't have a probability value associated with it.");
        }

        public static string GenerateRandomKey(int length) {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++) {
                builder.Append(CharOfValues[rnd.Next(26)]);
            }

            return builder.ToString();
        }
    }
}
