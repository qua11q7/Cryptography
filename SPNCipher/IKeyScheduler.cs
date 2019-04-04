using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    internal interface IKeyScheduler {
        int MasterKey { get; }
        IEnumerable<int> GetRoundKeys(int roundCount);
    }
}
