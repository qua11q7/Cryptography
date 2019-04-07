using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public class LFSRKeyScheduler : IKeyScheduler {
        public int MasterKey { get; }
        private IEnumerable<int> RoundKeys { get; set; }

        public LFSRKeyScheduler(int key) {
            MasterKey = key;
            RoundKeys = GetRoundKeys(10);
        }

        public IEnumerable<int> GetRoundKeys(int roundCount) {
            if (RoundKeys != null && roundCount + 1 < RoundKeys.Count())
                return RoundKeys.Take(roundCount + 1);

            int[] roundKeys = new int[roundCount + 1];
            int previousKey = MasterKey;

            for (int i = 0; i <= roundCount; i++) {
                roundKeys[i] = ((previousKey << 15) ^
                                  (previousKey << 13) ^
                                  (previousKey << 11) ^
                                  (previousKey << 10)) & 32768;
                roundKeys[i] = roundKeys[i] | (previousKey >> 1);

                previousKey = roundKeys[i];
            }

            RoundKeys = roundKeys;
            return RoundKeys;
        }
    }
}
