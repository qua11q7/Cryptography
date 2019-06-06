using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework3 {
    class Program {
        static void Main(string[] args) {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            // SYSTEM PARAMETERS FOR DIFFIE HELLMAN KEY EXCHANGE
            const int generator = 32941;    // denoted as g or alpha
            const int modulus = 268813;     // denoted as p

            // Create two parties, both parties create their own RSA public and private keys.
            Principal alice = new Principal("Alice", generator, modulus);
            Principal bob = new Principal("Bob", generator, modulus);

            Console.WriteLine("INITIATING PROTOCOL");
            Console.WriteLine("\r\nProtocol: Alice ---- g^x mod P ----> Bob");
            #region Alice ---- g^x mod P ----> Bob
            // Get calculated g^x mod P secret message.
            int secretMessageOfAlice = alice.SecretMessage;
            // Send g^x mod P secret message to Bob,
            // Bob stores Alice's secret message, calculates its own g^y mod P secret message.
            // Bob then calculates and stores K = (g^x)^y mod P symmetric key for communication with Alice
            bob.ReceiveSecretMessage(alice.Name, secretMessageOfAlice);
            #endregion

            Console.WriteLine("\r\nProtocol: Bob ---- g^y mod P, Ek(Sb(g^y mod P, g^x mod P)) ----> Alice");
            #region Bob ---- g^y mod P, Ek(Sb(g^y mod P, g^x mod P)) ----> Alice
            // Bob concatenates g^y mod P and g^x mod P.
            // Bob signs this concatenated secret messages with its private RSA key.
            // Bob encrypts this signed message with symmetric key K
            Tuple<int, string> encryptedMessageOfBob = bob.CreateSecretMessage(alice.Name);
            // Alice calculates and stores K = (g^y)^x mod P symmetric key for communication with Bob.
            // Alice decrypts the encrypted message with this symmetric key K.
            // Alice verifies signed message with Bob's public key.
            alice.ReceiveEncryptedMessage(bob.Name, encryptedMessageOfBob.Item1, encryptedMessageOfBob.Item2);
            #endregion

            Console.WriteLine("\r\nProtocol: Alice ---- Ek(Sa(g^x mod P, g^y mod P)) ----> Bob");
            #region Alice ---- Ek(Sa(g^x mod P, g^y mod P)) ----> Bob
            // Alice concatenates g^x mod P and g^y mod P.
            // Alice signs this concatenated secret messages with its private RSA key.
            // Alice encryptes this signed message with symmetric key K
            string encryptedMessageOfAlice = alice.CreateEncryptedMessage(bob.Name);
            // Bob decrypts the encrypted message with symmetric key K
            // Bob verifies signed message with Alice's public key.
            bob.ReceiveEncryptedMessage(alice.Name, encryptedMessageOfAlice);
            #endregion
            Console.WriteLine($"\r\nPROTOCOL HAS BEEN INITIATED SUCCESSFULLY BETWEEN {alice.Name} AND {bob.Name}.\r\n");

            string messageFromAliceToBob = "Hi Bob, It's me Alice!";
            Tuple<string, string> encryptedMessageFromAlice = alice.CreateEncryptedMessage(messageFromAliceToBob, bob.Name);
            bob.GetEncryptedMessage(encryptedMessageFromAlice, alice.Name);

            string messageFromBobToAlice = "Hi Alice, it is really you!";
            Tuple<string, string> encryptedMessageFromBob = bob.CreateEncryptedMessage(messageFromBobToAlice, alice.Name);
            alice.GetEncryptedMessage(encryptedMessageFromBob, bob.Name);
            
            watch.Stop();
            Console.WriteLine("Elapsed ms: " + watch.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }
}
