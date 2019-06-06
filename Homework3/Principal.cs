using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RSACipher;

namespace Homework3 {
    public class Principal {
        public string Name { get; }
        public int Generator { get; }
        public int Modulus { get; }
        public int SecretMessage { get; }   // (generator ^ _secret) mod Modulus

        private RSACryptoServiceProvider rsa;
        private RSAParameters _publicKey;
        private RSAParameters _privateKey;

        private readonly int _secret;

        Dictionary<string, int> _secretMessages;
        Dictionary<string, string> _symmetricKeys;
        HashSet<string> _authorizedPrincipals;

        public Principal(string name, int generator, int modulus) {
            Console.WriteLine($"Creating principal {name}...");
            Name = name;
            Generator = generator;
            Modulus = modulus;

            _secret = PrimeGenerator.GetPrime();
            SecretMessage = SquareAndMultiply.Calculate(Generator, _secret, Modulus);

            Console.WriteLine($"{Name} has selected a secret: {_secret}");
            Console.WriteLine($"{Name} calculated its secret message ({Generator}^{_secret}) mod {modulus}: {SecretMessage}");

            rsa = new RSACryptoServiceProvider(2048);
            _publicKey = rsa.ExportParameters(false);
            _privateKey = rsa.ExportParameters(true);

            Console.WriteLine($"{Name} created 2048 bit RSA public and private keys");

            RSAPublicKeyStore.AddPublicKey(Name, _publicKey);

            _secretMessages = new Dictionary<string, int>();
            _symmetricKeys = new Dictionary<string, string>();
            _authorizedPrincipals = new HashSet<string>();

            Console.WriteLine($"Principal {Name} has been created successfully.\r\n");
        }

        public void ReceiveSecretMessage(string principalName, int secretMessage) {
            Console.WriteLine($"{Name} received a secret message from {principalName}: {secretMessage}");
            _secretMessages[principalName] = secretMessage;

            int symmetricKey = SquareAndMultiply.Calculate(secretMessage, _secret, Modulus);
            _symmetricKeys[principalName] = symmetricKey.ToString();

            Console.WriteLine($"{Name} calculated symmetric key K = ({secretMessage}^{_secret}) mod {Modulus}: {symmetricKey}");
        }

        public Tuple<int, string> CreateSecretMessage(string principalName) {
            if (_secretMessages.ContainsKey(principalName) && _symmetricKeys.ContainsKey(principalName)) {
                string encryptedMessage = CreateEncryptedMessage(principalName);
                return new Tuple<int, string>(SecretMessage, encryptedMessage);
            }

            Console.WriteLine("Protocol has not been initiated with principal " + principalName);
            throw new Exception("Protocol has not been initiated with principal " + principalName);
        }

        public void ReceiveEncryptedMessage(string principalName, int secretMessage, string encryptedMessage) {
            ReceiveSecretMessage(principalName, secretMessage);

            bool isVerified = VerifyEncryptedMessage(principalName, encryptedMessage);

            if (!isVerified) {
                Console.WriteLine($"Principal {principalName} has not been authorized.");
                throw new Exception($"Principal {principalName} has not been authorized.");
            }
            else
                _authorizedPrincipals.Add(principalName);
        }

        public string CreateEncryptedMessage(string principalName) {
            string concatenatedSecretMessages = (((long)SecretMessage << 32) | _secretMessages[principalName]).ToString();
            byte[] concatenatedSecretMessageBytes = Encoding.Unicode.GetBytes(concatenatedSecretMessages);
            byte[] signedSecretMessageBytes = rsa.SignData(concatenatedSecretMessageBytes, CryptoConfig.MapNameToOID("SHA512"));
            string signedSecretMessage = Convert.ToBase64String(signedSecretMessageBytes);

            string encryptedMessage = Encryption.Encrypt(signedSecretMessage, _symmetricKeys[principalName]);

            Console.WriteLine($"{Name} is creating encrypted message Ek(S({SecretMessage}, {_secretMessages[principalName]}))");
            Console.WriteLine($"{Name}, {SecretMessage}, {_secretMessages[principalName]}: {concatenatedSecretMessages}");
            Console.WriteLine($"{Name}, Signed message: {signedSecretMessage}");
            Console.WriteLine($"{Name}, Encrypted message: {encryptedMessage}");
            Console.WriteLine($"{Name} created encrypted message successfully.");

            return encryptedMessage;
        }

        public void ReceiveEncryptedMessage(string principalName, string encryptedMessage) {
            bool isVerified = VerifyEncryptedMessage(principalName, encryptedMessage);

            if (!isVerified) {
                Console.WriteLine($"Principal {principalName} has not been authorized.");
                throw new Exception($"Principal {principalName} has not been authorized.");
            }
            else
                _authorizedPrincipals.Add(principalName);
        }

        public Tuple<string, string> CreateEncryptedMessage(string message, string principalName) {
            if (!_authorizedPrincipals.Contains(principalName))
                throw new Exception($"Principal {principalName} has not been authorized.");


            string encryptedMessage = Encryption.Encrypt(message, _symmetricKeys[principalName]);
            string hashString = GetHashValue(message);

            Console.WriteLine($"\r\n{Name} is sending a message to {principalName}...");
            Console.WriteLine($"\tMessage: {message}");
            Console.WriteLine($"\tEncrypted Message: {encryptedMessage}");
            Console.WriteLine($"\tMessage Hash with SHA256: {hashString}");

            return new Tuple<string, string>(encryptedMessage, hashString);
        }

        public string GetEncryptedMessage(Tuple<string, string> encryptedMessage, string principalName) {
            if (!_authorizedPrincipals.Contains(principalName))
                throw new Exception($"Principal {principalName} has not been authorized.");

            string decryptedMessage = Encryption.Decrypt(encryptedMessage.Item1, _symmetricKeys[principalName]);
            string expectedHashValue = GetHashValue(decryptedMessage);

            Console.WriteLine($"\r\n{Name} received a message from {principalName}");
            Console.WriteLine($"\tEncrypted Message and Hash: {encryptedMessage}");
            Console.WriteLine($"\tDecrypted Message: {decryptedMessage}");
            Console.WriteLine($"\tDecrypted Message Hash with SHA256: {expectedHashValue}");

            if (expectedHashValue != encryptedMessage.Item2) {
                Console.WriteLine("Received encrypted message has been altered");
                throw new Exception("Received encrypted message has been altered!");
            } else {
                Console.WriteLine($"\tDecrypted message hash value and received message hash value is same.");
            }

            return decryptedMessage;
        }

        private bool VerifyEncryptedMessage(string principalName, string encryptedMessage) {
            Console.WriteLine($"{Name} is decrypting and verifying the encrypted message...");

            RSACryptoServiceProvider rsaVerify = new RSACryptoServiceProvider();
            rsaVerify.ImportParameters(RSAPublicKeyStore.GetPublicKey(principalName));

            string message = (((long)_secretMessages[principalName] << 32) | SecretMessage).ToString();
            byte[] messageBytes = Encoding.Unicode.GetBytes(message);

            string decryptedMessage = Encryption.Decrypt(encryptedMessage, _symmetricKeys[principalName]);
            byte[] decryptedMessageBytes = Convert.FromBase64String(decryptedMessage);
            bool isVerified = rsaVerify.VerifyData(messageBytes, CryptoConfig.MapNameToOID("SHA512"), decryptedMessageBytes);

            Console.WriteLine($"{Name}, decrypted message: {decryptedMessage}");
            Console.WriteLine($"{Name}, verifying decrpyted message with {principalName}'s public key...");
            if (isVerified)
                Console.WriteLine($"{Name}, verification is successfull.");
            else
                Console.WriteLine($"{Name}, encrypted message cannot be verified!");

            return isVerified;
        }

        private string GetHashValue(string message) {
            using (SHA256 sha = SHA256.Create()) {
                byte[] messageBytes = Encoding.Unicode.GetBytes(message);
                byte[] hashValue = sha.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashValue);
            }
        }
    }
}
