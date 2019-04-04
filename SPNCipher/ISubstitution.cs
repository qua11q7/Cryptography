using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    internal interface ISubstitution {
        Dictionary<byte, byte> Mapping { get; }
        byte Substitute(byte input);
    }
}
