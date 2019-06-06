using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
    public static class MultiplicativeInverse {
        // Caclulates multiplicative inverse by using extended Euclidian Algorithm.
        // gcd(value, modulus) must be 1.
        public static long Calculate(long value, long modulus) {
            long m0 = modulus;
            long y = 0, x = 1;

            if (modulus == 1)
                return 0;

            while (value > 1) {
                // q is quotient 
                long q = value / modulus;

                long t = modulus;

                // m is remainder now, process 
                // same as Euclid's algo 
                modulus = value % modulus;
                value = t;
                t = y;

                // Update x and y 
                y = x - q * y;
                x = t;
            }

            // Make x positive 
            if (x < 0)
                x += m0;

            return x;
        }
    }
}
