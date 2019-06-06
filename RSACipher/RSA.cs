using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSACipher {
    public sealed class RSA {
        public long N { get; }
        public long E { get; }

        public RSAPublicKey PublicKey { get; }

        private readonly int _p;
        private readonly int _q;
        private readonly long _d;

        private RSAPrivateKey _privateKey;

        public RSA() {
            _p = PrimeGenerator.GetPrime();
            _q = PrimeGenerator.GetPrime();

            N = _p * _q;

            long psi = (_p - 1) * (_q - 1);
            E = PrimeGenerator.GetPrime();
            _d = MultiplicativeInverse.Calculate(E, psi);

            PublicKey = new RSAPublicKey(N, E);
            _privateKey = new RSAPrivateKey(_p, _q);
        }

        public long Sign(long message) {
            return SquareAndMultiply.Calculate(message, _d, N);
        }

        public long Verify(long signedMessage) {
            return Verify(signedMessage, PublicKey);
        }

        public long Verify(long signedMessage, RSAPublicKey publicKey) {
            return Verify(signedMessage, publicKey.E, publicKey.N);
        }

        public long Verify(long signedMessage, long E, long N) {
            return SquareAndMultiply.Calculate(signedMessage, E, N);
        }
    }

    public sealed class RSAPublicKey {
        public long N { get; }
        public long E { get; }

        public RSAPublicKey(long n, long e) {
            N = n;
            E = e;
        }
    }

    public sealed class RSAPrivateKey {
        public int P { get; }
        public int Q { get; }

        public RSAPrivateKey(int p, int q) {
            P = p;
            Q = q;
        }
    }
}
