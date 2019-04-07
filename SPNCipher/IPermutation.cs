using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public interface IPermutation {
        Dictionary<int, int> Mapping { get; }
        Dictionary<int, int> InverseMapping { get; }
        int Permutate(int input);
        int InversePermutate(int input);
    }
}
