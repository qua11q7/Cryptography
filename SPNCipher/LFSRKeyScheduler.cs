using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public class LFSRKeyScheduler : IKeyScheduler {
        public int MasterKey { get; }

        public LFSRKeyScheduler(int key) {
            MasterKey = key;
        }

        public IEnumerable<int> GetRoundKeys(int roundCount) {
            int[] roundKeys = new int[roundCount + 1];
            roundKeys[0] = MasterKey;
            int previousKey = MasterKey;

            for (int i = 0; i < roundCount; i++) {
                roundKeys[i+1] = ((previousKey << 15) ^
                                  (previousKey << 13) ^
                                  (previousKey << 11) ^
                                  (previousKey << 10)) & 32768;
                roundKeys[i+1] = roundKeys[i+1] | (previousKey >> 1);

                previousKey = roundKeys[i+1];
            }

            return roundKeys;
        }
    }
}
