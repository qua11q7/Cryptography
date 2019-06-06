using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
    public static class SquareAndMultiply {
        public static int Calculate(int generator, int exponent, int modulus) {
            long result = 1;
            for (int i = 31; i >= 0; i--) {
                result = (result * result) % modulus;
                if (((exponent >> i) & 1) == 1) {
                    result = (result * generator) % modulus;
                }
            }

            return (int)result;
        }

        public static long Calculate(long generator, long exponent, long modulus) {
            long result = 1;
            for (int i = 61; i >= 0; i--) {
                result = (result * result) % modulus;
                if (((exponent >> i) & 1) == 1) {
                    result = (result * generator) % modulus;
                }
            }

            return (int)result;
        }
    }
}
