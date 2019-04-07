using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public class SPNCipher1 : BaseSPN {
        public SPNCipher1(int masterKey) : base(new SubstitutionBox1(), new Permutation1(), new LFSRKeyScheduler(masterKey)) {
            Name = "SPN Block Cipher 1";
        }
    }

    public class SPNCipher2 : BaseSPN {
        public SPNCipher2(int masterKey) : base(new SubstitutionBox2(), new Permutation1(), new LFSRKeyScheduler(masterKey)) {
            Name = "SPN Block Cipher 2";
        }
    }

    public class SPNCipher3 : BaseSPN {
        public SPNCipher3(int masterKey) : base(new SubstitutionBox2(), new Permutation2(), new LFSRKeyScheduler(masterKey)) {
            Name = "SPN Block Cipher 3";
        }
    }

    public class SPNCipher4 : BaseSPN {
        public SPNCipher4(int masterKey) : base(new SubstitutionBox3(), new Permutation1(), new LFSRKeyScheduler(masterKey)) {
            Name = "SPN Block Cipher 4";
        }
    }
}
