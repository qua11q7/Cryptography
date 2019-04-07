using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPNCipher {
    public interface ISubstitution {
        byte InputLength { get; }
        Dictionary<byte, byte> Mapping { get; }
        Dictionary<byte, byte> InverseMapping { get; }
        byte Substitute(byte input);
        byte InverseSubstitute(byte input);
    }
}
