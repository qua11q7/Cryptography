using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSACipher {
    public sealed class RSAPublicKeyStore {
        private static RSAPublicKeyStore Instance { get { return SafeRSAPublicKeyStore.instance; } }

        private class SafeRSAPublicKeyStore {
            static SafeRSAPublicKeyStore() { }
            internal static readonly RSAPublicKeyStore instance = new RSAPublicKeyStore();
        }

        private RSAPublicKeyStore() {
            _RSAPublicKeys = new Dictionary<string, RSAParameters>();
        }

        private Dictionary<string, RSAParameters> _RSAPublicKeys;
        
        public static void AddPublicKey(string principalName, RSAParameters publicKey) {
            Instance._RSAPublicKeys[principalName] = publicKey;
        }

        public static RSAParameters GetPublicKey(string principalName) {
            if (!Instance._RSAPublicKeys.ContainsKey(principalName))
                throw new Exception($"Principal {principalName} doesn't have stored public key.");
            return Instance._RSAPublicKeys[principalName];
        }
    }
}
