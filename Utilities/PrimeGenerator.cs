using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers {
    public static class PrimeGenerator {
        private static List<int> _primes = new List<int>();
        private static Random _random = new Random();
        static PrimeGenerator() {
            var lines = System.IO.File.ReadLines(@"primes-to-200k.txt");
            var primes = lines.Select(l => int.Parse(l));
            _primes.AddRange(primes);
        }

        public static int GetPrime() {
            return _primes[_random.Next(_primes.Count)];
        }
    }
}
