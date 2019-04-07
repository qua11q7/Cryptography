using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public interface ISPNCipher {
        int RoundCount { get; }
        int SubstitutionBoxCount { get; }
        int BlockLength { get; }
        string Name { get; }

        ISubstitution Substitution { get; }
        IPermutation Permutation { get; }
        IKeyScheduler KeyScheduler { get; }

        int Encode(int input);
        int Decode(int input);
    }
}
