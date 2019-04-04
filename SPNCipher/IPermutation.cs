using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    internal interface IPermutation {
        Dictionary<int, int> Mapping { get; }
        int Permutate(int input);
    }
}
