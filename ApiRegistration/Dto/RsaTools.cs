using System.Security.Cryptography;

namespace ApiRegistration.Dto
{
    public class RsaTools
    {
        public static RSA GetPrivateKey()
        {
            string? f = File.ReadAllText("rsa/private_key.pem");

            RSA rsa = RSA.Create();

            rsa.ImportFromPem(f);

            return rsa;
        }

        public static RSA GetPublicKey()
        {
            string? f = File.ReadAllText("rsa/public_key.pem");

            RSA rsa = RSA.Create();

            rsa.ImportFromPem(f);

            return rsa;
        }
    }
}
